using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    
    public interface IGroupScreen : IScreen
    {
        RoutingState GroupRouter { get; }
        RoutingState InputRouter { get; }
        bool GroupEnabled { get; set; }

        void AggiornaGridByInt(int id);
    }


    public partial class ConfigurazioneViewModel : BaseViewModel, IGroupScreen
    {
        public RoutingState GroupRouter { get; } = new RoutingState();
        public RoutingState InputRouter { get; } = new RoutingState();
        public RoutingState Router => GroupRouter;

        public ReactiveCommand<Unit, Unit> EsciCommand { get; }

        public ConfigurazioneViewModel(IScreen host) : base(host)
        {
            EsciCommand = ReactiveCommand.CreateFromTask(OnGoToMenu);

            this.WhenActivated(d =>
            {
                EsciCommand.DisposeWith(d);
            });
            
        }

        protected override void OnFinalDestruction()
        {
            GroupRouter.NavigationStack.Clear();
            InputRouter.NavigationStack.Clear();
        }

        protected override async Task OnLoading()
        {
            //await GroupRouter.NavigateAndReset.Execute(new OperatoreGroupViewModel(this));

            
        }
         
        public void AggiornaGridByInt(int id)
        {
            if (GroupRouter.GetCurrentViewModel() is IGroupViewModelBase groupVm)
            {
                // Passiamo l'ID al metodo di caricamento della lista
                groupVm.CaricaDataSource(id);

                // Se hai un comando di ricarica nel GroupViewModel:
                // groupVm.LoadCommand.Execute().Subscribe();
            }
        }

        private async Task OnGoToMenu()
        {
            await HostScreen.Router.NavigateAndReset.Execute(new MenuViewModel(HostScreen));
        }
    }

    public partial class ConfigurazioneViewModel
    {
        #region GroupEnabled

        private bool _groupenabled = true;
        public bool GroupEnabled
        {
            get => _groupenabled;
            set => this.RaiseAndSetIfChanged(ref _groupenabled, value);
        }

        #endregion
    }
}
