using ReactiveUI;
using SysNet;
using SysNet.Converters;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial  class PersonInputBase : InputViewModel
    {
        protected int CodicePerson => BindingT is null ? 0 : BindingT.Id;
        protected int Natoil => BindingT is null ? 0 : BindingT.Natoil;

        protected int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        protected int CodiceTessera => BindingT is null ? 0 : BindingT.CodiceTessera;
        protected string CodiceUnivoco => BindingT?.CodiceUnivoco?.Trim() ?? "";

        protected bool IsCognomeEmpty => string.IsNullOrWhiteSpace(BindingT?.Cognome);
        protected bool IsNomeEmpty => string.IsNullOrWhiteSpace(BindingT?.Nome);
        protected bool CheckLess2Surname => BindingT.Cognome.Length < 2;
        protected bool CheckLess2FirstName => BindingT.Nome.Length < 2;
        
        protected bool IsLegalAge => BindingT.Natoil.IsLegalAge();
        protected string GetNumeroTessera => BindingT?.NumeroTessera?.Trim() ?? "";
        protected string GetNumeroSocio => BindingT?.NumeroSocio?.Trim() ?? "";
        protected int GetCodicePerson => CodicePerson;

        protected string GetCognome => BindingT?.Cognome?.Trim() ?? "";
        protected string GetNome => BindingT?.Nome?.Trim() ?? "";

        public PersonInputBase(IScreen host) : base(host)
        {

            EscPressedCommand = ReactiveCommand.CreateFromTask(OnBackEsc,
                                canExecute: this.WhenAnyValue(x => x.IsLoading, loading => !loading));

            this.WhenActivated(d =>
            {
                
                this.WhenAnyValue(x => x.DataNascitaOffSet)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Natoil = val.DateTimeOffsetToDateInt())
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.BindingT.Natoil) // Monitora specificamente questa proprietà
                    .Where(natoil => natoil != default)   // O != 0, a seconda del tipo di Natoil
                    .Subscribe(natoil =>
                    {
                        // Qui 'natoil' è già il valore della proprietà specifica, non l'intero oggetto BindingT
                        this.DataNascitaOffSet = natoil.DateIntToDateTimeOffset();
                    })
                    .DisposeWith(d);

            });
            
        }

        protected async override Task OnSaving() { await Task.CompletedTask; }

        protected async Task<bool> ValidaDati()
        {
            if (IsCognomeEmpty)
            {
                InfoLabel = "Inserire il cognome del socio";
                await CognomeFocus.Handle(Unit.Default);
                return false;
            }

            if (IsNomeEmpty)
            {
                InfoLabel = "Inserire il nome del socio";
                await NomeFocus.Handle(Unit.Default);
                return false;
            }

            if (CheckLess2Surname || CheckLess2FirstName)
            {
                InfoLabel = "Formato nome o cognome non valido (min. 2 caratteri)";
                await (CheckLess2Surname ? CognomeFocus : NomeFocus).Handle(Unit.Default);
                return false;
            }

            if (!IsLegalAge)
            {
                InfoLabel = "Il socio deve essere maggiorenne";
                await NatoFocus.Handle(Unit.Default);
                return false;
            }
        

            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
        }

        
        private async Task  OnBackEsc()
        {
            IsLoading = true;
            if (HostScreen is ISociScreen sociHost)
            {
                RxApp.MainThreadScheduler.Schedule(() => {
                    sociHost.InputRouter.NavigationStack.Clear();
                    sociHost.GroupEnabled = true;
                    IsLoading = false;
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

    public partial class PersonInputBase
    {
        
        private DateTimeOffset? datanascitaoffset = new DateTimeOffset(DateTime.Now);
        public DateTimeOffset? DataNascitaOffSet
        {
            get => datanascitaoffset;
            set => this.RaiseAndSetIfChanged(ref datanascitaoffset, value);
        }

        private PersonMap bindingt = Create<PersonMap>.Instance();
        public PersonMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
        }

        private List<PersonMap> _datasource = [];
        public List<PersonMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        public Interaction<Unit, Unit> CognomeFocus { get; } = new();
        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> NatoFocus { get; } = new();
        public Interaction<Unit, Unit> TesseraFocus { get; } = new();
        public Interaction<Unit, Unit> SocioFocus { get; } = new();
        

    }
}
