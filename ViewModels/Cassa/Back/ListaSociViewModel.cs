using DTO.Repository;
using Models.StoreProcedure;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class ListaSociViewModel : BaseViewModel
    {

        private ISchedaRepository Q;
        private PostazioneMap Map;

        public ListaSociViewModel(IScreen host, PostazioneMap map, ISchedaRepository repository) : base(host)
        {
            Map = map;
            Q = repository  ?? throw new ArgumentNullException(nameof(repository));
            Titolo = "ELENCO SOCI ALL'INTERNO";
        }

        protected override async Task OnEsc()
        {
            HostScreen.Router.NavigateBack.Execute().Subscribe();
        }

        protected override async Task OnLoading()
        {
            var data = await Q.SociDentro(token);

            if (data is null)
            {
                DataSource = [];
                return;
            }

            var mapped = await Task.Run(() => data.Select(dto => new SchedaMap(dto)).ToList());

            DataSource = mapped;
        }

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }

        #region DataSource

        private IList<SchedaMap> _datasource = [];
        public IList<SchedaMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        private string _titolo = string.Empty;
        public string Titolo
        {
            get => _titolo;
            set => this.RaiseAndSetIfChanged(ref _titolo, value);
        }
    }
}
