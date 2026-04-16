using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public partial class CassaViewModel : BaseViewModel
    {
        public RoutingState CassaRouter { get; } = new RoutingState();
        private int _cassaPostazioneId;


        public CassaViewModel(IScreen host, int cassaPostazioneId) : base(host)
        {
            _cassaPostazioneId = cassaPostazioneId;
        }

        protected override Task OnEsc()
        {
            throw new NotImplementedException();
        }

        protected override Task OnLoading()
        {
            throw new NotImplementedException();
        }

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }
    }
}
