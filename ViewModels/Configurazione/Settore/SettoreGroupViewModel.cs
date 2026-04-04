using Models.Entity;
using Models.Repository;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public class SettoreGroupViewModel : GroupViewModel<SettoreMap, SettoreR>
    {
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; }
        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; }
        public ReactiveCommand<Unit, Unit> ListiniCommand { get; }
        public ReactiveCommand<Unit, Unit> RepartiCommand { get; }

        public SettoreGroupViewModel(IScreen host) : base(host)
        {

            var isHostValid = this.WhenAnyValue(x => x.HostScreen)
            .Select(h => h is IGroupScreen);

            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);

            var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null && 
                                    !item.HasReparto && 
                                    item.CodiceListino != 0 &&
                                    !loading);

            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading);


            // 2. Definizione Comandi tramite i metodi della Base
            AddCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new SettoreAddViewModel(ConfigHost)),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            UpdCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new SettoreUpdViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            DelCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new SettoreDelViewModel(ConfigHost, GroupBindingT!.Id)), canDelete);

            // Navigazioni Semplici (NavigateAndReset)
            PostazioniCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new PostazioneGroupViewModel(ConfigHost)), isNotLoading);

            TariffeCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new TariffaGroupViewModel(ConfigHost)), isNotLoading);

            OperatoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreGroupViewModel(ConfigHost)), isNotLoading);

            ListiniCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreAddViewModel(ConfigHost)), isNotLoading);

            RepartiCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreAddViewModel(ConfigHost)), isNotLoading);

            this.WhenActivated(d =>
            {
                ListiniCommand.DisposeWith(d);
                OperatoriCommand.DisposeWith(d);
                PostazioniCommand.DisposeWith(d);
                TariffeCommand.DisposeWith(d);
                RepartiCommand.DisposeWith(d);
            });
        }
       
    }
}
