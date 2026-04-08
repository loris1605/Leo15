using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using System.Diagnostics;
using System.Reactive;

namespace ViewModels
{
    public class PersonUpdViewModel : PersonInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        

        public PersonUpdViewModel(IScreen host, int idperson, IPersonRepository personRepository = null) : base(host)
        {
            _idDaModificare = idperson;
            Q = personRepository; // ?? Locator.Current.GetService<IPersonRepository>();
            Titolo = "Modifica Socio";
            FieldsEnabled = true;
            FieldsVisibile = false;
                        
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

            if (BindingT.Id == 0)
            {
                InfoLabel = "Errore: Socio non trovato.";
                FieldsEnabled = false;
            }
            SetFocus(CognomeFocus);

        }

        protected override async Task OnSaving()
        {
            if (!await ValidaDati()) return;
           

            if (await EsisteAnagraficaUpd())
            {
                InfoLabel = "Socio già registrato";
                return;
            }

            InfoLabel = "Updating Database...";

            if (!await Q.Upd(BindingT.ToDto(), token))
            {
                InfoLabel = "Errore Db modifica person";
                SetFocus(CognomeFocus);
                return;
            }

            await OnBack(_idDaModificare);
            
        }

        

        private async Task<bool> EsisteAnagraficaUpd()
        {
            string srvcognome = (GetCognome ?? "").PadRight(3);
            string srvnome = (GetNome ?? "").PadRight(3);

            BindingT.CodiceUnivoco = string.Concat(
                                    srvcognome[..3],
                                    srvnome[..3],
                                    BindingT.Natoil.ToString());

            try
            {
                return await Q.EsisteCodiceUnivoco(CodiceUnivoco, BindingT.Id, token);
            }
            catch (OperationCanceledException)
            {
                InfoLabel = "Operazione annullata dall'utente";
                
            }
            catch (Exception ex)
            {
                InfoLabel = $"Esiste Anagrafica Error: {ex.Message}";
            }

            return false;
         

        }
    }
}
