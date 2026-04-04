using Models.Repository;
using ReactiveUI;
using SysNet;

namespace ViewModels
{
    public class PostazioneAddViewModel : PostazioneInputBase
    {
        private PostazioneR Q { get; set; }

        public PostazioneAddViewModel(IScreen host) : base(host)
        {
            Titolo = "Aggiungi Nuova Postazione";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Create<PostazioneR>.Instance();
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
            TipoPostDataSource = await Q.LoadTipiPostazione();
            TipoRientroDataSource = await Q.LoadTipiRientro();
            // Seleziona il primo elemento solo se la lista non è vuota
            if (TipoPostDataSource?.Count > 0)
                CodiceTipoPostazione = TipoPostDataSource[0].Id;

            if (TipoRientroDataSource?.Count > 0)
                CodiceTipoRientro = TipoRientroDataSource[0].Id;
        }

        protected async override Task OnSaving()
        {
            if (!await ValidaDati()) return;

            if (await Q.EsisteNome(BindingT))
            {
                InfoLabel = "Postazione già registrata";
                await OnFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            int newPostazioneId = await Q.Add(BindingT);

            if (newPostazioneId == -1)
            {
                InfoLabel = "Errore Db inserimento Postazione";
                await OnFocus(NomeFocus);
                return;
            }

            OnBack(newPostazioneId);
        }
    }
}
