using Avalonia.Collections;
using DTO.Entity;
using DTO.Repository;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
 
    public class PersonGroupViewModel : GroupViewModelBase<PersonMap>, IGroupViewModelBase
    {
        private IPersonRepository Q;

        public ReactiveCommand<Unit, Unit> AddCodiceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> DelCodiceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdCodiceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> AddTesseraCommand { get; }
        public ReactiveCommand<Unit, Unit> DelTesseraCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdTesseraCommand { get; }
        public ReactiveCommand<Unit, Unit> PersonSearchCommand { get; }

        protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;

        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.CodiceSocio, // Osserva esplicitamente la proprietà interna
            (item, codiceSocio) => item != null && codiceSocio == 0
        );

        public PersonGroupViewModel(IScreen host,
                              IPersonRepository personRepository = null) : base(host)
        {
            Q = personRepository ?? Locator.Current.GetService<IPersonRepository>();

            var isHostValid = this.WhenAnyValue(x => x.HostScreen)
            .Select(h => h is IGroupScreen);

            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);
        

            var canSocioUpdate = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                   item.CodiceSocio != 0 &&
                                   !loading);

            var canSocioDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                   item.CodiceSocio != 0 &&
                                   item.CodiceTessera == 0 &&
                                   !loading);

            var canTesseraDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                   item.CodiceSocio != 0 &&
                                   item.CodiceTessera != 0 &&
                                   !loading);

            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading);
           

            AddCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new CodiceSocioAddViewModel(ConfigHost, GroupBindingT!.Id,
                                                             Locator.Current.GetService<IPersonRepository>())), canAction);

            DelCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new CodiceSocioDelViewModel(ConfigHost,
                                                                    GroupBindingT.CodiceSocio,
                                                                    GroupBindingT.Id,
                                                                    Locator.Current.GetService<IPersonRepository>())), canSocioDelete);

            UpdCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new CodiceSocioUpdViewModel(ConfigHost,
                                                                    GroupBindingT.CodiceSocio,
                                                                    GroupBindingT.Id,
                                                                    Locator.Current.GetService<IPersonRepository>())), canSocioUpdate);

            AddTesseraCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TesseraAddViewModel(ConfigHost,
                                                                GroupBindingT.Id, 
                                                                GroupBindingT.CodiceSocio,
                                                                Locator.Current.GetService<IPersonRepository>())), canSocioUpdate);

            DelTesseraCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TesseraDelViewModel(ConfigHost,
                                                              GroupBindingT.CodiceTessera, 
                                                              GroupBindingT.Id,
                                                              Locator.Current.GetService<IPersonRepository>())), canTesseraDelete);

            UpdTesseraCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TesseraUpdViewModel(ConfigHost,
                                                              GroupBindingT.CodiceTessera, 
                                                              GroupBindingT.Id,
                                                              Locator.Current.GetService<IPersonRepository>())), canTesseraDelete);

            PersonSearchCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonSearchViewModel(ConfigHost, 
                                                                Locator.Current.GetService<IPersonRepository>())),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            this.WhenActivated(d =>
            {

                AddCodiceSocioCommand?.DisposeWith(d);
                DelCodiceSocioCommand?.DisposeWith(d);
                UpdCodiceSocioCommand?.DisposeWith(d);
                AddTesseraCommand?.DisposeWith(d);
                DelTesseraCommand?.DisposeWith(d);
                UpdTesseraCommand?.DisposeWith(d);
                PersonSearchCommand?.DisposeWith(d);

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
                DataSource = new List<PersonMap>();
                GroupedDataSource = null;
                GroupBindingT = null;
            }

            // Al termine, il wrapper della base metterà IsLoading = false
            // I pulsanti passeranno da Disabilitato a Abilitato UNA SOLA VOLTA.
        }

        private async Task UpdateCollection(List<PersonDTO> data, int id)
        {
            // Trasformazione dati
            var mapped = await Task.Run(() => data.Select(dto => new PersonMap(dto)).ToList());

            // Assegnazione singola (DataSource deve notificare una volta sola)
            //DataSource = mapped;

            // Configurazione View
            var view = new DataGridCollectionView(mapped);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            IsLoading = true;
            // Assegnazione alla UI
            var GroupBindingTBackup = GroupBindingT;
            GroupBindingT = null;
            GroupedDataSource = view;
            GroupBindingT = GroupBindingTBackup;
            IsLoading = false;

            IdIndex = id;
            GroupFocus = true;
        }


        public async Task CaricaDataSource(int id = 0)
        {
            try
            {
                var data = await Q.Load(id, token);
                token.ThrowIfCancellationRequested();
                await UpdateCollection(data, id);
            }
            catch (OperationCanceledException) { }
        }

        public override async Task CaricaByModel(object model)
        {
            IsLoading = true;
            var view = new DataGridCollectionView((List<PersonMap>)model);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            GroupedDataSource = view;
            GroupFocus = true;
            IdIndex = 0;
            IsLoading = false;
            await Task.CompletedTask;
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
            await NavigateToInput(new PersonAddViewModel(ConfigHost, 
                                      Locator.Current.GetService<IPersonRepository>())).ToTask();
        }

        protected async override Task OnDeleting()
        {
            await NavigateToInput(new PersonDelViewModel(ConfigHost, 
                                                         GroupBindingT!.Id,
                                      Locator.Current.GetService<IPersonRepository>())).ToTask();

        }

        protected async override Task OnUpdating()
        {
            await NavigateToInput(new PersonUpdViewModel(ConfigHost,
                                                         GroupBindingT!.Id,
                                      Locator.Current.GetService<IPersonRepository>())).ToTask();
        }

        protected override Task OnEsc() => Task.CompletedTask;
        

        public string NumeroSocio => BindingT is null ? "" : GroupBindingT.NumeroSocio;
        public string NumeroTessera => BindingT is null ? "" : GroupBindingT.NumeroTessera;
        public int CodiceSocio => BindingT is null ? 0 : GroupBindingT.CodiceSocio;
        public int CodiceTessera => BindingT is null ? 0 : GroupBindingT.CodiceTessera;
        public int Scadenza => BindingT is null ? 0 : GroupBindingT.Scadenza;

    }
}
