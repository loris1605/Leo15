using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class OperatoreDelViewModel : OperatoreInputBase
    {
        private OperatoreR Q { get; set; }
        private readonly int _idDaModificare;

        public OperatoreDelViewModel(IScreen host, int idoperatore) : base(host)
        {
            _idDaModificare = idoperatore;

            Titolo = "Cancella Operatore";
            
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
            }
            await OnFocus(EscFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT))
            {
                InfoLabel = "Errore Db eliminazione operatore";
                await OnEscFocus();
                return;
            }
            OnBack(-100);
        }
    }
}
