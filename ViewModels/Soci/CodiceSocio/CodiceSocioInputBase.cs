using DynamicData;
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
    public partial class CodiceSocioInputBase : InputViewModel
    {
        int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        int CodicePerson => BindingT is null ? 0 : BindingT.Id;

        protected string GetNumeroTessera => BindingT?.NumeroTessera?.Trim() ?? string.Empty;
        protected string GetNumeroSocio => BindingT?.NumeroSocio?.Trim() ?? string.Empty;
        protected int GetCodiceSocio => CodiceSocio;
        protected string GetNomeCognome => BindingT is null ? "" : BindingT.Nome + " " + BindingT.Cognome;
        protected int GetCodicePerson => CodicePerson;

        public CodiceSocioInputBase(IScreen host) : base(host)
        {
            EscPressedCommand = ReactiveCommand.CreateFromTask(OnBackEsc, 
                                canExecute: this.WhenAnyValue(x => x.IsLoading, loading => !loading));

            this.WhenActivated(d =>
            {
                EscPressedCommand.DisposeWith(d);
            });
        }

        protected async override Task OnSaving() { await Task.CompletedTask; }

        
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

    public partial class CodiceSocioInputBase
    {
        public Interaction<Unit, Unit> NumeroSocioFocus { get; } = new();
        public Interaction<Unit, Unit> NumeroTesseraFocus { get; } = new();
        
                
        private PersonMap bindingt = Create<PersonMap>.Instance();
        public PersonMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
        
        }


    }
}
