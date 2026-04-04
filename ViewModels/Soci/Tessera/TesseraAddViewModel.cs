using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class TesseraAddViewModel : TesseraInputBase
    {
        private PersonR Q { get; set; }
        private readonly int _idDaModificare;
        private readonly int _idRitorno;

        private int idtessera;

        public TesseraAddViewModel(IScreen host, int idperson, int idsocio) : base(host)
        {
            _idRitorno = idperson;
            _idDaModificare = idsocio;

            Titolo = "Nuova Tessera";
 
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Create<PersonR>.Instance();

        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
            BindingT = await Q.FirstSocio(_idDaModificare);
            if (BindingT == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
            }
            Titolo = "Nuova Tessera per " + GetNomeCognome;
            Titolo1 = "Numero Socio : " + GetNumeroSocio;
            NumeroTessera = string.Empty;
            await OnNumeroTesseraFocus();
        }

        protected async override Task OnSaving()
        {

            if (BindingT is null)
                return;

            
            if (int.TryParse(GetNumeroTessera, out int numeroTessera))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroTessera <= 0) { }
                else
                {
                    if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera))
                    {
                        InfoLabel = "Tessera già in uso";
                        await OnNumeroTesseraFocus();
                        return;
                    }
                }

            }
            else
            {
                // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                // (In questo caso considerala come se fosse <= 0)
                InfoLabel = "Numero Tessera non può essere zero";
                await OnNumeroTesseraFocus();
                return;
            }

            try
            {
                idtessera = await Q.AddTessera(BindingT);
                //await Host.Router.NavigateBack.Execute();
            }
            catch (Exception ex)
            {
                InfoLabel = $"Errore durante il salvataggio: {ex.Message}";
            }
            finally
            {
                if (idtessera == -1)
                {
                    InfoLabel = "Errore durante il salvataggio. Verificare i dati e riprovare.";
                    await OnNumeroTesseraFocus();
                }
                else
                {
                    OnBack(_idRitorno);
                }
            }
        }
    }
}
