using Models.Mappers;
using Models.Repository;
using Models.Tables;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class OperatoreUpdViewModel : OperatoreInputBase
    {
        private OperatoreR Q { get; set; }
        private readonly int _idDaModificare;

        public OperatoreUpdViewModel(IScreen host, int idoperatore) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Modifica Operatore";
            
            FieldsEnabled = true;
            
            Q = Create<OperatoreR>.Instance();
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
            BindingT = await Q.GetById(_idDaModificare);
            if (GetCodiceOperatore == 0)
            {
                InfoLabel = "Errore: Operatore non trovato nel database.";
                FieldsEnabled = false;
                await OnFocus(NomeFocus);
                return;
            }
            NomeOperatoreEnabled = _idDaModificare != -1;
            if (NomeOperatoreEnabled)
            {
                await OnFocus(NomeFocus);
            }
            else await OnFocus(PasswordFocus);
        }

        protected override async Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (await Q.EsisteNomeUpd(BindingT))
            {
                InfoLabel = "Operatore già registrato";
                return;
            }

            InfoLabel = "";

            if (!await Q.Upd(BindingT))
            {
                InfoLabel = "Errore Db modifica operatore";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(_idDaModificare);

        }
    }
}
