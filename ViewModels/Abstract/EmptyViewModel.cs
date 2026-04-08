using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EmptyViewModel : BaseViewModel
    {
        protected override Task OnLoading()
        {
            return Task.CompletedTask;
        }

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }

        protected override Task OnEsc() => Task.CompletedTask;
        

        public EmptyViewModel(IScreen host) : base(host) { }
    }
}
