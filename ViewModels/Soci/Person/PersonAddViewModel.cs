using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using System.Reactive;
using System.Reactive.Linq;
using System.Diagnostics;
using SysNet.Converters;

namespace ViewModels
{
    public class PersonAddViewModel : PersonInputBase
    {

        private IPersonRepository Q;

        public PersonAddViewModel(IScreen host, IPersonRepository personRepository) : base(host)
        {
            Q = personRepository; //?? Locator.Current.GetService<IPersonRepository>();
            Titolo = "Aggiungi Nuovo Socio";
            FieldsVisibile = true;
            FieldsEnabled = true;

        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
        }


        protected override async Task OnLoading()
        {
            SetFocus(CognomeFocus);
            await Task.CompletedTask;
        }

        protected async override Task OnSaving()
        {
            if (!ValidaDati()) return;
            

            if (!int.TryParse(GetNumeroTessera, out int numeroTessera) || numeroTessera <= 0)
            {
                InfoLabel = "Numero Tessera non valido o uguale a zero";
                SetFocus(TesseraFocus);
                return;
            }

            if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera, token))
            {
                InfoLabel = "Tessera già in uso";
                SetFocus(TesseraFocus);
                return;
            }

            if (int.TryParse(GetNumeroSocio, out int numeroSocio))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroSocio <= 0) { }
                else
                {
                    if (await Q.EsisteNumeroSocio(BindingT.NumeroSocio, token))
                    {
                        InfoLabel = "Codice Socio già in uso";
                        SetFocus(SocioFocus);
                        IsLoading = false;
                        return;
                    }
                }

            }
            else
            {
                // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                // (In questo caso considerala come se fosse <= 0)
                InfoLabel = "Codice Socio non può essere zero";
                SetFocus(SocioFocus);
                return;
            }
            
            if (await EsisteAnagrafica())
            {
                InfoLabel = "Socio già registrato";
                SetFocus(SocioFocus);
                return;
            }

            InfoLabel = "Salvataggio in corso...";

            int newPersonId = await Q.AddPerson(BindingT.ToDto());

            if (newPersonId == -1)
            {
                InfoLabel = "Errore Db inserimento Socio";
                SetFocus(CognomeFocus);
                return;
            }

            await OnBack(newPersonId);
        }

        private async Task<bool> EsisteAnagrafica()
        {
            if (BindingT is null) return false;

            string srvcognome = BindingT.Cognome.PadRight(3);
            string srvnome = BindingT.Nome.PadRight(3);


            BindingT.CodiceUnivoco = string.Concat(
                                                srvcognome[..3],
                                                srvnome[..3],
                                                BindingT.Natoil.ToString());

            try
            {
                return await Q.EsisteCodiceUnivoco(BindingT.CodiceUnivoco, token);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operazione annullata dall'utente");
            }
            catch (Exception ex)
            {
                { Debug.WriteLine($"Esiste Anagrafica Error: {ex.Message}"); }
            }

            return false;



        }
    }
}
