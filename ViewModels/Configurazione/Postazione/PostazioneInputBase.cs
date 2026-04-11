using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

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
        protected async override Task OnLoading() => await Task.CompletedTask;

        protected bool ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome della posizione";
                SetFocus(NomeFocus);
                return false;
            }

            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Postazione non valido";
                SetFocus(NomeFocus);
                return false;
            }

            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
        }


        protected void OnBack(int value = 0)
        {
            if (HostScreen is IGroupScreen Host)
            {
                // Svuota completamente lo stack del router di input
                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    // Eseguiamo la navigazione e la pulizia
                    Host.InputRouter.NavigateBack.Execute().Subscribe();
                    Host.InputRouter.NavigationStack.Clear();

                    // Aggiorniamo la grid e riabilitiamo i controlli
                    Host.AggiornaGridByInt(value);
                    Host.GroupEnabled = true;
                });
            }
        }




        protected override async Task OnEsc()
        {
            if (HostScreen is IGroupScreen Host)
            {
                RxApp.MainThreadScheduler.Schedule(() => {
                    Host.InputRouter.NavigationStack.Clear();
                    Host.GroupEnabled = true;
                });
            }

            await Task.CompletedTask;
        }
    }

    public partial class PostazioneInputBase
    {
        public PostazioneInputBase(IScreen host) : base(host)
        {
            this.WhenActivated(d =>
            {
                
                // Osserva BindingT e CodiceTipoPostazione per calcolare la visibilità
                this.WhenAnyValue(
                    x => x.BindingT,
                    x => x.BindingT.CodiceTipoPostazione,
                    (bt, codice) => bt is not null && codice == 2) // Questa è la tua logica IsCassa
                .Subscribe(isCassa =>
                {
                    // Se è una cassa, il rientro è visibile (o invisibile, a seconda della tua logica)
                    RientroVisibile = isCassa;
                })
                .DisposeWith(d);


            });
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
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
        
        }

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        
    }
}
