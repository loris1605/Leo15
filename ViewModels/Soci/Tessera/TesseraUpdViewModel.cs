using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using Splat;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class TesseraUpdViewModel : TesseraInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        private readonly int _idRitorno;
        
        public TesseraUpdViewModel(IScreen host, int idtessera, int idperson,
                                       IPersonRepository personRepository = null) : base(host)
        {
            _idDaModificare = idtessera;
            _idRitorno = idperson;

            FieldVisibile = true;
            FieldsEnabled = true;
            FieldsVisibile = true;

            Q = personRepository ?? Locator.Current.GetService<IPersonRepository>();

        }

        protected override void OnFinalDestruction()
        {
            Q = null;
        }

        protected override async Task OnLoading()
        {
            var data = await Q.FirstTessera(_idDaModificare, token);

            if (data == null)
            {
                InfoLabel = "Errore: Tesera non trovata nel database.";
                FieldsEnabled = false;
            }
            else
            {
                BindingT = new PersonMap(data);
                Titolo = "Modifica Tessera : " + GetNumeroTessera;
                Titolo1 = "per " + GetNomeCognome;
            }
    
            SetFocus(NumeroTesseraFocus);
        }

        protected async override Task OnSaving()
        {

            if (BindingT is null) return;


            if (int.TryParse(GetNumeroTessera, out int numeroTessera))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroTessera <= 0) { }
                else
                {
                    if (await Q.EsisteNumeroTesseraUpd(BindingT.ToDto(),token))
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

            try
            {
                if (!await Q.UpdTessera(BindingT.ToDto(), token))
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
            
            await OnBack(_idRitorno);
        }

    }
}
