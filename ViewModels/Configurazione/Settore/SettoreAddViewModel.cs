using Models.Repository;
using ReactiveUI;
using SysNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SettoreAddViewModel : SettoreInputBase
    {
        private SettoreR Q { get; set; }

        public SettoreAddViewModel(IScreen host) : base(host)
        {
            Titolo = "Aggiungi Nuovo Settore";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Create<SettoreR>.Instance();
        }

        protected override void OnFinalDestruction()
        {
            Q?.Dispose();
            Q = null;
        }

        protected override async Task OnLoading()
        {
            await CaricaCombos();
            await OnFocus(NomeFocus);
        }

        private async Task CaricaCombos()
        {
            TipoSettDataSource = await Q.LoadTipiSettore();
            CodiceTipoSettore = TipoSettDataSource[0].Id;
        }

        protected async override Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (await Q.EsisteNome(BindingT))
            {
                InfoLabel = "Settore già registrato";
                await OnFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            int newSettoreId = await Q.Add(BindingT);

            if (newSettoreId == -1)
            {
                InfoLabel = "Errore Db inserimento Settore";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(newSettoreId);
        }
    }
}
