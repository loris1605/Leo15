using DTO.Entity;
using DTO.Repository;
using Models.Entity.Global;
using ReactiveUI;
using Splat;
using System.Diagnostics.Metrics;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class MenuViewModel : BaseViewModel
    {
        private IMenuRepository Q { get; set; }
        
        public ReactiveCommand<string, Unit> NavigateCommand { get; }
        public ReactiveCommand<string, Unit> CassaCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
                
        
        public MenuViewModel(IScreen host,
                             IMenuRepository menuRepository = null) : base(host)
        {
            Q = menuRepository ?? Locator.Current.GetService<IMenuRepository>();

            var canExecute = this.WhenAnyValue(x => x.IsLoading, x => !x);

            NavigateCommand = ReactiveCommand.CreateFromTask<string>(async (dest) =>
            {
                IRoutableViewModel page = dest switch
                {
                    "Connection" => new ConnectionViewModel(HostScreen),
                    "Soci" => new SociViewModel(HostScreen),
                    "Configurazione" => new ConfigurazioneViewModel(HostScreen), // Corretto l'errore precedente
                    _ => null
                };

                if (page != null)
                    await HostScreen.Router.NavigateAndReset.Execute(page);
            }, canExecute);

            CassaCommand = ReactiveCommand.CreateFromTask<string>(param => OnCassa(param), canExecute);
            LogoutCommand = ReactiveCommand.CreateFromTask(GoToLogin, canExecute);
                                   

            this.WhenActivated(d => 
            {
                LogoutCommand?.DisposeWith(d);
                CassaCommand?.DisposeWith(d);
                NavigateCommand?.DisposeWith(d);
                             
            });
                
        }

        protected override void OnFinalDestruction()
        {
            CassaPostazioniDataSource?.Clear(); // Svuota la lista
            CassaPostazioniDataSource = null;   // Rimuovi il riferimento
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            if (GlobalValuesC.MySetting == null) return;

            AttivaPermessi();

            List<PostazioneDTO> listaDto = await Q.CaricaPostazioniCassa(GlobalValuesC.MySetting.IDOPERATORE, token);

            CassaPostazioniDataSource = listaDto
                                        .Select(dto => new PostazioneMap(dto))
                                        .ToList();

            ApriGiornataEnabled = !(await Q.EsisteGiornataAperta(token));
            if (GlobalValuesC.MySetting is null) await Task.CompletedTask;
            if (GlobalValuesC.MySetting.POSTAZIONI is not null)
            {
                if (GlobalValuesC.MySetting.POSTAZIONI.Count == 0) ApriPostazioneEnabled = false;
            }

            
        }

        private void AttivaPermessi()
        {
            if (GlobalValuesC.MySetting is null) return;

            OperatoreName = "Operatore : " + GlobalValuesC.MySetting.NOMEOPERATORE;
            SessioneContabile = "Sessione Contabile " + (ApriGiornataEnabled ? "Chiusa" : "Aperta");

            if (GlobalValuesC.MySetting.POSTAZIONI is null) return;

            try
            {
                foreach (PostazioneXC Element in GlobalValuesC.MySetting.POSTAZIONI)
                {
                    switch (Element.TIPOPOSTAZIONE)
                    {
                        case (int)Enums.Postazioni.Amministratore:
                            AmministratoreVisible = true;
                            ReportVisible = true;
                            break;

                        case (int)Enums.Postazioni.Cassa:
                            CassaVisible = true;
                            ReportVisible = true;
                            break;

                        case (int)Enums.Postazioni.Bar:
                            BarVisible = true;
                            break;

                        case (int)Enums.Postazioni.Guardaroba:
                            GuardarobaVisible = true;
                            break;

                        case (int)Enums.Postazioni.Pulizie:
                            PulizieVisible = true;
                            break;

                    }
                }
            }
            catch (NullReferenceException)
            {
                return;
            }

            IsMenuReady = true;


        }

        private async Task OnCassa(string x)
        {
            await Task.CompletedTask;
            //GlobalValuesC.MyPostazione = new()
            //{
            //    DESCPOSTAZIONE = x
            //};
            //MessageBox.Show(x);
            //MainNavigator.NotifyColleagues("CassaBase");
        }

        private async Task GoToLogin()
        {
            await HostScreen.Router.NavigateAndReset.Execute(new LoginViewModel(HostScreen));
        }

        protected override async Task OnSaving() => await Task.CompletedTask;

        protected override Task OnEsc()
        {
            throw new NotImplementedException();
        }
    }

    public partial class MenuViewModel
    {
        //Voci visibili nel Menu
        private List<bool> _visibile = [];
        public List<bool> Visibile
        {
            get => _visibile;
            set => this.RaiseAndSetIfChanged(ref _visibile, value);
        }

        private bool _myamministratorevisible;
        public bool AmministratoreVisible
        {
            get => _myamministratorevisible;
            set => this.RaiseAndSetIfChanged(ref _myamministratorevisible, value);
        }

        private bool _myreportvisible;
        public bool ReportVisible
        {
            get => _myreportvisible;
            set => this.RaiseAndSetIfChanged(ref _myreportvisible, value);
        }

        private bool _mycassavisible;
        public bool CassaVisible
        {
            get => _mycassavisible;
            set => this.RaiseAndSetIfChanged(ref _mycassavisible, value);
        }

        private bool _mybarvisible;
        public bool BarVisible
        {
            get => _mybarvisible;
            set => this.RaiseAndSetIfChanged(ref _mybarvisible, value);
        }

        private bool _myguardarobavisible;
        public bool GuardarobaVisible
        {
            get => _myguardarobavisible;
            set => this.RaiseAndSetIfChanged(ref _myguardarobavisible, value);
        }

        private bool _mypulizievisible;
        public bool PulizieVisible
        {
            get => _mypulizievisible;
            set => this.RaiseAndSetIfChanged(ref _mypulizievisible, value);
        }

        private List<PostazioneMap> _mycassapostazionidatasource = null;
        public List<PostazioneMap> CassaPostazioniDataSource
        {
            get => _mycassapostazionidatasource;
            set => this.RaiseAndSetIfChanged(ref _mycassapostazionidatasource, value);
        }

        private string _myoperatorename = string.Empty;
        public string OperatoreName
        {
            get => _myoperatorename;
            set => this.RaiseAndSetIfChanged(ref _myoperatorename, value);
        }

        private string _mysessionecontabile = string.Empty;
        public string SessioneContabile
        {
            get => _mysessionecontabile;
            set => this.RaiseAndSetIfChanged(ref _mysessionecontabile, value);
        }

        private bool _myaprigiornataenabled = false;
        public bool ApriGiornataEnabled
        {
            get => _myaprigiornataenabled;
            set
            {
                this.RaiseAndSetIfChanged(ref _myaprigiornataenabled, value);
                ChiudiGiornataEnabled = !value;
                Visibile[(int)Enums.Postazioni.Cassa] = !value;
                SessioneContabile = "Sessione Contabile " + (value ? "Chiusa" : "Aperta");
            }
        }

        private bool _myapripostazioneenabled = false;
        public bool ApriPostazioneEnabled
        {
            get => _myapripostazioneenabled;
            set => this.RaiseAndSetIfChanged(ref _myapripostazioneenabled, value);
        }

        private bool _mychiudigiornataenabled = false;
        public bool ChiudiGiornataEnabled
        {
            get => _mychiudigiornataenabled;
            set => this.RaiseAndSetIfChanged(ref _mychiudigiornataenabled, value);
        }

        private bool _isMenuReady = false;
        public bool IsMenuReady
        {
            get => _isMenuReady;
            set => this.RaiseAndSetIfChanged(ref _isMenuReady, value);
        }
        
    }

    
}
