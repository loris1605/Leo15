using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public interface ISociScreen : IGroupScreen
    {
        void AggiornaGrid(object model);
        

    }
    public partial class SociViewModel : BaseViewModel, ISociScreen
    {
        public RoutingState GroupRouter { get; } = new RoutingState();
        public RoutingState InputRouter { get; } = new RoutingState();
        public RoutingState Router => GroupRouter;

        public ReactiveCommand<Unit, Unit> EsciCommand { get; }
         

        public SociViewModel(IScreen host) : base(host)
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
            await GroupRouter.NavigateAndReset.Execute(new PersonGroupViewModel(this));

        }

        private async Task OnGoToMenu()
        {
            await HostScreen.Router.NavigateAndReset.Execute(new MenuViewModel(HostScreen));
        }


        public void AggiornaGrid(object model)
        {
            if (GroupRouter.GetCurrentViewModel() is IGroupViewModelBase groupVm)
            {
                if (model is int n)
                {
                    AggiornaGrid(n);
                    
                }
                else
                {
                    groupVm.CaricaByModel(model);
                }
                    // Passiamo l'ID al metodo di caricamento della lista
                    

                // Se hai un comando di ricarica nel GroupViewModel:
                // groupVm.LoadCommand.Execute().Subscribe();
            }
        }

        public void AggiornaGrid(int id)
        {
            if (GroupRouter.GetCurrentViewModel() is IGroupViewModelBase groupVm)
            {
                // Passiamo l'ID al metodo di caricamento della lista
                groupVm.CaricaDataSource(id);

                // Se hai un comando di ricarica nel GroupViewModel:
                // groupVm.LoadCommand.Execute().Subscribe();
            }
        }
    }

    public partial class SociViewModel
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
