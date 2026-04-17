using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CassaPostazioneViewModel : BaseViewModel
    {
        private int cassaPostazioneId;
        public CassaPostazioneViewModel(IScreen host, int postazioneId) : base(host)
        {
            cassaPostazioneId = postazioneId;
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
