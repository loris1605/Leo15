using Avalonia.Collections;
using DTO.Entity;
using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class OperatoreGroupViewModel : GroupViewModelBase<OperatoreMap>
    {
        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; }
        public ReactiveCommand<Unit, Unit> PermessiCommand { get; }

        private IOperatoreRepository Q;

        protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;

        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.CodicePermesso, // Osserva esplicitamente la proprietà interna
            (item, codiceSocio) => item != null && codiceSocio == 0
        );

        protected override IObservable<bool> canUpd => this.WhenAnyValue(x => x.GroupBindingT)
                                                           .Select(item => item != null);


        public OperatoreGroupViewModel(IScreen host,
                              IOperatoreRepository Repository = null) : base(host)
        {
            Q = Repository ?? Locator.Current.GetService<IOperatoreRepository>();

            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);

            var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                    item.CodicePermesso == 0 &&
                                    item.Id != -1 &&
                                    !loading);

            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading);


            // 2. Definizione Comandi tramite i metodi della Base
            //AddCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new OperatoreAddViewModel(ConfigHost)),
            //    this.WhenAnyValue(x => x.IsLoading, x => !x));

            //UpdCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new OperatoreUpdViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            //DelCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new OperatoreDelViewModel(ConfigHost, GroupBindingT!.Id)), canDelete);

            //// Navigazioni Semplici (NavigateAndReset)
            //PostazioniCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new PostazioneGroupViewModel(ConfigHost)), isNotLoading);

            //SettoriCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new SettoreGroupViewModel(ConfigHost)), isNotLoading);

            //TariffeCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new TariffaGroupViewModel(ConfigHost)), isNotLoading);

            //PermessiCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new PermessiViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            this.WhenActivated(d =>
            {
               
                PostazioniCommand?.DisposeWith(d);
                SettoriCommand?.DisposeWith(d);
                TariffeCommand?.DisposeWith(d);
                PermessiCommand?.DisposeWith(d);

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
                UpdateCollection(data, 0);

                // 3. Seleziona l'elemento SENZA scatenare ricalcoli intermedi
                // Accertati che IdIndex o la logica di selezione non faccia scattare altri comandi
                GroupBindingT = DataSource.FirstOrDefault();
            }
            else
            {
                DataSource = new List<OperatoreMap>();
                GroupedDataSource = null;
                GroupBindingT = null;
            }

            // Al termine, il wrapper della base metterà IsLoading = false
            // I pulsanti passeranno da Disabilitato a Abilitato UNA SOLA VOLTA.
        }

        private void UpdateCollection(List<OperatoreDTO> data, int id)
        {
            // Trasformazione dati
            var mapped = data.Select(dto => new OperatoreMap(dto)).ToList();

            // Assegnazione singola (DataSource deve notificare una volta sola)
            //DataSource = mapped;

            // Configurazione View
            var view = new DataGridCollectionView(mapped);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            // Assegnazione alla UI
            GroupedDataSource = view;
            IdIndex = id;
            GroupFocus = true;
        }

        public async Task CaricaDataSource(int id = 0)
        {
            try
            {
                var data = await Q.Load(id, token);
                token.ThrowIfCancellationRequested();
                UpdateCollection(data, id);
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

        protected override Task OnAdding()
        {
            throw new NotImplementedException();
        }

        protected override Task OnDeleting()
        {
            throw new NotImplementedException();
        }

        protected override Task OnUpdating()
        {
            throw new NotImplementedException();
        }

        protected override Task OnEsc()
        {
            throw new NotImplementedException();
        }
    }
}
