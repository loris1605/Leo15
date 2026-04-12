using DTO.Repository;
using ReactiveUI;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class SettoreAddViewModel : SettoreInputBase
    {

        private ISettoreRepository Q;

        public SettoreAddViewModel(IScreen host, ISettoreRepository Repository) : base(host)
        {
            Titolo = "Aggiungi Nuovo Settore";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            await CaricaCombos();
            SetFocus(NomeFocus);
        }

        private async Task CaricaCombos()
        {
            var data = await Q.LoadTipiSettore();
            TipoSettDataSource = data.Select(dto => new TipoSettoreMap(dto)).ToList();
            CodiceTipoSettore = TipoSettDataSource[0].Id;
        }

        protected async override Task OnSaving()
        {
            if (!ValidaDati()) return;

            if (await Q.EsisteNome(BindingT.ToDto()))
            {
                InfoLabel = "Settore già registrato";
                SetFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            int newSettoreId = await Q.Add(BindingT.ToDto());

            if (newSettoreId == -1)
            {
                InfoLabel = "Errore Db inserimento Settore";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(newSettoreId);
        }
    }
}
