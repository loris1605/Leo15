using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneUpdViewModel : PostazioneInputBase
    {
        private IPostazioneRepository Q;
        private readonly int _idDaModificare;

        public PostazioneUpdViewModel(IScreen host, int idoperatore, IPostazioneRepository Repository) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Postazione";

            FieldsEnabled = true;

            Q = Repository;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {

            var dataTipoPostazione = await Q.LoadTipiPostazione();
            TipoPostDataSource = dataTipoPostazione.Select(dto => new TipoPostazioneMap(dto)).ToList();

            var dataTipoRientro = await Q.LoadTipiRientro();
            TipoRientroDataSource = dataTipoRientro.Select(dto => new TipoRientroMap(dto)).ToList();

            var data = await Q.FirstPostazione(_idDaModificare);
            BindingT = new BindableObjects.PostazioneMap(data);

            
            if (GetCodicePostazione == 0)
            {
                InfoLabel = "Errore: Postazione non trovata nel database.";
                FieldsEnabled = false;
                SetFocus(EscFocus);
                return;
            }
            SetFocus(NomeFocus);
        }



        protected override async Task OnSaving()
        {
            if (!ValidaDati()) return;

            if (await Q.EsisteNomeUpd(BindingT.ToDto()))
            {
                InfoLabel = "Operatore già registrato";
                return;
            }

            InfoLabel = "";

            if (!await Q.Upd(BindingT.ToDto()))
            {
                InfoLabel = "Errore Db modifica postazione";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);

        }
    }
}
