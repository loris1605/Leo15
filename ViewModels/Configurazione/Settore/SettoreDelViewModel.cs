using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class SettoreDelViewModel : SettoreInputBase
    {
        private ISettoreRepository Q;

        private readonly int _idDaModificare;

        public SettoreDelViewModel(IScreen host, int idsettore, ISettoreRepository Repository) : base(host)
        {
            _idDaModificare = idsettore;
            Titolo = "Cancella Settore";
            Q = Repository;
            FieldsEnabled = false;
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

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT.ToDto()))
            {
                InfoLabel = "Errore Db eliminazione Settore";
                SetFocus(EscFocus);
                return;
            }
            OnBack(-100);
        }

    }
}
