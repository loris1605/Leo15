using Models.Entity;
using Models.Repository;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public class TariffaGroupViewModel : GroupViewModel<TariffaMap, TariffaR>
    {

        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; }
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; }
        public ReactiveCommand<Unit, Unit> ListiniCommand { get; }

        public TariffaGroupViewModel(IScreen host) : base(host)
        {
            var isHostValid = this.WhenAnyValue(x => x.HostScreen)
            .Select(h => h is IGroupScreen);

            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);

            var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null && !item.HasListino && !loading);

            // 1. Definiamo la condizione: abilitato solo se NON sta caricando
            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading);


            // 2. Definizione Comandi tramite i metodi della Base
            AddCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TariffaAddViewModel(ConfigHost)),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            UpdCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TariffaUpdViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            DelCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TariffaDelViewModel(ConfigHost, GroupBindingT!.Id)), canDelete);


            // Navigazioni Semplici (NavigateAndReset)
            PostazioniCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new PostazioneGroupViewModel(ConfigHost)), isNotLoading);

            SettoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new SettoreGroupViewModel(ConfigHost)), isNotLoading);

            OperatoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreGroupViewModel(ConfigHost)), isNotLoading);

            ListiniCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreAddViewModel(ConfigHost)), isNotLoading);


            this.WhenActivated(d =>
            {
                // Nota: Add/Upd/Del sono gestiti dal DisposeWith della classe base
                PostazioniCommand.DisposeWith(d);
                SettoriCommand.DisposeWith(d);
                OperatoriCommand.DisposeWith(d);
                ListiniCommand.DisposeWith(d);
            });
        }

        


    }
}
