using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class SettoreInputBase : InputViewModel
    {

        protected async override Task OnSaving() { await Task.CompletedTask; }
        protected async override Task OnLoading() => await Task.CompletedTask;

        public SettoreInputBase(IScreen host) : base(host)
        {

        }

        protected override async Task OnEsc()
        {
            if (HostScreen is IGroupScreen Host)
            {
                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    Host.InputRouter.NavigationStack.Clear();
                    Host.GroupEnabled = true;
                });
            }

            await Task.CompletedTask;

        }

        protected void OnBack(int value = 0)
        {
            if (HostScreen is IGroupScreen Host)
            {
                // Svuota completamente lo stack del router di input
                RxApp.MainThreadScheduler.Schedule(() =>
                {
                    // Eseguiamo la navigazione e la pulizia
                    Host.InputRouter.NavigateBack.Execute().Subscribe();
                    Host.InputRouter.NavigationStack.Clear();

                    // Aggiorniamo la grid e riabilitiamo i controlli
                    Host.AggiornaGridByInt(value);
                    Host.GroupEnabled = true;
                });
            }
        }
    }

    public partial class SettoreInputBase
    {

        protected string Name => BindingT.NomeSettore.Trim() is null ? "" : BindingT.NomeSettore.Trim();
        protected string Label => BindingT.EtichettaSettore.Trim() is null ? "" : BindingT.EtichettaSettore.Trim();
        protected int CodiceSettore => BindingT is null ? 0 : BindingT.Id;

        protected int GetCodiceSettore => CodiceSettore;

        protected bool IsNameEmpty => BindingT is not null && (Name == "");
        protected bool CheckLess2Name => Name.Length < 2;

        protected bool IsLabelEmpty => BindingT is not null && (Label == "");
        protected bool CheckLess2Label => Label.Length < 2;

        protected bool ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome del settore";
                SetFocus(NomeFocus);
                return false;
            }
            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Settore non valido";
                SetFocus(NomeFocus);
                return false;
            }
            if (IsLabelEmpty)
            {
                InfoLabel = "Inserire l'etichetta del settore";
                SetFocus(LabelFocus);
                return false;
            }
            if (CheckLess2Label)
            {
                InfoLabel = "Formato Etichetta Settore non valido";
                SetFocus(LabelFocus);
                return false;
            }
            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;

        }

        
        private int _codiceTipoSettore = 0;
        public int CodiceTipoSettore
        {
            get => _codiceTipoSettore;
            set => this.RaiseAndSetIfChanged(ref _codiceTipoSettore, value);
        }

        private IList<TipoSettoreMap> tipoSettoreMaps = [];
        public IList<TipoSettoreMap> TipoSettDataSource
        {
            get => tipoSettoreMaps;
            set => this.RaiseAndSetIfChanged(ref tipoSettoreMaps, value);
        }

        private SettoreMap bindingt = Create<SettoreMap>.Instance();
        public SettoreMap BindingT
        {
            get => bindingt;
        
        }

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> LabelFocus { get; } = new();

    }
}


