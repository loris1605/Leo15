using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class TariffaUpdViewModel : TariffaInputBase
    {
        private TariffaR Q { get; set; }
        private readonly int _idDaModificare;

        public TariffaUpdViewModel(IScreen host, int idoperatore) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Tariffa";
            FieldsEnabled = true;

            Q = Create<TariffaR>.Instance();
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

        protected override async Task OnSaving()
        {
            InfoLabel = "";
            if (!await ValidaDati()) return;
            if (await Q.EsisteNomeUpd(BindingT))
            {
                InfoLabel = "Tariffa già registrata";
                return;
            }

            if (!await Q.Upd(BindingT))
            {
                InfoLabel = "Errore Db modifica Tariffa";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);
        }
    }
}
