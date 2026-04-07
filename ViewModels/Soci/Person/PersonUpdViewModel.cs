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
            IsLoading = true;
            var token = _cts?.Token ?? CancellationToken.None;

            try
            {
                // Passa il token al repository
                var data = await Q.FirstPerson(_idDaModificare, token);
                BindingT = new BindableObjects.PersonMap(data);

                if (BindingT.Id == 0)
                {
                    InfoLabel = "Errore: Socio non trovato.";
                    FieldsEnabled = false;
                }
                IsLoading = false;
                SetFocus(CognomeFocus);
            }
            catch (OperationCanceledException)
            {
                InfoLabel = "Errore: Operazione fermata dall'utente.";
            }
            catch (Exception ex)
            {
                { Debug.WriteLine($"OnLoading Error: {ex.Message}"); }
            }
            finally { IsLoading = false; }

        }

        protected override async Task OnSaving()
        {
            IsLoading = true;
            var token = _cts?.Token ?? CancellationToken.None;

            if (!await ValidaDati())
            {
                IsLoading = false;
                return;
            }

            if (await EsisteAnagraficaUpd(token))
            {
                InfoLabel = "Socio già registrato";
                IsLoading = false;
                return;
            }

            InfoLabel = "Updating Database...";

            try
            {
                if (!await Q.Upd(BindingT.ToDto(), token))
                {
                    InfoLabel = "Errore Db modifica person";
                    SetFocus(CognomeFocus);
                    IsLoading = false;
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                InfoLabel = "Person Upd annullata dall'utente";

            }
            catch (Exception ex)
            {
                InfoLabel = $"Person Upd Error: {ex.Message}";
            }
            finally { IsLoading = false; }

            await OnBack(_idDaModificare);
            
        }

        

        private async Task<bool> EsisteAnagraficaUpd(CancellationToken token)
        {
            string srvcognome = (GetCognome ?? "").PadRight(3);
            string srvnome = (GetNome ?? "").PadRight(3);

            BindingT.CodiceUnivoco = string.Concat(
                                    srvcognome[..3],
                                    srvnome[..3],
                                    BindingT.Natoil.ToString());

            try
            {
                return await Q.EsisteCodiceUnivoco(CodiceUnivoco, BindingT.Id);
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
