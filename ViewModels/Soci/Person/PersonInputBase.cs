using ReactiveUI;
using SysNet;
using SysNet.Converters;
using System.Diagnostics;
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
        protected async override Task OnLoading() { await Task.CompletedTask; }
        
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
      
        protected override async Task OnEsc()
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
            if (HostScreen is ISociScreen sociHost)
            {
                // 1. PROTEZIONE CRITICA: 
                // Se il primo click ha già svuotato lo stack, il secondo click 
                // deve uscire subito senza fare nulla.
                if (sociHost.InputRouter.NavigationStack.Count == 0)
                {
                    return;
                }

                // 2. Impostiamo IsLoading per disabilitare la UI
                IsLoading = true;

                try
                {
                    // 3. Eseguiamo il back solo perché abbiamo verificato che il Count > 0
                    await sociHost.InputRouter.NavigateBack.Execute();

                    // 4. Pulizia finale
                    sociHost.InputRouter.NavigationStack.Clear();
                    sociHost.AggiornaGridByInt(value);
                    sociHost.GroupEnabled = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Errore durante la navigazione: {ex.Message}");
                }
            }
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
