using DTO.Repository;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class ListiniViewModel : SettoreInputBase
    {
        private ISettoreRepository Q;

        private readonly int _idDaModificare;

        public ListiniViewModel(IScreen host, int idsettore, ISettoreRepository Repository) : base(host)
        {
            _idDaModificare = idsettore;
            Q = Repository;
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            var settore = await Q.FirstSettore(_idDaModificare, token);
            Titolo = "Listini per il settore : " + settore.NomeSettore;
            var data = await Q.GetListini(_idDaModificare, token);
            DataSource = data.Select(dto => new TariffaMap(dto)).ToList();
            SetFocus(EscFocus);
        }
    } 

    public partial class ListiniViewModel
    {
        #region DataSource
        
        private IList<TariffaMap> _datasource = [];
        public IList<TariffaMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private TariffaMap _bindingT;
        public new TariffaMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        #endregion
    }
}
