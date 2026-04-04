using Models.Entity;
using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial class PostazioneInputBase : InputViewModel
    {
        
        protected string Name => BindingT.NomePostazione.Trim() is null ? "" : BindingT.NomePostazione.Trim();
        int CodicePostazione => BindingT is null ? 0 : BindingT.Id;

        protected bool IsNameEmpty => BindingT is not null && (Name == "");
        protected bool CheckLess2Name => Name.Length < 2;
        //protected bool EsisteName => Q.EsisteName(Name);
        //protected bool EsisteNameUpd => Q.EsisteNameUpd(BindingT);
        protected int GetCodicePostazione => CodicePostazione;

        protected async override Task OnSaving() { await Task.CompletedTask; }

        protected async Task OnFocus(Interaction<Unit, Unit> control)
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);

            try
            {
                await control.Handle(Unit.Default);
            }
            catch (Exception ex)
            {
                // Evita crash se l'handler non è ancora pronto o la vista è già chiusa
                System.Diagnostics.Debug.WriteLine("Interaction Focus fallita: " + ex.Message);
            }
        }

        protected async Task<bool> ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome della posizione";
                await OnFocus(NomeFocus);
                return false;
            }

            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Postazione non valido";
                await OnFocus(NomeFocus);
                return false;
            }

            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
        }

        private void OnBackEsc()
        {
            if (HostScreen is IGroupScreen Host)
            {
                RxApp.MainThreadScheduler.Schedule(() => {
                    Host.InputRouter.NavigationStack.Clear();
                    Host.GroupEnabled = true;
                });
            }
        }

        protected void OnBack(int value = 0)
        {
            if (HostScreen is IGroupScreen Host)
            {
                // Svuota completamente lo stack del router di input
                Host.InputRouter.NavigateBack.Execute();
                Host.InputRouter.NavigationStack.Clear();
                Host.AggiornaGrid(value);
                Host.GroupEnabled = true;
            }
        }

        
    }

    public partial class PostazioneInputBase
    {
        public PostazioneInputBase(IScreen host) : base(host)
        {
            EscPressedCommand = ReactiveCommand.Create(OnBackEsc);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.NomePostazione)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.NomePostazione = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.CodiceTipoPostazione)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.CodiceTipoPostazione = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.CodiceTipoRientro)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.CodiceTipoRientro = val)
                    .DisposeWith(d);

                // Osserva BindingT e CodiceTipoPostazione per calcolare la visibilità
                this.WhenAnyValue(
                    x => x.BindingT,
                    x => x.CodiceTipoPostazione,
                    (bt, codice) => bt is not null && codice == 2) // Questa è la tua logica IsCassa
                .Subscribe(isCassa =>
                {
                    // Se è una cassa, il rientro è visibile (o invisibile, a seconda della tua logica)
                    RientroVisibile = isCassa;
                })
                .DisposeWith(d);


            });
        }

        private string nome = string.Empty;
        public string NomePostazione
        {
            get => nome;
            set => this.RaiseAndSetIfChanged(ref nome, value);
        }

        private int _codiceTipoPostazione = 0;
        public int CodiceTipoPostazione
        {
            get => _codiceTipoPostazione;
            set => this.RaiseAndSetIfChanged(ref _codiceTipoPostazione, value);
        }

        private int _codiceTipoRientro = 0;
        public int CodiceTipoRientro
        {
            get => _codiceTipoRientro;
            set => this.RaiseAndSetIfChanged(ref _codiceTipoRientro, value);
        }

        private bool _rientroVisibile = true;
        public bool RientroVisibile
        {
            get => _rientroVisibile;
            set => this.RaiseAndSetIfChanged(ref _rientroVisibile, value);
        }

        private IList<TipoPostazioneMap> tipoPostazioneMaps = [];
        public IList<TipoPostazioneMap> TipoPostDataSource
        {
            get => tipoPostazioneMaps;
            set => this.RaiseAndSetIfChanged(ref tipoPostazioneMaps, value);
        }

        private IList<TipoRientroMap> _tipoRientroDataSource = [];
        public IList<TipoRientroMap> TipoRientroDataSource
        {
            get => _tipoRientroDataSource;
            set => this.RaiseAndSetIfChanged(ref _tipoRientroDataSource, value);
        }

        private PostazioneMap bindingt = Create<PostazioneMap>.Instance();
        public PostazioneMap BindingT
        {
            get => bindingt;
            set
            {
                // 1. Aggiorna il riferimento (fondamentale per RaiseAndSetIfChanged)
                this.RaiseAndSetIfChanged(ref bindingt, value);

                // 2. Se carichi una postazione, allinea la UI al modello
                if (value != null)
                {
                    this.NomePostazione = value.NomePostazione ?? "";
                    this.CodiceTipoPostazione = value.CodiceTipoPostazione;
                    this.CodiceTipoRientro = value.CodiceTipoRientro;
                    
                }
            }
        }

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        
    }
}
