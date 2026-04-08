using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using DTO.Entity;
using DTO.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using SysNet.Converters;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;
using Windows.Security.EnterpriseData;

namespace ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private ILoginRepository Q;
        
        public ReactiveCommand<Unit, Unit> EntraCommand { get; }
        public ReactiveCommand<Unit, Unit> EsciCommand { get; }

        private CancellationTokenSource _loadingCts;

        protected override IObservable<bool> canSave => this.WhenAnyValue(
            x => x.PasswordText,
            x => x.BindingT,
            (pass, operatore) =>
                !string.IsNullOrWhiteSpace(pass) &&
                operatore != null &&
                pass == operatore.Password);

        
        public LoginViewModel(IScreen host,
                              ILoginRepository loginRepository = null) : base(host)
        {
            Q = loginRepository ?? Locator.Current.GetService<ILoginRepository>();


            EntraCommand = SaveCommand;
            //EntraCommand = ReactiveCommand.CreateFromTask(OnEntra);

            EsciCommand = ReactiveCommand.Create(() =>
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                    lifetime.Shutdown();
            });

            this.WhenActivated(d =>
            {
               
                EntraCommand?.DisposeWith(d);
                EsciCommand?.DisposeWith(d);
            });
 
        }
        
        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
            BindingT = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            IsLoading = true;
            //Q = new(); // Istanza locale del Repository
            List<LoginDTO> dbData = await Q.GetOperatoriAbilitati(token);

            token.ThrowIfCancellationRequested();

            if (dbData != null && dbData.Count != 0)
            {

                // Trasforma l'Expression in una funzione e usala con LINQ .Select()
                // Aggiorna la DataSource della UI
                DataSource = dbData.Select(dto => new LoginMap(dto)).ToList();

                // Seleziona il primo
                BindingT = DataSource[0];
            }
            SetFocus(PasswordFocus);
            
            
        }

        //private async Task OnPasswordFocus()
        //{
        //    // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
        //    await Task.Delay(200);

        //    try
        //    {
        //        await PasswordFocus.Handle(Unit.Default);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Evita crash se l'handler non è ancora pronto o la vista è già chiusa
        //        System.Diagnostics.Debug.WriteLine("Interaction Focus fallita: " + ex.Message);
        //    }
        //}

        protected override async Task OnSaving()
        {
            try
            {
                // Salva le impostazioni dell'operatore selezionato
                await Q.SaveSettings(BindingT.ToDto());

                // Naviga al Menu principale resettando lo stack di navigazione
                await GoToMenu();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Login fallito durante il salvataggio o la navigazione: {ex.Message}");
                // Qui potresti aggiungere un'interaction per mostrare un messaggio di errore all'utente
            }

            await Task.CompletedTask;
        }

        private async Task GoToMenu()
        {
            await HostScreen.Router.NavigateAndReset.Execute(new EmptyViewModel(HostScreen));
            await HostScreen.Router.NavigateAndReset.Execute(new MenuViewModel(HostScreen));
        }
    }
        
    public partial class LoginViewModel
    {
        //DataSource della ComboBox
        private List<LoginMap> _datasource;
        public List<LoginMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        private string _mypassordtext = string.Empty;
        public string PasswordText
        {
            get => _mypassordtext;
            set => this.RaiseAndSetIfChanged(ref _mypassordtext, value);
        }

        private LoginMap bindingt = new();
        public LoginMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
        }

        #region Observable

        public Interaction<Unit, Unit> PasswordFocus { get; } = new();

        #endregion

    }
}
