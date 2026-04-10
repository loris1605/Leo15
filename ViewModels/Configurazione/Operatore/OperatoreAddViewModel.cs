using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class OperatoreAddViewModel : OperatoreInputBase
    {
        private IOperatoreRepository Q;

        public OperatoreAddViewModel(IScreen host, IOperatoreRepository Repository) : base(host)
        {
            Titolo = "Aggiungi Nuovo Operatore";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository;
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
        }

        protected override async Task OnLoading()
        {
            SetFocus(NomeFocus);
            await Task.CompletedTask;
        }

        protected async override Task OnSaving()
        {
            if (!ValidaDati()) return;

            if (await Q.EsisteNome(BindingT.ToDto()))
            {
                InfoLabel = "Operatore già registrato";
                SetFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            BindingT.CodicePerson = -2;
            
            int newOperatoreId = await Q.Add(BindingT.ToDto());

            if (newOperatoreId == -1)
            {
                InfoLabel = "Errore Db inserimento Operatore";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(newOperatoreId);
        }
    }
}
