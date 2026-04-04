using Models.Repository;
using ReactiveUI;
using SysNet;
using SysNet.Converters;
using System.Reactive;
using System.Reactive.Linq;

namespace ViewModels
{
    public class CodiceSocioAddViewModel : CodiceSocioInputBase
    {
        private PersonR Q { get; set; }
        private readonly int _idDaModificare;
        
        public CodiceSocioAddViewModel(IScreen host, int idperson) : base(host)
        {
            _idDaModificare = idperson;

            Titolo = "Nuovo Codice Socio";
            
            FieldsVisibile = true;
            FieldsEnabled = true;

            Q = Create<PersonR>.Instance();

            OnNumeroSocioFocus().FireAndForget();

        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
            BindingT = await Q.FirstPerson(_idDaModificare);
            if (BindingT == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
            }
            Titolo1 = "per " + GetNomeCognome;
            NumeroSocio = string.Empty;
            NumeroTessera = string.Empty;
            await OnNumeroSocioFocus();
        }
       

        private int idsocio;

        protected async override Task OnSaving()
        {

            if (BindingT is null)
                return;

            if (int.TryParse(GetNumeroSocio, out int numeroSocio))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroSocio <= 0) { }
                else
                {
                    if (await Q.EsisteNumeroSocio(BindingT.NumeroSocio))
                    {
                        InfoLabel = "Codice Socio già in uso";
                        await NumeroSocioFocus.Handle(Unit.Default);
                        return;
                    }
                }
            }
            else
            {
                // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                // (In questo caso considerala come se fosse <= 0)
                InfoLabel = "Codice Socio non può essere zero";
                await NumeroSocioFocus.Handle(Unit.Default);
                return;
            }

            if (int.TryParse(GetNumeroTessera, out int numeroTessera))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroTessera <= 0) { }
                else
                {
                    if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera))
                    {
                        InfoLabel = "Tessera già in uso";
                        await NumeroTesseraFocus.Handle(Unit.Default);
                        return;
                    }
                }

            }
            else
            {
                // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                // (In questo caso considerala come se fosse <= 0)
                InfoLabel = "Numero Tessera non può essere zero";
                await NumeroTesseraFocus.Handle(Unit.Default);
                return;
            }



            try
            {
                idsocio = await Q.AddCodiceSocio(BindingT);
                //await Host.Router.NavigateBack.Execute();
            }
            catch (Exception ex)
            {
                InfoLabel = $"Errore durante il salvataggio: {ex.Message}";
            }
            finally
            {
                if (idsocio == -1)
                {
                    InfoLabel = "Errore durante il salvataggio. Verificare i dati e riprovare.";
                    await NumeroSocioFocus.Handle(Unit.Default);
                }
                else
                {
                    OnBack(_idDaModificare);
                }
            }
        }
    }
}
