using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class SettoreUpdViewModel : SettoreInputBase
    {
        private SettoreR Q { get; set; }
        private readonly int _idDaModificare;

        public SettoreUpdViewModel(IScreen host, int idoperatore) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Settore";
            FieldsEnabled = true;

            Q = Create<SettoreR>.Instance();
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
            TipoSettDataSource = await Q.LoadTipiSettore();
            BindingT = await Q.GetById(_idDaModificare);
            if (GetCodiceSettore == 0)
            {
                InfoLabel = "Errore: Settore non trovato nel database.";
                FieldsEnabled = false;
            }
            await OnFocus(EscFocus);
        }

        protected override async Task OnSaving()
        {
            InfoLabel = "";
            if (!await ValidaDati()) return;
            if (await Q.EsisteNomeUpd(BindingT))
            {
                InfoLabel = "Settore già registrato";
                return;
            }
            
            if (!await Q.Upd(BindingT))
            {
                InfoLabel = "Errore Db modifica Settore";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);
        }
    }
}
