using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class OperatoreDelViewModel : OperatoreInputBase
    {
        private IOperatoreRepository Q;
        private readonly int _idDaModificare;

        public OperatoreDelViewModel(IScreen host, int idoperatore, IOperatoreRepository Repository) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Cancella Operatore";

            Q = Repository;
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
var data = await Q.FirstOperatore(_idDaModificare);
            BindingT = new BindableObjects.OperatoreMap(data);
            if (GetCodiceOperatore == 0)
            {
                InfoLabel = "Errore: Operatore non trovato nel database.";
                FieldsEnabled = false;
            }
            SetFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT.ToDto()))
            {
                InfoLabel = "Errore Db eliminazione operatore";
                SetFocus(EscFocus);
                return;
            }
            OnBack(-100);
        }
    }
}
