using DTO.Repository;
using ReactiveUI;
using SysNet;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class SettoreUpdViewModel : SettoreInputBase
    {
        private ISettoreRepository Q;
        private readonly int _idDaModificare;

        public SettoreUpdViewModel(IScreen host, int idoperatore, ISettoreRepository Repository) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Settore";
            FieldsEnabled = true;

            Q = Repository;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {

            await CaricaCombos();

            var data = await Q.FirstSettore(_idDaModificare);
            BindingT = new BindableObjects.SettoreMap(data);

            if (GetCodiceSettore == 0)
            {
                InfoLabel = "Errore: Settore non trovato nel database.";
                FieldsEnabled = false;
            }
            SetFocus(EscFocus);
        }

        private async Task CaricaCombos()
        {
            var data = await Q.LoadTipiSettore();
            TipoSettDataSource = data.Select(dto => new TipoSettoreMap(dto)).ToList();
            
        }

        protected override async Task OnSaving()
        {
            InfoLabel = "";
            if (!ValidaDati()) return;
            if (await Q.EsisteNomeUpd(BindingT.ToDto()))
            {
                InfoLabel = "Settore già registrato";
                return;
            }
            
            if (!await Q.Upd(BindingT.ToDto()))
            {
                InfoLabel = "Errore Db modifica Settore";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);
        }
    }
}
