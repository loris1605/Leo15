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
using ViewModels.BindableObjects;

namespace ViewModels
{
 
    public class PersonGroupViewModel : GroupViewModelBase<PersonMap>, IGroupViewModelBase
    {
        private IPersonRepository Q;

        public ReactiveCommand<Unit, Unit> AddCodiceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> DelCodiceSocioCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UpdCodiceSocioCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AddTesseraCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DelTesseraCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UpdTesseraCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PersonSearchCommand { get; }

        protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;

        public PersonGroupViewModel(IScreen host,
                              IPersonRepository personRepository = null) : base(host)
        {
            Q = personRepository ?? Locator.Current.GetService<IPersonRepository>();

            var isHostValid = this.WhenAnyValue(x => x.HostScreen)
            .Select(h => h is IGroupScreen);

            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);

            var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                   item.CodiceSocio == 0 &&
                                   !loading);

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

            // 2. Definizione Comandi tramite i metodi della Base
            AddCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonAddViewModel(ConfigHost, Locator.Current.GetService<IPersonRepository>())),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            UpdCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonUpdViewModel(ConfigHost, GroupBindingT!.Id,
                                                             Locator.Current.GetService<IPersonRepository>())), canAction);

            DelCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonDelViewModel(ConfigHost, GroupBindingT!.Id,
                                                             Locator.Current.GetService<IPersonRepository>())), canDelete);

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

            //DelTesseraCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new TesseraDelViewModel(ConfigHost,
            //                            GroupBindingT.CodiceTessera, GroupBindingT.Id)), canTesseraDelete);

            //UpdTesseraCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new TesseraUpdViewModel(ConfigHost,
            //                            GroupBindingT.CodiceTessera, GroupBindingT.Id)), canTesseraDelete);

            PersonSearchCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonSearchViewModel(ConfigHost, Locator.Current.GetService<IPersonRepository>())),
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
            var token = _cts?.Token ?? CancellationToken.None;

            IsLoading = true;
            try
            {
                var data = await Q.Load(0, token);

                token.ThrowIfCancellationRequested();

                if (data?.Count > 0)
                {
                    UpdateCollection(data, 0);
                    GroupBindingT = DataSource[0];
                }
            }
            catch (OperationCanceledException) { /* Caricamento interrotto dal cambio pagina */ }
            catch (Exception ex) { Debug.WriteLine($"Load Error: {ex.Message}"); }
            finally { IsLoading = false; }
        }

        private void UpdateCollection(List<PersonDTO> data, int id)
        {
            DataSource = data.Select(dto => new PersonMap(dto)).ToList(); ;

            // Creiamo la collezione raggruppata
            var view = new DataGridCollectionView(DataSource);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            GroupedDataSource = view;
            GroupFocus = true;
            IdIndex = id;
        }

        public async Task CaricaDataSource(int id = 0)
        {
            var token = _cts?.Token ?? CancellationToken.None;
            try
            {
                var data = await Q.Load(id, token);
                token.ThrowIfCancellationRequested();
                UpdateCollection(data, id);
            }
            catch (OperationCanceledException) { }
        }

        public override async Task CaricaByModel(object model)
        {

            var view = new DataGridCollectionView((List<PersonMap>)model);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            GroupedDataSource = view;
            GroupFocus = true;
            IdIndex = 0;
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
            if (ConfigHost == null) return Observable.Return(Unit.Default);

            IsLoading = true;
            var stringa = GroupBindingT is null ? "" : GroupBindingT.CodiceSocio.ToString();

            Debug.WriteLine($"canDelete {stringa}");

            // Esegue il cambio di stato della UI e poi naviga
            return Observable.Start(() => ConfigHost.GroupEnabled = false, RxApp.MainThreadScheduler)
                .SelectMany(_ => ConfigHost.InputRouter.Navigate.Execute(vm))
                .Select(_ => Unit.Default).Finally(() => this.IsLoading = false);
        }

        public string NumeroSocio => BindingT is null ? "" : GroupBindingT.NumeroSocio;
        public string NumeroTessera => BindingT is null ? "" : GroupBindingT.NumeroTessera;
        public int CodiceSocio => BindingT is null ? 0 : GroupBindingT.CodiceSocio;
        public int CodiceTessera => BindingT is null ? 0 : GroupBindingT.CodiceTessera;
        public int Scadenza => BindingT is null ? 0 : GroupBindingT.Scadenza;

    }
}
