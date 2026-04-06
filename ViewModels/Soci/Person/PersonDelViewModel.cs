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
            SaveCommand = ReactiveCommand.CreateFromTask(OnSaving);
            
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
        }

        protected override async Task OnLoading()
        {
            var token = CancellationToken.None;
            try
            {
                var data = await Q.FirstPerson(_idDaModificare, token);
                BindingT = new BindableObjects.PersonMap(data);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operazione annullata dall'utente");
                return;
            }
            catch (Exception ex)
            {
                { Debug.WriteLine($"OnLoading Error: {ex.Message}"); }
            }


            if (BindingT == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
            }

            SetFocus(EscFocus);
        }

        

        protected async override Task OnSaving()
        {
            var token = CancellationToken.None;

            try
            {
                if (!await Q.Del(BindingT.ToDto(), token))
                {
                    InfoLabel = "Errore Db eliminazione person";
                    await TriggerInteraction(EscFocus, Unit.Default);
                    return;
                }
                OnBack(-100);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operazione annullata dall'utente");
                return;
            }
            catch (Exception ex)
            {
                { Debug.WriteLine($"Del Person Error: {ex.Message}"); }
            }
           
        }

        
    }
}
