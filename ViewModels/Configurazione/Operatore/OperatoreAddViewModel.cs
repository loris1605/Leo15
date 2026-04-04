using Models.Entity;
using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class OperatoreAddViewModel : OperatoreInputBase
    {
        private OperatoreR Q { get; set; }

        public OperatoreAddViewModel(IScreen host) : base(host)
        {
            Titolo = "Aggiungi Nuovo Operatore";
            FieldsVisibile = true;
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
            await OnFocus(NomeFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (await Q.EsisteNome(BindingT))
            {
                InfoLabel = "Operatore già registrato";
                await OnFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            CodicePerson = -2;
            
            int newOperatoreId = await Q.Add<OperatoreMap>(BindingT);

            if (newOperatoreId == -1)
            {
                InfoLabel = "Errore Db inserimento Operatore";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(newOperatoreId);
        }
    }
}
