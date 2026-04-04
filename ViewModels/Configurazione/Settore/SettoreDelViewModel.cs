using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class SettoreDelViewModel : SettoreInputBase
    {
        private SettoreR Q { get; set; }

        private readonly int _idDaModificare;

        public SettoreDelViewModel(IScreen host, int idsettore) : base(host)
        {
            _idDaModificare = idsettore;
            Titolo = "Cancella Settore";
            Q = Create<SettoreR>.Instance();
            FieldsEnabled = false;
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

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT))
            {
                InfoLabel = "Errore Db eliminazione Settore";
                await OnEscFocus();
                return;
            }
            OnBack(-100);
        }

    }
}
