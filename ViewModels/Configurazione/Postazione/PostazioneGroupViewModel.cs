using Avalonia.Collections;
using DTO.Entity;
using DTO.Repository;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneGroupViewModel : GroupViewModelBase<PostazioneMap>, IGroupViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; }
        public ReactiveCommand<Unit, Unit> PermessiCommand { get; }
        public ReactiveCommand<Unit, Unit> RepartiCommand { get; }

        private IPostazioneRepository Q;

        protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;

        //fa il merge con la IObservable base
        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.CodiceReparto,
            x => x.GroupBindingT.HasPermesso,
            (item, codiceSocio, hasPermesso) => item != null && codiceSocio == 0 && !hasPermesso
        );

        public PostazioneGroupViewModel(IScreen host,
                                       IPostazioneRepository Repository) : base(host)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);

            var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                    !item.HasPermesso &&
                                    item.CodiceReparto != 0 &&
                                    !loading);

            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading);


            OperatoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreGroupViewModel(ConfigHost, Locator.Current.GetService<IOperatoreRepository>())), isNotLoading);

            SettoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new SettoreGroupViewModel(ConfigHost, Locator.Current.GetService<ISettoreRepository>())), isNotLoading);

            TariffeCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new TariffaGroupViewModel(ConfigHost, Locator.Current.GetService<ITariffaRepository>())), isNotLoading);

            //PermessiCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new OperatoreAddViewModel(ConfigHost)), isNotLoading);

            RepartiCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new RepartiViewModel(ConfigHost, GroupBindingT!.Id,
                Locator.Current.GetService<IPostazioneRepository>())), canAction);

            this.WhenActivated(d =>
            {

                OperatoriCommand?.DisposeWith(d);
                SettoriCommand?.DisposeWith(d);
                TariffeCommand?.DisposeWith(d);
                PermessiCommand?.DisposeWith(d);
                RepartiCommand?.DisposeWith(d);

            });
                  
        }

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            // 1. Recupero dati (IsLoading è già TRUE grazie al wrapper)
            var data = await Q.Load(0, token);

            if (data != null && data.Count > 0)
            {
                // 2. Aggiorna tutto il blocco dati
                await UpdateCollection(data, 0);

                // 3. Seleziona l'elemento SENZA scatenare ricalcoli intermedi
                // Accertati che IdIndex o la logica di selezione non faccia scattare altri comandi
                GroupBindingT = DataSource.FirstOrDefault();
            }
            else
            {
                DataSource = new List<PostazioneMap>();
                GroupedDataSource = null;
                GroupBindingT = null;
            }

            // Al termine, il wrapper della base metterà IsLoading = false
            // I pulsanti passeranno da Disabilitato a Abilitato UNA SOLA VOLTA.
        }

        private async Task UpdateCollection(List<PostazioneDTO> data, int id)
        {
            // Trasformazione dati
            var mapped = await Task.Run(() => data.Select(dto => new PostazioneMap(dto)).ToList());

            // Assegnazione singola (DataSource deve notificare una volta sola)
            //DataSource = mapped;

            // Configurazione View
            var view = new DataGridCollectionView(mapped);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            // Assegnazione alla UI
            var GroupBindingTBackup = GroupBindingT;
            GroupBindingT = null;
            GroupedDataSource = view;
            GroupBindingT = GroupBindingTBackup;
           
            IdIndex = id;
            GroupFocus = true;
        }

        public async Task CaricaDataSource(int id = 0)
        {
            try
            {
                var data = await Q.Load(id, token);
                await UpdateCollection(data, id);
            }
            catch (OperationCanceledException) { }
        }

        protected IObservable<Unit> NavigateToReset(IRoutableViewModel vm)
        {
            if (ConfigHost == null) return Observable.Return(Unit.Default);

            IsLoading = true;
            return ConfigHost.GroupRouter
                .NavigateAndReset
                .Execute(vm)
                .Select(_ => Unit.Default)
                .Finally(() => this.IsLoading = false);
        }

        protected IObservable<Unit> NavigateToInput(IRoutableViewModel vm)
        {
            return Observable.Start(() => ConfigHost.GroupEnabled = false, RxApp.MainThreadScheduler)
                .SelectMany(_ => ConfigHost.InputRouter.Navigate.Execute(vm))
                .Select(_ => Unit.Default);
        }

        protected async override Task OnAdding()
        {
            await NavigateToInput(new PostazioneAddViewModel(ConfigHost,
                                      Locator.Current.GetService<IPostazioneRepository>())).ToTask();
        }

        protected async override Task OnDeleting()
        {
            await NavigateToInput(new PostazioneDelViewModel(ConfigHost, GroupBindingT.Id,
                                      Locator.Current.GetService<IPostazioneRepository>())).ToTask();
        }

        protected async override Task OnUpdating()
        {
            await NavigateToInput(new PostazioneUpdViewModel(ConfigHost, GroupBindingT.Id,
                                      Locator.Current.GetService<IPostazioneRepository>())).ToTask();
        }

        protected override Task OnEsc()
        {
            throw new NotImplementedException();
        }
    }
}
