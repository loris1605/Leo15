using DTO.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using System.Diagnostics;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class TesseraDelViewModel : TesseraInputBase
    {
        private IPersonRepository Q;
        private readonly int _idDaModificare;
        private readonly int _idRitorno;

        public TesseraDelViewModel(IScreen host, int idtessera, int idperson,
                                       IPersonRepository personRepository = null) : base(host)
        {
            _idDaModificare = idtessera;
            _idRitorno = idperson;

            FieldVisibile = false;
            FieldsEnabled = false;

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
                Titolo = "Elimina Tessera : " + GetNumeroTessera;
                Titolo1 = "per " + GetNomeCognome;
            }

            SetFocus(EscFocus);

        }

        protected async override Task OnSaving()
        {

            //if (!await Q.DelTessera(BindingT))
            //{
            //    InfoLabel = "Errore Db eliminazione person";
            //    await OnEscFocus();
            //    return;
            //}
            //OnBack(_idRitorno);
        }
    }
}
