using DTO.Repository;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class CassaPostazioneViewModel : BaseViewModel
    {
        private PostazioneMap cassaPostazione;
        public CassaPostazioneViewModel(IScreen host, PostazioneMap postazione) : base(host)
        {
            cassaPostazione = postazione;
            Titolo = $"Postazione {cassaPostazione.NomePostazione}";
        }

        protected async override Task OnEsc() => await HostScreen
                                                        .Router
                                                        .NavigateAndReset
                                                        .Execute(new MenuViewModel(HostScreen,
                                                            Locator.Current.GetService<IMenuRepository>()));

        protected override async Task OnLoading()
        {
            await Task.CompletedTask;
        }

        protected override async Task OnSaving()
        {
            await Task.CompletedTask;
        }
    }

    public partial class CassaPostazioneViewModel
    {
        private string _titolo = string.Empty;
        public string Titolo
        {
            get => _titolo;
            set => this.RaiseAndSetIfChanged(ref _titolo, value);
        }
        
    }
}
