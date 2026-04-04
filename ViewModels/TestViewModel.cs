using DTO.Repository;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        public TestViewModel(IScreen host) : base(host)
        {

        }

        protected override Task OnLoading()
        {
            return Task.CompletedTask;
        }
    }
}
