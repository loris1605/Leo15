using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using SysNet.Converters;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class CodiceSocioAddViewModel : CodiceSocioInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        
        public CodiceSocioAddViewModel(IScreen host, int idperson, IPersonRepository personRepository = null) : base(host)
        {
            _idDaModificare = idperson;

            Titolo = "Nuovo Codice Socio";
            
            FieldsVisibile = true;
            FieldsEnabled = true;

            Q = personRepository ?? Locator.Current.GetService<IPersonRepository>();

            SetFocus(NumeroSocioFocus);
           
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
        }

        protected override async Task OnLoading()
        {
            var token = _cts?.Token ?? CancellationToken.None;

            IsLoading = true;
            try
            {
                var dto = await Q.FirstPerson(_idDaModificare, token);
                token.ThrowIfCancellationRequested();
                if (dto == null)
                {
                    InfoLabel = "Errore: Socio non trovato nel database.";
                    FieldsEnabled = false;
                    SetFocus(EscFocus);
                }
                else
                {
                    BindingT = new PersonMap(dto);
                    Titolo1 = "per " + GetNomeCognome;
                    BindingT.NumeroSocio = string.Empty;
                    BindingT.NumeroTessera = string.Empty;
                    SetFocus(NumeroSocioFocus);
                }
       
                
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
            finally { IsLoading = false; }


        }
       

        private int idsocio;

        protected async override Task OnSaving()
        {
            IsLoading = true;
            var token = _cts?.Token ?? CancellationToken.None;

            if (BindingT is null)
                return;

            try
            {
                if (int.TryParse(GetNumeroSocio, out int numeroSocio))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroSocio <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroSocio(BindingT.NumeroSocio, token))
                        {
                            InfoLabel = "Codice Socio già in uso";
                            SetFocus(NumeroSocioFocus);
                            return;
                        }
                    }
                }
                else
                {
                    // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                    // (In questo caso considerala come se fosse <= 0)
                    InfoLabel = "Codice Socio non può essere zero";
                    SetFocus(NumeroSocioFocus);
                    return;
                }

                if (int.TryParse(GetNumeroTessera, out int numeroTessera))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroTessera <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera, token))
                        {
                            InfoLabel = "Tessera già in uso";
                            SetFocus(NumeroTesseraFocus);
                            return;
                        }
                    }

                }
                else
                {
                    // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                    // (In questo caso considerala come se fosse <= 0)
                    InfoLabel = "Numero Tessera non può essere zero";
                    SetFocus(NumeroTesseraFocus);
                    return;
                }

                idsocio = await Q.AddCodiceSocio(BindingT.ToDto(), token);

                if (idsocio == -1)
                {
                    SetFocus(NumeroSocioFocus);
                }
                else
                {
                    await OnBack(_idDaModificare);
                }


            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operazione annullata dall'utente");
                return;
            }
            catch (Exception ex)
            {
                InfoLabel = $"Errore durante il salvataggio: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                
            }
        }
    }
}
