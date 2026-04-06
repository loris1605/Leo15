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

        public PersonAddViewModel(IScreen host, IPersonRepository personRepository = null) : base(host)
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
            var ct = CancellationToken.None;

            if (!await ValidaDati()) return;

            if (!int.TryParse(GetNumeroTessera, out int numeroTessera) || numeroTessera <= 0)
            {
                InfoLabel = "Numero Tessera non valido o uguale a zero";
                SetFocus(TesseraFocus);
                return;
            }

            try
            {
                if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera, ct))
                {
                    InfoLabel = "Tessera già in uso";
                    SetFocus(TesseraFocus);
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operazione annullata dall'utente");
                return;
            }
            catch (Exception ex)
            {
                { Debug.WriteLine($"OnSaving Error: {ex.Message}"); }
            }



            if (int.TryParse(GetNumeroSocio, out int numeroSocio))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroSocio <= 0) { }
                else
                {
                    try
                    {
                        if (await Q.EsisteNumeroSocio(BindingT.NumeroSocio, ct))
                        {
                            InfoLabel = "Codice Socio già in uso";
                            SetFocus(SocioFocus);
                            return;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine("Operazione annullata dall'utente");
                        return;
                    }
                    catch (Exception ex)
                    {
                        { Debug.WriteLine($"Esiste Numero Socio Error: {ex.Message}"); }
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

            
            if (await EsisteAnagrafica(ct))
            {
                InfoLabel = "Socio già registrato";
                SetFocus(SocioFocus);
                return;
            }

            InfoLabel = "Salvataggio in corso...";

            int newPersonId = await Q.Add(BindingT.ToDto(), ct);

            if (newPersonId == -1)
            {
                InfoLabel = "Errore Db inserimento Socio";
                SetFocus(CognomeFocus);
                return;
            }

            OnBack(newPersonId);
        }

        private async Task<bool> EsisteAnagrafica(CancellationToken ct)
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
                return await Q.EsisteCodiceUnivoco(BindingT.CodiceUnivoco, ct);
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
