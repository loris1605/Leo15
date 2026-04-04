using Models.Entity;
using ReactiveUI;
using SysNet;
using SysNet.Converters;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial  class PersonInputBase : InputViewModel
    {
        protected int CodicePerson => BindingT is null ? 0 : BindingT.Id;
        protected int Natoil => BindingT is null ? 0 : BindingT.Natoil;

        protected int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        protected int CodiceTessera => BindingT is null ? 0 : BindingT.CodiceTessera;
        protected string CodiceUnivoco => BindingT is null ? "" : BindingT.CodiceUnivoco.Trim();

        protected bool IsCognomeEmpty => Cognome == "";
        protected bool IsNomeEmpty => Nome == "";
        protected bool CheckLess2Surname => Cognome.Length < 2;
        protected bool CheckLess2FirstName => Nome.Length < 2;
        
        protected bool IsLegalAge => BindingT.Natoil.IsLegalAge();
        protected string GetNumeroTessera => NumeroTessera;
        protected string GetNumeroSocio => BindingT.NumeroSocio;
        protected int GetCodicePerson => CodicePerson;
        

        
        public PersonInputBase(IScreen host) : base(host)
        {

            EscPressedCommand = ReactiveCommand.Create(OnBackEsc);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.Cognome)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Cognome = val)
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.Nome)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Nome = val)
                    .DisposeWith(d);

                
                this.WhenAnyValue(x => x.NumeroSocio)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.NumeroSocio = val)
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.NumeroTessera)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.NumeroTessera = val)
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.DataNascitaOffSet)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Natoil = val.DateTimeOffsetToDateInt())
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
            if (HostScreen is ISociScreen sociHost)
            {
                RxApp.MainThreadScheduler.Schedule(() => {
                    sociHost.InputRouter.NavigationStack.Clear();
                    sociHost.GroupEnabled = true;
                });
            }
        }

        protected void OnBack(int value = 0)
        {
            if (HostScreen is ISociScreen sociHost)
            {
                // Svuota completamente lo stack del router di input
                sociHost.InputRouter.NavigateBack.Execute();
                sociHost.InputRouter.NavigationStack.Clear();
                sociHost.AggiornaGrid(value);
                sociHost.GroupEnabled = true;
            }
        }
    }

    public partial class PersonInputBase
    {
        private string cognome = string.Empty;
        public string Cognome
        {
            get => cognome;
            set => this.RaiseAndSetIfChanged(ref cognome, value);
            
        }

        private string nome = string.Empty;
        public string Nome
        {
            get => nome;
            set => this.RaiseAndSetIfChanged(ref nome, value);
            
        }

        private DateTimeOffset? datanascitaoffset = new DateTimeOffset(DateTime.Now);
        public DateTimeOffset? DataNascitaOffSet
        {
            get => datanascitaoffset;
            set => this.RaiseAndSetIfChanged(ref datanascitaoffset, value);
        }

        private string numerosocio = string.Empty;
        public string NumeroSocio
        {
            get => numerosocio;
            set => this.RaiseAndSetIfChanged(ref numerosocio, value);
            
        }

        private string numerotessera = string.Empty;
        public string NumeroTessera
        {
            get => numerotessera;
            set => this.RaiseAndSetIfChanged(ref numerotessera, value);
            
        }

        private PersonMap bindingt = Create<PersonMap>.Instance();
        public PersonMap BindingT
        {
            get => bindingt;
            set
            {
                // 1. Aggiorna il riferimento (fondamentale per RaiseAndSetIfChanged)
                this.RaiseAndSetIfChanged(ref bindingt, value);

                // 2. Se carichi un socio, allinea la UI al modello
                if (value != null)
                {
                    this.Cognome = value.Cognome ?? "";
                    this.Nome = value.Nome ?? "";
                    this.DataNascitaOffSet = value.Natoil.DateIntToDateTimeOffset();
                    this.NumeroSocio = value.NumeroSocio ?? "";
                    this.NumeroTessera = value.NumeroTessera;
                }
            }
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
