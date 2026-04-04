using Models.Repository;
using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Linq;

namespace ViewModels
{
    public class PersonAddViewModel : PersonInputBase
    {
               
        private PersonR Q { get; set; }

        public PersonAddViewModel(IScreen host) : base(host)
        {
            
            Titolo = "Aggiungi Nuovo Socio";
            FieldsVisibile = true;
            FieldsEnabled = true;

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
            await OnFocus(CognomeFocus);
        }

        protected async override Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (int.TryParse(GetNumeroTessera, out int numeroTessera))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroTessera <= 0) {}
                else
                {
                    if (await Q.EsisteNumeroTessera(BindingT.NumeroTessera))
                    {
                        InfoLabel = "Tessera già in uso";
                        await TesseraFocus.Handle(Unit.Default);
                        return;
                    }
                }
                
            }
            else
            {
                // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                // (In questo caso considerala come se fosse <= 0)
                InfoLabel = "Numero Tessera non può essere zero";
                await TesseraFocus.Handle(Unit.Default);
                return;
            }

            if (int.TryParse(GetNumeroSocio, out int numeroSocio))
            {
                // 2. Se la conversione riesce, controlliamo il valore
                if (numeroSocio <= 0) {}
                else
                {
                    if (await Q.EsisteNumeroSocio(BindingT.NumeroSocio))
                    {
                        InfoLabel = "Codice Socio già in uso";
                        await SocioFocus.Handle(Unit.Default);
                        return;
                    }
                }
            }
            else
            {
                // 3. Se è stringa vuota o contiene lettere, finisce qui senza crash
                // (In questo caso considerala come se fosse <= 0)
                InfoLabel = "Codice Socio non può essere zero";
                await SocioFocus.Handle(Unit.Default);
                return;
            }
           

            if (await EsisteAnagrafica())
            {
                InfoLabel = "Socio già registrato";
                await SocioFocus.Handle(Unit.Default);
                return;
            }

            InfoLabel = "";
            
            int newPersonId = await Q.Add(BindingT);

            if (newPersonId == -1)
            {
                InfoLabel = "Errore Db inserimento Socio";
                await CognomeFocus.Handle(Unit.Default);
                return;
            }

            OnBack(newPersonId);
        }

        private async Task<bool> EsisteAnagrafica()
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

            BindingT.CodiceUnivoco = string.Concat(srvcognome[..3], srvnome[..3],
                                            BindingT.Natoil.ToString());

            return await Q.EsisteCodiceUnivoco(BindingT.CodiceUnivoco);

        }
    }
}
