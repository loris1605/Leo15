using DTO.Repository;
using ReactiveUI;
using Splat;
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

        public ConfigurazioneViewModel(IScreen host) : base(host)
        {
            

            //this.WhenActivated(d =>
            //{
                
            //});
            
        }

        protected override void OnFinalDestruction()
        {
            GroupRouter.NavigationStack.Clear();
            InputRouter.NavigationStack.Clear();
        }

        protected override async Task OnLoading()
        {
            await GroupRouter.NavigateAndReset.Execute(new OperatoreGroupViewModel(this, 
                                                       Locator.Current.GetService<IOperatoreRepository>()));
            await Task.CompletedTask;
            
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

        protected override Task OnSaving() => Task.CompletedTask;
        
        protected async override Task OnEsc() => await HostScreen
                                                        .Router
                                                        .NavigateAndReset
                                                        .Execute(new MenuViewModel(HostScreen,
                                                            Locator.Current.GetService<IMenuRepository>()));

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
