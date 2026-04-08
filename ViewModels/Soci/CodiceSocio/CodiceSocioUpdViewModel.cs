using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using SysNet.Converters;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class CodiceSocioUpdViewModel : CodiceSocioInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        private readonly int _idRitorno;

        public CodiceSocioUpdViewModel(IScreen host,
                                       int idsocio,
                                       int idperson,
                                       IPersonRepository personRepository = null) : base(host)
        {
            _idDaModificare = idsocio;
            _idRitorno = idperson;
            
            FieldsEnabled = true;
            FieldsVisibile = true;
            FieldVisibile = false;

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
                var data = await Q.FirstSocio(_idDaModificare, token);
                token.ThrowIfCancellationRequested();

                if (data == null)
                {
                    InfoLabel = "Errore: Socio non trovato nel database.";
                    FieldsEnabled = false;
                    SetFocus(EscFocus);
                }
                else
                {
                    BindingT = new PersonMap(data);
                    Titolo = "Modifica Codice Socio per ";
                    Titolo1 = "per " + GetNomeCognome;
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

        protected override async Task OnSaving()
        {
            IsLoading = true;

            var token = _cts?.Token ?? CancellationToken.None;

            try
            {
                if (BindingT is null)
                    return;

                if (int.TryParse(GetNumeroSocio, out int numeroSocio))
                {
                    // 2. Se la conversione riesce, controlliamo il valore
                    if (numeroSocio <= 0) { }
                    else
                    {
                        if (await Q.EsisteNumeroSocioUpd(BindingT.ToDto(), token))
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

                InfoLabel = "";

                if (!await Q.UpdSocio(BindingT.ToDto(), token))
                {
                    InfoLabel = "Errore Db modifica person";
                    SetFocus(NumeroSocioFocus);
                    return;
                }

                await OnBack(_idRitorno);

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


    }
}
