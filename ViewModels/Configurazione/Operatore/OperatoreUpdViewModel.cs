using DTO.Repository;
using Models.Tables;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class OperatoreUpdViewModel : OperatoreInputBase
    {
        private IOperatoreRepository Q;
        private readonly int _idDaModificare;

        public OperatoreUpdViewModel(IScreen host, int idoperatore, IOperatoreRepository Repository) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Operatore";
            
            FieldsEnabled = true;
            
            Q = Repository;
        }

        protected override void OnFinalDestruction() => Q = null;
        

        protected override async Task OnLoading()
        {
            var data = await Q.FirstOperatore(_idDaModificare);

            BindingT = new BindableObjects.OperatoreMap(data);

            if (GetCodiceOperatore == 0)
            {
                InfoLabel = "Errore: Operatore non trovato nel database.";
                FieldsEnabled = false;
                SetFocus(NomeFocus);
                return;
            }
            NomeOperatoreEnabled = _idDaModificare != -1;
            if (NomeOperatoreEnabled)
            {
                SetFocus(NomeFocus);
            }
            else SetFocus(PasswordFocus);
        }

        protected override async Task OnSaving()
        {
            if (token.IsCancellationRequested) return;

            if (!ValidaDati()) return;

            if (await Q.EsisteNomeUpd(BindingT.ToDto()))
            {
                InfoLabel = "Operatore già registrato";
                return;
            }

            InfoLabel = "";

            if (!await Q.Upd(BindingT.ToDto()))
            {
                InfoLabel = "Errore Db modifica operatore";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);

        }
    }
}
