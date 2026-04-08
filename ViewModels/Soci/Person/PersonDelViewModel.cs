using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using SysNet;
using System.Diagnostics;
using System.Reactive;

namespace ViewModels
{
    public class PersonDelViewModel : PersonInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        
        public PersonDelViewModel(IScreen host, int idperson, IPersonRepository personRepository = null) : base(host)
        {
            _idDaModificare = idperson;
            Q = personRepository; // ?? Locator.Current.GetService<IPersonRepository>();
            Titolo = "Elimina Socio";
            FieldsEnabled = false;
            FieldsVisibile = false; ;
                       
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
        }

        protected override async Task OnLoading()
        {
            var data = await Q.FirstPerson(_idDaModificare, token);
            BindingT = new BindableObjects.PersonMap(data);

            if (BindingT == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
            }
            SetFocus(EscFocus);
        }
        

        protected async override Task OnSaving()
        {
            if (!await Q.Del(BindingT.ToDto(), token))
            {
                InfoLabel = "Errore Db eliminazione person";
                await TriggerInteraction(EscFocus, Unit.Default);
                IsLoading = false;
                return;
            }
            await OnBack(-100);
            
        }

        
    }
}
