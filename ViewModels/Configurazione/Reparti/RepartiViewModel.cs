using DTO.Repository;
using Models.Tables;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class RepartiViewModel : PostazioneInputBase
    {
        private IPostazioneRepository Q;

        private readonly int _idDaModificare;

        public RepartiViewModel(IScreen host, int idsettore, IPostazioneRepository Repository) : base(host)
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
            var postazione = await Q.FirstPostazione(_idDaModificare, token);
            Titolo = "Reparti per la postazione : " + postazione.NomePostazione;
            var data = await Q.GetReparti(_idDaModificare, token);
            
            DataSource = data.Select(dto => new SettoreElencoMap(dto)).ToList();
            SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            var dtoSource = DataSource.Select(p => p.ToDto()).ToList();

            if (!await Q.SaveReparti(_idDaModificare, dtoSource, token))
            {
                InfoLabel = "Errore Db modifica reparti";
                SetFocus(EscFocus);
                return;
            }

            OnBack(_idDaModificare);
        }

    }

    public partial class RepartiViewModel
    {
        #region DataSource

        private IList<SettoreElencoMap> _datasource = [];
        public IList<SettoreElencoMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private SettoreElencoMap _bindingT;
        public new SettoreElencoMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        #endregion


    }
}
