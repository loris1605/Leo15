using DTO.Repository;
using ReactiveUI;
using SysNet;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneAddViewModel : PostazioneInputBase
    {
        private IPostazioneRepository Q;

        public PostazioneAddViewModel(IScreen host, IPostazioneRepository Repository) : base(host)
        {
            Titolo = "Aggiungi Nuova Postazione";
            FieldsVisibile = true;
            FieldsEnabled = true;
            Q = Repository;
        }

        protected override void OnFinalDestruction() => Q = null;

        protected override async Task OnLoading()
        {
            await CaricaCombos();
            SetFocus(NomeFocus);
            
        }

        private async Task CaricaCombos()
        {
            var dataTipoPostazione = await Q.LoadTipiPostazione();
            TipoPostDataSource = dataTipoPostazione.Select(dto => new TipoPostazioneMap(dto)).ToList();

            var dataTipoRientro = await Q.LoadTipiRientro();
            TipoRientroDataSource = dataTipoRientro.Select(dto => new TipoRientroMap(dto)).ToList();


            // Seleziona il primo elemento solo se la lista non è vuota
            if (TipoPostDataSource?.Count > 0)
                BindingT.CodiceTipoPostazione = TipoPostDataSource[0].Id;

            if (TipoRientroDataSource?.Count > 0)
                BindingT.CodiceTipoRientro = TipoRientroDataSource[0].Id;
        }

        protected async override Task OnSaving()
        {
            if (!ValidaDati()) return;

            if (await Q.EsisteNome(BindingT.ToDto()))
            {
                InfoLabel = "Postazione già registrata";
                SetFocus(NomeFocus);
                return;
            }

            InfoLabel = "";

            int newPostazioneId = await Q.Add(BindingT.ToDto());

            if (newPostazioneId == -1)
            {
                InfoLabel = "Errore Db inserimento Postazione";
                SetFocus(NomeFocus);
                return;
            }

            OnBack(newPostazioneId);
        }
    }
}
