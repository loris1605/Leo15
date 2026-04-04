using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class PersonUpdViewModel : PersonInputBase
    {
        private PersonR Q { get; set; }
        private readonly int _idDaModificare;
        

        public PersonUpdViewModel(IScreen host, int idperson) : base(host)
        {
            _idDaModificare = idperson;
                        
            Titolo = "Modifica Socio";
            FieldsEnabled = true;
            FieldsVisibile = false;

            Q = Create<PersonR>.Instance();

            
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
            DataSource = null;
        }

        protected override async Task OnLoading()
        {
            BindingT = await Q.FirstPerson(_idDaModificare);
            if (BindingT == null)
            {
                InfoLabel = "Errore: Socio non trovato nel database.";
                FieldsEnabled = false;
            }
            await OnFocus(CognomeFocus);
        }

        protected override async Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (await EsisteAnagraficaUpd())
            {
                InfoLabel = "Socio già registrato";
                return;
            }

            InfoLabel = "";

            if (!await Q.Upd(BindingT))
            {
                InfoLabel = "Errore Db modifica person";
                await OnFocus(CognomeFocus);
                return;
            }

            OnBack(_idDaModificare);
            
        }

        

        private async Task<bool> EsisteAnagraficaUpd()
        {
            string srvcognome = Cognome;
            string srvnome = Nome;

            if (srvcognome.Length == 2)
            {
                srvcognome += " ";
            }
            if (srvnome.Length == 2)
            {
                srvnome += " ";
            }
            if (BindingT is null) return false;

            BindingT.CodiceUnivoco = string.Concat(srvcognome[..3],
                                                   srvnome[..3],
                                                   BindingT.Natoil.ToString());


            return await Q.EsisteCodiceUnivoco(CodiceUnivoco, BindingT.Id);

        }
    }
}
