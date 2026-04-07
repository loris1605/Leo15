using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using SysNet.Converters;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
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

        protected int _deadEntries;

        protected CancellationTokenSource _cts;

        // Implementazione di IActivatableViewModel
        public ViewModelActivator Activator { get; } = new();

        
        public ReactiveCommand<Unit, Unit> AppExitCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        public BaseViewModel(IScreen hostScreen, string urlPathSegment = null)
        {
            Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()} caricato *****");

            HostScreen = hostScreen;
            UrlPathSegment = urlPathSegment ?? this.GetType().Name;
            
            LoadCommand = ReactiveCommand.CreateFromTask(OnLoading);
            AppExitCommand = ReactiveCommand.Create(OnAppShutDown);
            

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

                    OnFinalDestruction();
#if DEBUG
                    Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()} disposed *****");
#endif
                }).DisposeWith(disposables);

                AppExitCommand.DisposeWith(disposables);
                LoadCommand.DisposeWith(disposables);
            });
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

        protected void OnAppShutDown()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
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
