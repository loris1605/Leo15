using Models.Entity;
using Models.Repository;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public class PostazioneGroupViewModel : GroupViewModel<PostazioneMap, PostazioneR>
    {
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; }
        public ReactiveCommand<Unit, Unit> PermessiCommand { get; }
        public ReactiveCommand<Unit, Unit> RepartiCommand { get; }

        public PostazioneGroupViewModel(IScreen host) : base(host)
        {
            var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            (item, loading) => item != null && !loading);

            var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                    !item.HasPermesso &&
                                    item.CodiceReparto != 0 &&
                                    !loading);

            var isNotLoading = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading);


            // 2. Definizione Comandi tramite i metodi della Base
            AddCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PostazioneAddViewModel(ConfigHost)),
                this.WhenAnyValue(x => x.IsLoading, x => !x));

            UpdCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PostazioneUpdViewModel(ConfigHost, GroupBindingT!.Id)), canAction);

            DelCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToInput(new PostazioneDelViewModel(ConfigHost, GroupBindingT!.Id)), canDelete);

            
            // Navigazioni Semplici (NavigateAndReset)
            SettoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new SettoreGroupViewModel(ConfigHost)), isNotLoading);

            TariffeCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new TariffaGroupViewModel(ConfigHost)), isNotLoading);

            OperatoriCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreGroupViewModel(ConfigHost)), isNotLoading);

            PermessiCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreAddViewModel(ConfigHost)), isNotLoading);

            RepartiCommand = ReactiveCommand.CreateFromObservable(
                () => NavigateToReset(new OperatoreAddViewModel(ConfigHost)), isNotLoading);

            this.WhenActivated(d =>
            {

                OperatoriCommand.DisposeWith(d);
                SettoriCommand.DisposeWith(d);
                TariffeCommand.DisposeWith(d);
                PermessiCommand.DisposeWith(d);
                RepartiCommand.DisposeWith(d);

            });
                  
        }

        
    }
}
