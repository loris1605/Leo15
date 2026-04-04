using Models.Repository;
using ReactiveUI;
using SysNet;
using SysNet.Converters;

namespace ViewModels
{
    public class CodiceSocioDelViewModel : CodiceSocioInputBase
    {
        private PersonR Q { get; set; }
        private readonly int _idDaModificare;
        private readonly int _idRitorno;

        public CodiceSocioDelViewModel(IScreen host, int idsocio, int idperson) : base(host)
        {
            _idDaModificare = idsocio;
            _idRitorno = idperson;

            FieldsVisibile = false;
            FieldsEnabled = false;

            Q = Create<PersonR>.Instance();

            OnEscFocus().FireAndForget();
            
        }

        protected override void OnFinalDestruction()
        {
            Q.Dispose();
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
            Titolo = "Elimina Codice Socio : " + GetNumeroSocio;
            Titolo1 = "per " + GetNomeCognome;
            
            await OnEscFocus();
        }
 
        protected async override Task OnSaving()
        {

            if (!await Q.DelSocio(BindingT))
            {
                InfoLabel = "Errore Db eliminazione person";
                await OnEscFocus();
                return;
            }
            OnBack(_idRitorno);
        }
    }
}
