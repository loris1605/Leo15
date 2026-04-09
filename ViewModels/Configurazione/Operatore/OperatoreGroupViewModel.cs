using Models.Entity;
using Models.Repository;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public class OperatoreGroupViewModel : GroupViewModelBase<OperatoreMap>
    {
        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; }
        public ReactiveCommand<Unit, Unit> PermessiCommand { get; }

        public OperatoreGroupViewModel(IScreen host) : base(host)
        {
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
            AddCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new OperatoreAddViewModel(ConfigHost)),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            UpdCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new OperatoreUpdViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            DelCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new OperatoreDelViewModel(ConfigHost, GroupBindingT!.Id)), canDelete);

            // Navigazioni Semplici (NavigateAndReset)
            PostazioniCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new PostazioneGroupViewModel(ConfigHost)), isNotLoading);

            SettoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new SettoreGroupViewModel(ConfigHost)), isNotLoading);

            TariffeCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new TariffaGroupViewModel(ConfigHost)), isNotLoading);

            PermessiCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PermessiViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            this.WhenActivated(d =>
            {
               
                PostazioniCommand.DisposeWith(d);
                SettoriCommand.DisposeWith(d);
                TariffeCommand.DisposeWith(d);
                PermessiCommand.DisposeWith(d);

            });

            

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
