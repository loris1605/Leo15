using Models.Repository;
using ReactiveUI;

namespace ViewModels
{
    public class TesseraUpdViewModel : TesseraInputBase
    {
        private PersonR Q { get; set; }
        private readonly int _idDaModificare;
        private readonly int _idRitorno;
        
        public TesseraUpdViewModel(IScreen host, int idtessera, int idperson) : base(host)
        {
            _idDaModificare = idtessera;
            _idRitorno = idperson;

            FieldVisibile = true;
            FieldsEnabled = true;
            FieldsVisibile = true;

            Q = new ();

        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
            BindingT = await Q.FirstTessera(_idDaModificare);
            if (BindingT == null)
            {
                InfoLabel = "Errore: Tesera non trovata nel database.";
                FieldsEnabled = false;
            }
            Titolo = "Modifica Tessera : " + GetNumeroTessera;
            Titolo1 = "per " + GetNomeCognome;

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
                    if (await Q.EsisteNumeroTesseraUpd(BindingT))
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
                if (!await Q.UpdTessera(BindingT))
                {
                    InfoLabel = "Errore Db modifica person";
                    await OnNumeroTesseraFocus();
                    return;
                }
                    
                //await Host.Router.NavigateBack.Execute();
            }
            catch (Exception ex)
            {
                InfoLabel = $"Errore durante il salvataggio: {ex.Message}";
            }
            
            OnBack(_idRitorno);
        }

    }
}
