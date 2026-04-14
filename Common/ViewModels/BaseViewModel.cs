using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Common.Sys;

namespace Common.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        //Sequenza di operazioni
        //Eliminare il public override string UrlPathSegment => "";
        //Eliminare il static int deadentries;
        //Eliminare il public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        // Implementazione di IRoutableViewModel
        public string UrlPathSegment { get; }
        public IScreen HostScreen { get; }

        protected bool _isClosing = false;

        protected CancellationTokenSource? _cts;
        protected CancellationToken token => _cts?.Token ?? CancellationToken.None;

        // Implementazione di IActivatableViewModel
        public ViewModelActivator Activator { get; } = new();

        
        public ReactiveCommand<Unit, Unit> AppExitCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> EscPressedCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        protected virtual IObservable<bool> canSave => Observable.Return(true);
        protected virtual IObservable<bool> canEsc => Observable.Return(true);



        public BaseViewModel(IScreen hostScreen, string urlPathSegment = "")
        {
            Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()} caricato *****");

            HostScreen = hostScreen;
            UrlPathSegment = urlPathSegment ?? this.GetType().Name;

            // 1. Definiamo un buffer temporale (es. 500ms) per evitare click compulsivi
            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .StartWith(true); // Serve per non aspettare 500ms al primo avvio

            // 2. Applichiamo il ritardo al Save
            var canExecuteSave = isNotLoading
                .CombineLatest(canSave, (notLoading, childCanSave) => notLoading && childCanSave)
                .ObserveOn(RxApp.MainThreadScheduler);

            // 3. Applichiamo il ritardo all'Esc (e correggiamo il riferimento a canEsc)
            var canExecuteEsc = isNotLoading
                .CombineLatest(canEsc, (notLoading, childCanEsc) => notLoading && childCanEsc)
                .ObserveOn(RxApp.MainThreadScheduler);

            var canExecuteCombined = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler) // Impedisce riattivazioni repentine
                .CombineLatest(canSave, (isNotLoading, childCanSave) => isNotLoading && childCanSave)
                .ObserveOn(RxApp.MainThreadScheduler);

            LoadCommand = ReactiveCommand.CreateFromTask(ExecuteLoading,
                    this.WhenAnyValue(x => x.IsLoading, loading => !loading));
            SaveCommand = ReactiveCommand.CreateFromTask(ExecuteSaving, canExecuteCombined);
            AppExitCommand = ReactiveCommand.Create(OnAppShutDown);
            EscPressedCommand = ReactiveCommand.CreateFromTask(ExecuteEscing, canExecuteEsc);
            

#if DEBUG

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

#endif

            // Gestione dell'attivazione/disattivazione
            this.WhenActivated(disposables =>
            {
                _cts = new CancellationTokenSource();

                Observable.Return(Unit.Default)
                .InvokeCommand(LoadCommand)
                .DisposeWith(disposables);

                
                this.LoadCommand.ThrownExceptions
                    .Subscribe(ex =>
                    {
                        // Qui gestisci l'errore (es. mostri una notifica o logghi)
                        Debug.WriteLine($"***** [VM] {this.GetType().Name} Errore durante il caricamento: {ex.Message}");
                   
                    })
                .DisposeWith(disposables);

                Disposable.Create(() => {

                    _cts.Cancel();
                    _cts.Dispose();
                    _cts = null;

                    OnFinalDestruction();
#if DEBUG
                    Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()} disposed *****");
#endif
                }).DisposeWith(disposables);

                AppExitCommand?.DisposeWith(disposables);
                SaveCommand?.DisposeWith(disposables);
                LoadCommand?.DisposeWith(disposables);
                EscPressedCommand?.DisposeWith(disposables);
            });
        }

        protected async Task ExecuteLoading()
        {
            // Se stiamo già caricando o chiudendo, usciamo
            if (_isClosing) return;

            await Task.Delay(50);

            IsLoading = true;
            try
            {
                await OnLoading();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Caricamento annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE: {ex.Message}");
            }
            finally
            {
                // Se durante il caricamento è successo qualcosa che ha triggerato la chiusura
                // non riabilitiamo i controlli
                if (!_isClosing)
                {
                    IsLoading = false;
                }
            }
        }


        protected async Task ExecuteSaving()
        {
            if (_isClosing) return; // Se stiamo uscendo, ignora ogni nuovo click

            await Task.Delay(50);

            IsLoading = true;
            try
            {
                await OnSaving();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Caricamento annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE: {ex.Message}");
            }
            finally
            {
                // Se OnSaving ha chiamato OnBack, _isClosing sarà true.
                // In tal caso, lasciamo IsLoading = true per tenere i pulsanti disabilitati
                if (!_isClosing)
                {
                    IsLoading = false;
                }
            }
        }


        protected async Task ExecuteEscing()
        {
            if (_isClosing) return; // Se stiamo già uscendo, ignora il secondo Esc

            await Task.Delay(50);

            IsLoading = true;
            try
            {
                await OnEsc();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} Esc annullato.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"***** [VM] {this.GetType().Name} ERRORE ESC: {ex.Message}");
            }
            finally
            {
                // Importante: se OnEsc ha avviato la navigazione (OnBack o simile),
                // manteniamo IsLoading = true per evitare che i pulsanti "lampeggino"
                if (!_isClosing)
                {
                    IsLoading = false;
                }
            }
        }


        protected virtual void OnFinalDestruction()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            // Questo log conferma che la LOGICA di rimozione ha funzionato
            Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()}  +" +
                            $"rimosso correttamente dallo stack *****");
        }

#if DEBUG
        ~BaseViewModel()
        {
            // Questo apparirà nella finestra "Output" di Visual Studio
            Debug.WriteLine($"***** [GC] {this.GetType().Name} {this.GetHashCode()} DISTRUTTO *****");
        }
#endif

        protected abstract Task OnLoading();
        protected abstract Task OnSaving();
        protected abstract Task OnEsc();

        protected void OnAppShutDown()
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.Shutdown();
            
        }

        protected void SetFocus(Interaction<Unit, Unit> focusInteraction, int delay = 200)
        {
            // Chiama il tuo metodo esistente passando Unit.Default come input
            TriggerInteraction(focusInteraction, Unit.Default, delay).FireAndForget();
        }


        protected async Task TriggerInteraction<TInput, TOutput>(
        Interaction<TInput, TOutput> interaction,
        TInput input,
        int delayMs = 200)
            {
                // Attendiamo che la View sia agganciata e pronta
                await Task.Delay(delayMs);

                try
                {
                    // Verifichiamo se l'interazione ha almeno un handler registrato
                    // per evitare eccezioni se la View è già stata deattivata
                    await interaction.Handle(input);
                }
                catch (UnhandledInteractionException<TInput, TOutput>)
                {
                Debug.WriteLine($">>> [WARN] Interaction {typeof(TInput).Name}->{typeof(TOutput).Name} non gestita (View deattivata).");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($">>> [ERROR] Errore Interaction: {ex.Message}");
                }
            }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

    }
}
