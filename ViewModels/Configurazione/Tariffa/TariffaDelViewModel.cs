using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class TariffaDelViewModel : TariffaInputBase
    {
        private TariffaR Q { get; set; }

        private readonly int _idDaModificare;

        public TariffaDelViewModel(IScreen host, int idsettore) : base(host)
        {
            _idDaModificare = idsettore;
            Titolo = "Cancella Settore";
            Q = Create<TariffaR>.Instance();
            FieldsEnabled = false;
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
            
        }

        protected override async Task OnLoading()
        {
            BindingT = await Q.GetById(_idDaModificare);
            if (GetCodiceTariffa == 0)
            {
                InfoLabel = "Errore: Tariffa non trovata nel database.";
                FieldsEnabled = false;
            }
            await OnFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT))
            {
                InfoLabel = "Errore Db eliminazione Tariffa";
                await OnEscFocus();
                return;
            }
            OnBack(-100);
        }
    }
}
