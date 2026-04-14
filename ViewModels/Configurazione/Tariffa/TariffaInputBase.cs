using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class TariffaInputBase : InputViewModel
    {
        public string Name => BindingT.NomeTariffa.Trim() is null ? "" : BindingT.NomeTariffa.Trim();
        string Label => BindingT.EtichettaTariffa.Trim() is null ? "" : BindingT.EtichettaTariffa.Trim();
        protected int GetCodiceTariffa => BindingT is null ? 0 : BindingT.Id;

        protected bool IsNameEmpty => BindingT is not null && (Name == "");
        protected bool CheckLess2Name => Name.Length < 2;
        public bool IsLabelEmpty => BindingT is not null && (Label == "");
        public bool CheckLess2Label => Label.Length < 2;

        public TariffaInputBase(IScreen host) : base(host)
        {
            
        }

        protected async override Task OnSaving() { await Task.CompletedTask; }
        protected async override Task OnLoading() => await Task.CompletedTask;

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

    public partial class TariffaInputBase
    {
        protected bool ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome della tariffa";
                SetFocus(NomeFocus);
                return false;
            }
            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Tariffa non valido";
                SetFocus(NomeFocus);
                return false;
            }
            if (IsLabelEmpty)
            {
                InfoLabel = "Inserire l'etichetta della tariffa";
                SetFocus(LabelFocus);
                return false;
            }
            if (CheckLess2Label)
            {
                InfoLabel = "Formato Etichetta Tariffa non valido";
                SetFocus(LabelFocus);
                return false;
            }
            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;

        }

        private string nome = string.Empty;
        public string NomeTariffa
        {
            get => nome;
            set => this.RaiseAndSetIfChanged(ref nome, value);
        }

        private string etichetta = string.Empty;
        public string EtichettaTariffa
        {
            get => etichetta;
            set => this.RaiseAndSetIfChanged(ref etichetta, value);
        }

        private decimal prezzo = 0M;
        public decimal PrezzoTariffa
        {
            get => prezzo;
            set => this.RaiseAndSetIfChanged(ref prezzo, value);
        }

        private TariffaMap bindingt = Create<TariffaMap>.Instance();
        public TariffaMap BindingT
        {
            get => bindingt;
            set
            {
                // 1. Aggiorna il riferimento (fondamentale per RaiseAndSetIfChanged)
                this.RaiseAndSetIfChanged(ref bindingt, value);

                // 2. Se carichi una postazione, allinea la UI al modello
                if (value != null)
                {
                    this.NomeTariffa = value.NomeTariffa ?? "";
                    this.EtichettaTariffa = value.EtichettaTariffa ?? "";
                    this.PrezzoTariffa = value.PrezzoTariffa;

                }
            }
        }

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> LabelFocus { get; } = new();

    }
}
