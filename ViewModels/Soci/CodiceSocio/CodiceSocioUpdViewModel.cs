using Models.Repository;
using ReactiveUI;
using SysNet;
using SysNet.Converters;

namespace ViewModels
{
    public class CodiceSocioUpdViewModel : CodiceSocioInputBase
    {
        private PersonR Q { get; set; }
        private readonly int _idDaModificare;
        private readonly int _idRitorno;

        public CodiceSocioUpdViewModel(IScreen host, int idsocio, int idperson) : base(host)
        {
            _idDaModificare = idsocio;
            _idRitorno = idperson;
            
            FieldsEnabled = true;
            FieldsVisibile = true;
            FieldVisibile = false;

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
            BindingT = await Q.FirstSocio(_idDaModificare);
            if (BindingT == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
            }
            Titolo = "Modifica Codice Socio per ";
            Titolo1 = "per " + GetNomeCognome;

            await OnNumeroSocioFocus();
        }

        protected override async Task OnSaving()
        {
            if (BindingT is null)
                return;

            if (int.TryParse(GetNumeroSocio, out int numeroSocio))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroSocio <= 0) { }
                else
                {
                    if (await Q.EsisteNumeroSocioUpd(BindingT))
                    {
                        InfoLabel = "Codice Socio già in uso";
                        await OnNumeroSocioFocus();
                        return;
                    }
                }
            }
            else
            {
                // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                // (In questo caso considerala come se fosse <= 0)
                InfoLabel = "Codice Socio non può essere zero";
                await OnNumeroSocioFocus();
                return;
            }

            

            InfoLabel = "";

            if (!await Q.UpdSocio(BindingT))
            {
                InfoLabel = "Errore Db modifica person";
                await OnNumeroSocioFocus();
                return;
            }

            OnBack(_idRitorno);

        }


    }
}
