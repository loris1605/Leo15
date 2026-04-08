using DTO.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using SysNet.Converters;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class CodiceSocioDelViewModel : CodiceSocioInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        private readonly int _idRitorno;

        public CodiceSocioDelViewModel(IScreen host, 
                                       int idsocio, 
                                       int idperson,
                                       IPersonRepository personRepository = null) : base(host)
        {
            _idDaModificare = idsocio;
            _idRitorno = idperson;

            FieldsVisibile = false;
            FieldsEnabled = false;

            Q = personRepository ?? Locator.Current.GetService<IPersonRepository>();

            SetFocus(EscFocus);
            
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

                var data = await Q.FirstSocio(_idDaModificare, token);
                token.ThrowIfCancellationRequested();
                if (data == null)
                {
                    InfoLabel = "Errore: Socio non trovato nel database.";
                    FieldsEnabled = false;
                    
                }
                else
                {
                    BindingT = new PersonMap(data);
                    Titolo = "Elimina Codice Socio : " + GetNumeroSocio;
                    Titolo1 = "per " + GetNomeCognome;
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

            SetFocus(EscFocus);

        }
 
        protected async override Task OnSaving()
        {
            if (BindingT is null) return;

            if (!await Q.DelSocio(BindingT.ToDto(), token))
            {
                InfoLabel = "Errore Db eliminazione person";
                SetFocus(EscFocus);
                return;
            }
            
            await OnBack(_idRitorno);
        }
    }
}
