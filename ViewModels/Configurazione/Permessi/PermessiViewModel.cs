using Models.Entity;
using Models.Repository;
using ReactiveUI;
using SysNet;
using System.Reactive.Concurrency;

namespace ViewModels
{
    public partial class PermessiViewModel : InputViewModel
    {
        private OperatoreR OperatoreQ { get; set; }
        private PermessoR Q { get; set; }
        private readonly int _idDaModificare;

        public PermessiViewModel(IScreen host, int idoperatore) : base(host)
        {
            Q = Create<PermessoR>.Instance();
            OperatoreQ = Create<OperatoreR>.Instance();
            _idDaModificare = idoperatore;
           
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
            OperatoreQ?.Dispose();
            OperatoreQ = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            var operatore = await OperatoreQ.GetById(_idDaModificare);
            Titolo = "Permessi per l'operatore : " + operatore.NomeOperatore;
            DataSource = await Q.GetPostazioniSenzaPermesso(_idDaModificare);
        }

        protected override Task OnSaving() => Task.CompletedTask;
        

        public void OnBackEsc()
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

    public partial class PermessiViewModel
    {
        #region DataSource

        private IList<PostazioneElencoMap> _datasource = [];
        public IList<PostazioneElencoMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private PostazioneElencoMap _bindingT;
        public PostazioneElencoMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        #endregion


    }
}
