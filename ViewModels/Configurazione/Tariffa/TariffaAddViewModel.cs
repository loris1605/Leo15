using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class TariffaAddViewModel : TariffaInputBase
    {
        private TariffaR Q { get; set; }

        public TariffaAddViewModel(IScreen host) : base(host)
        {
            Titolo = "Aggiungi Nuova Tariffa";
            FieldsVisibile = true;
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
            await OnFocus(NomeFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (await Q.EsisteNome(BindingT))
            {
                InfoLabel = "Tariffa già registrata";
                await OnFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            int newTariffaId = await Q.Add(BindingT);

            if (newTariffaId == -1)
            {
                InfoLabel = "Errore Db inserimento Tariffa";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(newTariffaId);
        }
    }
}
