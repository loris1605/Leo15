using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class TariffaUpdViewModel : TariffaInputBase
    {
        private ITariffaRepository Q;
        private readonly int _idDaModificare;

        public TariffaUpdViewModel(IScreen host, int idoperatore, ITariffaRepository Repository) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Tariffa";
            FieldsEnabled = true;

            Q = Repository;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            var data = await Q.FirstTariffa(_idDaModificare);

            BindingT = new BindableObjects.TariffaMap(data);

            if (GetCodiceTariffa == 0)
            {
                InfoLabel = "Errore: Tariffa non trovata nel database.";
                FieldsEnabled = false;
            }
            SetFocus(EscFocus);
        }

        protected override async Task OnSaving()
        {
            InfoLabel = "";
            if (!ValidaDati()) return;

            var input = BindingT.ToDTO();
            if (await Q.EsisteNomeUpd(input))
            {
                InfoLabel = "Tariffa già registrata";
                return;
            }

            if (!await Q.Upd(input))
            {
                InfoLabel = "Errore Db modifica Tariffa";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);
        }
    }
}
