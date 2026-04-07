using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class TesseraInputBase : InputViewModel
    {
        string Cognome => BindingT is null ? "" : BindingT.Cognome.Trim();
        string Nome => BindingT is null ? "" : BindingT.Nome.Trim();
        string NumeroSocio => BindingT is null ? string.Empty : BindingT.NumeroSocio;
        int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        int CodicePerson => BindingT is null ? 0 : BindingT.Id;

        protected string GetNumeroTessera => BindingT is null ? "" : BindingT.NumeroTessera;
        protected string GetNumeroSocio => NumeroSocio;
        protected int GetCodiceSocio => CodiceSocio;
        protected string GetNomeCognome => Nome + " " + Cognome;
        protected int GetCodicePerson => CodicePerson;

        protected void ResetNumeroTessera() => BindingT.NumeroTessera = string.Empty;

        public Interaction<Unit, Unit> NumeroTesseraFocus { get; } = new();
        
        public TesseraInputBase(IScreen host) : base(host)
        {
            EscPressedCommand = ReactiveCommand.CreateFromTask(OnBackEsc,
                                canExecute: this.WhenAnyValue(x => x.IsLoading, loading => !loading));

            this.WhenActivated(d =>
            {
                EscPressedCommand.DisposeWith(d);
            });
        }

        protected async override Task OnSaving() { await Task.CompletedTask; }

        public async Task OnNumeroTesseraFocus()
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);
            SetFocus(NumeroTesseraFocus);
        }

        protected async Task OnBackEsc()
        {
            IsLoading = true;
            if (HostScreen is ISociScreen sociHost)
            {
                RxApp.MainThreadScheduler.Schedule(() => {
                    sociHost.InputRouter.NavigationStack.Clear();
                    sociHost.GroupEnabled = true;
                });
            }

            await Task.CompletedTask;
        }

        protected async Task OnBack(int value = 0)
        {
            IsLoading = true;
            if (HostScreen is ISociScreen sociHost)
            {
                // Svuota completamente lo stack del router di input
                await sociHost.InputRouter.NavigateBack.Execute();
                sociHost.InputRouter.NavigationStack.Clear();
                sociHost.AggiornaGridByInt(value);
                sociHost.GroupEnabled = true;
            }

            await Task.CompletedTask;
        }
    }

    public partial class TesseraInputBase
    {

        private PersonMap bindingt = Create<PersonMap>.Instance();
        public PersonMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
        
        }
    }
}
