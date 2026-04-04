using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class PostazioneUpdViewModel : PostazioneInputBase
    {
        private PostazioneR Q { get; set; }
        private readonly int _idDaModificare;

        public PostazioneUpdViewModel(IScreen host, int idoperatore) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Postazione";

            FieldsEnabled = true;

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
                await OnFocus(EscFocus);
                return;
            }
            await OnFocus(NomeFocus);
        }

        protected override async Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (await Q.EsisteNomeUpd(BindingT))
            {
                InfoLabel = "Operatore già registrato";
                return;
            }

            InfoLabel = "";

            if (!await Q.Upd(BindingT))
            {
                InfoLabel = "Errore Db modifica postazione";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);

        }
    }
}
