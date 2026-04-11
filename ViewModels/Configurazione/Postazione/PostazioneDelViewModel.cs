using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneDelViewModel : PostazioneInputBase
    {
        private IPostazioneRepository Q;
        private readonly int _idDaModificare;

        public PostazioneDelViewModel(IScreen host, int idoperatore, IPostazioneRepository Repository) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Cancella Postazione";

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
            }
            SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT.ToDto()))
            {
                InfoLabel = "Errore Db eliminazione postazione";
                SetFocus(EscFocus);
                return;
            }
            OnBack(-100);
        }
    }
}
