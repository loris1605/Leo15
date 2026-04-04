using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class PostazioneDelViewModel : PostazioneInputBase
    {
        private PostazioneR Q { get; set; }
        private readonly int _idDaModificare;

        public PostazioneDelViewModel(IScreen host, int idoperatore) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Cancella Postazione";

            Q = Create<PostazioneR>.Instance();
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
            TipoPostDataSource = await Q.LoadTipiPostazione();
            TipoRientroDataSource = await Q.LoadTipiRientro();
            BindingT = await Q.GetById(_idDaModificare);
            if (GetCodicePostazione == 0)
            {
                InfoLabel = "Errore: Postazione non trovata nel database.";
                FieldsEnabled = false;
            }
            await OnFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT))
            {
                InfoLabel = "Errore Db eliminazione postazione";
                await OnEscFocus();
                return;
            }
            OnBack(-100);
        }
    }
}
