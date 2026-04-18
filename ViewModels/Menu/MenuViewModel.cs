using DTO.Entity;
using DTO.Repository;
using Models.Entity.Global;
using ReactiveUI;
using Splat;
using System.Diagnostics;
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
        public ReactiveCommand<PostazioneMap, Unit> SelezionaPostazioneCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> ApriGiornataCommand { get; }     



        public MenuViewModel(IScreen host,
                             IMenuRepository menuRepository = null) : base(host)
        {
            Q = menuRepository ?? Locator.Current.GetService<IMenuRepository>();

            var canExecute = this.WhenAnyValue(x => x.IsLoading, x => !x);
            var canApri = this.WhenAnyValue(x => x.ApriGiornataEnabled);

            _chiudiGiornataEnabled = this.WhenAnyValue(x => x.ApriGiornataEnabled)
                .Select(x => !x)
                .ToProperty(this, x => x.ChiudiGiornataEnabled);

                // 2. Gestisci la stringa della Sessione
                _sessioneContabile = this.WhenAnyValue(x => x.ApriGiornataEnabled)
                    .Select(v => $"Sessione Contabile {(v ? "Chiusa" : "Aperta")}")
                    .ToProperty(this, x => x.SessioneContabile);

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

            SelezionaPostazioneCommand = ReactiveCommand.CreateFromTask<PostazioneMap>(x => GoToCassa(x), canExecute);
            //{
            //    // 1. Salva la selezione
            //    SelectedPostazione = postazione;

            //    // 2. Esegui la logica necessaria
            //    Debug.WriteLine($"Postazione selezionata: {postazione.NomePostazione}");



            //    // Esempio: aggiorna altri stati della UI o chiama servizi
            //});


            LogoutCommand = ReactiveCommand.CreateFromTask(GoToLogin, canExecute);

            ApriGiornataCommand = ReactiveCommand.Create(OpenGiornata, canApri);



            this.WhenActivated(d => 
            {
                this.WhenAnyValue(x => x.ApriGiornataEnabled)
                .Select(x => !x)
                .BindTo(this, x => x.ChiudiGiornataEnabled)
                .DisposeWith(d);

                this.WhenAnyValue(x => x.ApriGiornataEnabled)
                .Select(enabled => $"Sessione Contabile {(enabled ? "Chiusa" : "Aperta")}")
                .BindTo(this, x => x.SessioneContabile).DisposeWith(d);

                LogoutCommand?.DisposeWith(d);
                SelezionaPostazioneCommand?.DisposeWith(d);
                NavigateCommand?.DisposeWith(d);
                ApriGiornataCommand?.DisposeWith(d);
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
            //SessioneContabile = "Sessione Contabile " + (ApriGiornataEnabled ? "Chiusa" : "Aperta");

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

        private async Task GoToCassa(PostazioneMap map)
        {
            await HostScreen.Router.NavigateAndReset.Execute(new CassaViewModel(HostScreen, map));
        }

        private async Task GoToLogin()
        {
            await HostScreen.Router.NavigateAndReset.Execute(new LoginViewModel(HostScreen));
        }

        protected override async Task OnSaving() => await Task.CompletedTask;

        private void OpenGiornata()
        {
            bool result = Q.OpenGiornata(token);
            if (result)
            {
                ApriGiornataEnabled = false;
                //MessageBox.Show("Giornata aperta con successo!");
            }
            else
            {
                Debug.WriteLine("Errore durante l'apertura della giornata.");
            }
        }   

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

        readonly ObservableAsPropertyHelper<string> _sessioneContabile;
        public string SessioneContabile => _sessioneContabile.Value;

        private bool _apriGiornataEnabled;
        public bool ApriGiornataEnabled
        {
            get => _apriGiornataEnabled;
            set => this.RaiseAndSetIfChanged(ref _apriGiornataEnabled, value);
        }

        private bool _myapripostazioneenabled = false;
        public bool ApriPostazioneEnabled
        {
            get => _myapripostazioneenabled;
            set => this.RaiseAndSetIfChanged(ref _myapripostazioneenabled, value);
        }

        readonly ObservableAsPropertyHelper<bool> _chiudiGiornataEnabled;
        public bool ChiudiGiornataEnabled => _chiudiGiornataEnabled.Value;

        private bool _isMenuReady = false;
        public bool IsMenuReady
        {
            get => _isMenuReady;
            set => this.RaiseAndSetIfChanged(ref _isMenuReady, value);
        }

        private PostazioneMap _selectedPostazione;
        public PostazioneMap SelectedPostazione
        {
            get => _selectedPostazione;
            set => this.RaiseAndSetIfChanged(ref _selectedPostazione, value);
        }

    }

    
}
