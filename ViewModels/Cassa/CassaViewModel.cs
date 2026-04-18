using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class CassaViewModel : BaseViewModel
    {
        public RoutingState CassaRouter { get; } = new RoutingState();
        public RoutingState SettingsRouter { get; } = new RoutingState();

        private PostazioneMap _cassaPostazione;


        public CassaViewModel(IScreen host, PostazioneMap cassaPostazione) : base(host)
        {
            _cassaPostazione = cassaPostazione;
        }

        protected override Task OnEsc()
        {
            throw new NotImplementedException();
        }

        protected override async Task OnLoading()
        {
            await CassaRouter.NavigateAndReset.Execute(new CassaPostazioneViewModel(HostScreen, 
                                                                                   _cassaPostazione));
        }

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }
    }
}
