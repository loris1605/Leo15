using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Common.ViewModels;
using Core;
using DTO.Entity;
using DTO.Repository;
using LoginModule.Maps;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;


namespace ViewModel
{
    public partial class LoginVM : BaseViewModel, ILoginViewModel
    {
        private ILoginRepository Q;
        
        public ReactiveCommand<Unit, Unit> EntraCommand { get; }
        public ReactiveCommand<Unit, Unit> EsciCommand { get; }

        protected override IObservable<bool> canSave => this.WhenAnyValue(
            x => x.PasswordText,
            x => x.BindingT,
            (pass, operatore) =>
                !string.IsNullOrWhiteSpace(pass) &&
                operatore != null &&
                pass == operatore.Password);

        
        public LoginVM(IScreen host,
                              ILoginRepository loginRepository) : base(host)
        {
            Q = loginRepository;

            EntraCommand = SaveCommand;
            //EntraCommand = ReactiveCommand.CreateFromTask(OnEntra);

            EsciCommand = EscPressedCommand;

            this.WhenActivated(d =>
            {
               
                EntraCommand?.DisposeWith(d);
                EsciCommand?.DisposeWith(d);
            });
 
        }
        
        protected override void OnFinalDestruction()
        {
            Q = null!;
            DataSource = null!;
            BindingT = null!;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            List<LoginDTO> dbData = await Q.GetOperatoriAbilitati(token);
            
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
            //await HostScreen.Router.NavigateAndReset.Execute(new EmptyViewModel(HostScreen));
            //await HostScreen.Router.NavigateAndReset.Execute(new MenuViewModel(HostScreen,
            //                                                                   Locator.Current.GetService<IMenuRepository>()));
        }

        protected async override Task OnEsc()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.Shutdown();

            await Task.CompletedTask;

            
        }
    }
        
    public partial class LoginVM
    {
        //DataSource della ComboBox
        private List<LoginMap> _datasource = new();
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
