using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class PersonDelViewModel : PersonInputBase
    {
        private PersonR Q { get; set; }
        private readonly int _idDaModificare;
        
        public PersonDelViewModel(IScreen host, int idperson) : base(host)
        {
            _idDaModificare = idperson;
            Titolo = "Elimina Socio";
            FieldsEnabled = false;
            FieldsVisibile = false; ;
            Q = Create<PersonR>.Instance();
            SaveCommand = ReactiveCommand.CreateFromTask(OnSaving);
            
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
            DataSource = null;
        }

        protected override async Task OnLoading()
        {
            BindingT = await Q.FirstPerson(_idDaModificare);
            if (BindingT == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
            }
            
            await OnFocus(EscFocus);
        }

        

        protected async override Task OnSaving()
        {
            
            if (!await Q.Del(BindingT))
            {
                InfoLabel = "Errore Db eliminazione person";
                await OnEscFocus();
                return;
            }
            OnBack(-100);
        }

        
    }
}
