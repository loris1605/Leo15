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
    public class TariffaGroupViewModel : GroupViewModelBase<TariffaMap>, IGroupViewModelBase
    {

        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; }
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; }
        public ReactiveCommand<Unit, Unit> ListiniCommand { get; }

        private ITariffaRepository Q;
        protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;

        protected override IObservable<bool> canDel => this.WhenAnyValue(
        x => x.GroupBindingT,
        x => x.GroupBindingT.HasListino, // Osserva esplicitamente la proprietà interna
            (item, hasListino) => item != null && !hasListino);

        

        public TariffaGroupViewModel(IScreen host,
                                       ITariffaRepository Repository) : base(host)

        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
               .Select(loading => !loading);

            PostazioniCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new PostazioneGroupViewModel(ConfigHost, Locator.Current.GetService<IPostazioneRepository>())), isNotLoading);

            SettoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new SettoreGroupViewModel(ConfigHost, Locator.Current.GetService<ISettoreRepository>())), isNotLoading);

            OperatoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreGroupViewModel(ConfigHost, Locator.Current.GetService<IOperatoreRepository>())), isNotLoading);


            this.WhenActivated(d =>
            {
                // Nota: Add/Upd/Del sono gestiti dal DisposeWith della classe base
                PostazioniCommand?.DisposeWith(d);
                SettoriCommand?.DisposeWith(d);
                OperatoriCommand?.DisposeWith(d);
                ListiniCommand?.DisposeWith(d);
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
                DataSource = new List<TariffaMap>();
                GroupedDataSource = null;
                GroupBindingT = null;
            }

            // Al termine, il wrapper della base metterà IsLoading = false
            // I pulsanti passeranno da Disabilitato a Abilitato UNA SOLA VOLTA.
        }

        private async Task UpdateCollection(List<TariffaDTO> data, int id)
        {
            // Trasformazione dati
            var mapped = await Task.Run(() => data.Select(dto => new TariffaMap(dto)).ToList());
            var GroupBindingTBackup = GroupBindingT;
            GroupBindingT = null;
            DataSource = mapped;
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
            await NavigateToInput(new TariffaAddViewModel(ConfigHost,
                                      Locator.Current.GetService<ITariffaRepository>())).ToTask();
        }

        protected async override Task OnDeleting()
        {
            await NavigateToInput(new TariffaDelViewModel(ConfigHost, GroupBindingT.Id,
                                      Locator.Current.GetService<ITariffaRepository>())).ToTask();
        }

        protected async override Task OnUpdating()
        {
            await NavigateToInput(new TariffaUpdViewModel(ConfigHost, GroupBindingT.Id,
                                      Locator.Current.GetService<ITariffaRepository>())).ToTask();
        }

        protected override async Task OnEsc() => await Task.CompletedTask;

    }
}
