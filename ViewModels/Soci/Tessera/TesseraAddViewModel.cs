using DTO.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class TesseraAddViewModel : TesseraInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        private readonly int _idRitorno;

        private int idtessera;

        public TesseraAddViewModel(IScreen host, 
                                   int idperson, 
                                   int idsocio,
                                   IPersonRepository personRepository = null) : base(host)
        {
            _idRitorno = idperson;
            _idDaModificare = idsocio;

            Titolo = "Nuova Tessera";
 
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = personRepository ?? Locator.Current.GetService<IPersonRepository>();

        }

        protected override void OnFinalDestruction()
        {
            Q = null;
        }

        protected override async Task OnLoading()
        {
            IsLoading = true;
            var token = _cts?.Token ?? CancellationToken.None;

            try
            {
                var dto = await Q.FirstSocio(_idDaModificare, token);
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
                    Titolo = "Nuova Tessera per " + GetNomeCognome;
                    Titolo1 = "Numero Socio : " + GetNumeroSocio;
                    BindingT.NumeroTessera = string.Empty;
                    await OnNumeroTesseraFocus();
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

        protected async override Task OnSaving()
        {
            IsLoading = true;
            var token = _cts?.Token ?? CancellationToken.None;

            try
            {
                if (BindingT is null) { return; }

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

                idtessera = await Q.AddTessera(BindingT.ToDto(), token);

                if (idtessera == -1)
                {
                    InfoLabel = "Errore durante il salvataggio. Verificare i dati e riprovare.";
                    await OnNumeroTesseraFocus();
                }
                else
                {
                    await OnBack(_idRitorno);
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
