using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class TariffaAddViewModel : TariffaInputBase
    {
        private ITariffaRepository Q;

        public TariffaAddViewModel(IScreen host, ITariffaRepository Repository) : base(host)
        {
            Titolo = "Aggiungi Nuova Tariffa";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            SetFocus(NomeFocus);
            await Task.CompletedTask;
        }

        protected async override Task OnSaving()
        {
            if (!ValidaDati()) return;

            if (await Q.EsisteNome(BindingT.ToDTO()))
            {
                InfoLabel = "Tariffa già registrata";
                SetFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            int newTariffaId = await Q.Add(BindingT.ToDTO());

            if (newTariffaId == -1)
            {
                InfoLabel = "Errore Db inserimento Tariffa";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(newTariffaId);
        }
    }
}
