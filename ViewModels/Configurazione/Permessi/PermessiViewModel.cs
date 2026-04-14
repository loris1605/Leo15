using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;
using System.Reactive.Concurrency;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class PermessiViewModel : OperatoreInputBase
    {
        private IOperatoreRepository Q;
        
        private readonly int _idDaModificare;

        public PermessiViewModel(IScreen host, int idoperatore, IOperatoreRepository Repository) : base(host)
        {
            _idDaModificare = idoperatore;
            Q = Repository;
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            var operatore = await Q.FirstOperatore(_idDaModificare, token);
            Titolo = "Permessi per l'operatore : " + operatore.NomeOperatore;
            var data = await Q.GetPermessi(_idDaModificare,token);

            DataSource = data.Select(dto => new PostazioneElencoMap(dto)).ToList();
            SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            var dtoSource = DataSource.Select(p => p.ToDto()).ToList();

            if (!await Q.SavePermessi(_idDaModificare, dtoSource, token))
            {
                InfoLabel = "Errore Db modifica permessi";
                SetFocus(EscFocus);
                return;
            }

            OnBack(_idDaModificare);
        }


    }

    public partial class PermessiViewModel
    {
        #region DataSource

        private IList<PostazioneElencoMap> _datasource = [];
        public IList<PostazioneElencoMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private PostazioneElencoMap _bindingT;
        public new PostazioneElencoMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        #endregion


    }
}
