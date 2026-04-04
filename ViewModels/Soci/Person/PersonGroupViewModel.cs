using Models.Entity;
using Models.Repository;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public class PersonGroupViewModel : GroupViewModel<PersonMap, PersonR>
    {

        public ReactiveCommand<Unit, Unit> AddCodiceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> DelCodiceSocioCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UpdCodiceSocioCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> AddTesseraCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DelTesseraCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> UpdTesseraCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PersonSearchCommand { get; }

        public PersonGroupViewModel(IScreen host) : base(host)
        {
            var isHostValid = this.WhenAnyValue(x => x.HostScreen)
            .Select(h => h is IGroupScreen);

            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);

            var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
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
                () => NavigateToInput(new PersonAddViewModel(ConfigHost)),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            UpdCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonUpdViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            DelCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonDelViewModel(ConfigHost, GroupBindingT!.Id)), canDelete);

            AddCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new CodiceSocioAddViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            DelCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new CodiceSocioDelViewModel(ConfigHost,
                                                                    GroupBindingT.CodiceSocio,
                                                                    GroupBindingT.Id)), canSocioDelete);

            UpdCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new CodiceSocioUpdViewModel(ConfigHost,
                                                                    GroupBindingT.CodiceSocio,
                                                                    GroupBindingT.Id)), canDelete);

            AddTesseraCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TesseraAddViewModel(ConfigHost,
                                        GroupBindingT.Id, GroupBindingT.CodiceSocio)), canDelete);

            DelTesseraCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TesseraDelViewModel(ConfigHost,
                                        GroupBindingT.CodiceTessera, GroupBindingT.Id)), canTesseraDelete);

            UpdTesseraCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new TesseraUpdViewModel(ConfigHost,
                                        GroupBindingT.CodiceTessera, GroupBindingT.Id)), canTesseraDelete);

            PersonSearchCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PersonSearchViewModel(ConfigHost)),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            this.WhenActivated(d => 
            {
                               
                AddCodiceSocioCommand.DisposeWith(d);
                DelCodiceSocioCommand.DisposeWith(d);
                UpdCodiceSocioCommand.DisposeWith(d);
                AddTesseraCommand.DisposeWith(d);
                DelTesseraCommand.DisposeWith(d);
                UpdTesseraCommand.DisposeWith(d);
                PersonSearchCommand.DisposeWith(d);

            });

        }
        
        public string NumeroSocio => BindingT is null ? "" : BindingT.NumeroSocio;
        public string NumeroTessera => BindingT is null ? "" : BindingT.NumeroTessera;
        public int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        public int CodiceTessera => BindingT is null ? 0 : BindingT.CodiceTessera;
        public int Scadenza => BindingT is null ? 0 : BindingT.Scadenza;

        
    }
}
