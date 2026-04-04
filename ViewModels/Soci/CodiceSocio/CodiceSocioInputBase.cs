using Models.Entity;
using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace ViewModels
{
    public partial class CodiceSocioInputBase : InputViewModel
    {
        int CodiceSocio => BindingT is null ? 0 : BindingT.CodiceSocio;
        int CodicePerson => BindingT is null ? 0 : BindingT.Id;

        protected string GetNumeroTessera => NumeroTessera;
        protected string GetNumeroSocio => NumeroSocio;
        protected int GetCodiceSocio => CodiceSocio;
        protected string GetNomeCognome => Nome + " " + Cognome;
        protected int GetCodicePerson => CodicePerson;

        public CodiceSocioInputBase(IScreen host) : base(host)
        {
            EscPressedCommand = ReactiveCommand.Create(OnBackEsc);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.NumeroSocio)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.NumeroSocio = val)
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.NumeroTessera)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.NumeroTessera = val)
                    .DisposeWith(d);
            });
        }

        protected async override Task OnSaving() { await Task.CompletedTask; }

        public async Task OnNumeroSocioFocus()
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);
            await NumeroSocioFocus.Handle(Unit.Default).ToTask();
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

    public partial class CodiceSocioInputBase
    {
        public Interaction<Unit, Unit> NumeroSocioFocus { get; } = new();
        public Interaction<Unit, Unit> NumeroTesseraFocus { get; } = new();
        
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

        //private DateTimeOffset? datanascitaoffset = new DateTimeOffset(DateTime.Now);
        //public DateTimeOffset? DataNascitaOffSet
        //{
        //    get => datanascitaoffset;
        //    set => this.RaiseAndSetIfChanged(ref datanascitaoffset, value);
        //}

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
                    this.NumeroSocio = value.NumeroSocio ?? "";
                    this.NumeroTessera = value.NumeroTessera;
                }
            }
        }

        //private List<PersonMap> _datasource = [];
        //public List<PersonMap> DataSource
        //{
        //    get => _datasource;
        //    set => this.RaiseAndSetIfChanged(ref _datasource, value);
        //}

    }
}
