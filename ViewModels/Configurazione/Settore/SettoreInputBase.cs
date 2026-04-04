using Models.Entity;
using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial class SettoreInputBase : InputViewModel
    {
       

        public SettoreInputBase(IScreen host) : base(host)
        {
            EscPressedCommand = ReactiveCommand.Create(OnBackEsc);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.NomeSettore)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.NomeSettore = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.EtichettaSettore)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.EtichettaSettore = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.CodiceTipoSettore)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.CodiceTipoSettore = val)
                    .DisposeWith(d);
                

            });
        }

        protected async override Task OnSaving() { await Task.CompletedTask; }

        protected async Task OnFocus(Interaction<Unit, Unit> control)
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);

            try
            {
                await control.Handle(Unit.Default);
            }
            catch (Exception ex)
            {
                // Evita crash se l'handler non è ancora pronto o la vista è già chiusa
                System.Diagnostics.Debug.WriteLine("Interaction Focus fallita: " + ex.Message);
            }
        }

        private void OnBackEsc()
        {
            if (HostScreen is IGroupScreen Host)
            {
                RxApp.MainThreadScheduler.Schedule(() => {
                    Host.InputRouter.NavigationStack.Clear();
                    Host.GroupEnabled = true;
                });
            }
        }

        protected void OnBack(int value = 0)
        {
            if (HostScreen is IGroupScreen Host)
            {
                // Svuota completamente lo stack del router di input
                Host.InputRouter.NavigateBack.Execute();
                Host.InputRouter.NavigationStack.Clear();
                Host.AggiornaGrid(value);
                Host.GroupEnabled = true;
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

        protected async Task<bool> ValidaDati()
        {
            if (IsNameEmpty)
            {
                InfoLabel = "Inserire il nome del settore";
                await OnFocus(NomeFocus);
                return false;
            }
            if (CheckLess2Name)
            {
                InfoLabel = "Formato Nome Settore non valido";
                await OnFocus(NomeFocus);
                return false;
            }
            if (IsLabelEmpty)
            {
                InfoLabel = "Inserire l'etichetta del settore";
                await OnFocus(LabelFocus);
                return false;
            }
            if (CheckLess2Label)
            {
                InfoLabel = "Formato Etichetta Settore non valido";
                await OnFocus(LabelFocus);
                return false;
            }
            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;

        }

        private string nome = string.Empty;
        public string NomeSettore
        {
            get => nome;
            set => this.RaiseAndSetIfChanged(ref nome, value);
        }

        private string etichetta = string.Empty;
        public string EtichettaSettore
        {
            get => etichetta;
            set => this.RaiseAndSetIfChanged(ref etichetta, value);
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
            set
            {
                // 1. Aggiorna il riferimento (fondamentale per RaiseAndSetIfChanged)
                this.RaiseAndSetIfChanged(ref bindingt, value);

                // 2. Se carichi una postazione, allinea la UI al modello
                if (value != null)
                {
                    this.NomeSettore = value.NomeSettore ?? "";
                    this.EtichettaSettore = value.EtichettaSettore ?? "";
                    this.CodiceTipoSettore = value.CodiceTipoSettore;

                }
            }
        }

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> LabelFocus { get; } = new();

    }
}


