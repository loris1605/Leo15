using Models.Entity;
using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial class OperatoreInputBase : InputViewModel
    {
        protected string Nickname => BindingT is null ? "" : BindingT.NomeOperatore.Trim();
        int CodiceOperatore => BindingT is null ? 0 : BindingT.Id;

        public int GetCodiceOperatore => CodiceOperatore;

        public bool IsNicknameEmpty => Nickname == "";
        public bool IsPasswordEmpty => Password == "";

        public bool CheckLess2Nickname => Nickname.Length < 2;
        public bool CheckLess2Password => Password.Length < 2;

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

        protected async Task<bool> ValidaDati()
        {
            if (IsNicknameEmpty)
            {
                InfoLabel = "Inserire il nome dell'operatore";
                await OnFocus(NomeFocus);
                return false;
            }

            if (IsPasswordEmpty)
            {
                InfoLabel = "Inserire la password di accesso";
                await OnFocus(PasswordFocus);
                return false;
            }

            if (CheckLess2Nickname)
            {
                InfoLabel = "Formato nome non valido (min. 2 caratteri)";
                await OnFocus(NomeFocus);
                return false;
            }

            if (CheckLess2Password)
            {
                InfoLabel = "Formato password non valido (min. 2 caratteri)";
                await OnFocus(PasswordFocus);
                return false;
            }


            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
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

    public partial class OperatoreInputBase
    {
        public OperatoreInputBase(IScreen host) : base(host)
        {
            EscPressedCommand = ReactiveCommand.Create(OnBackEsc);

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.NomeOperatore)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.NomeOperatore = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.Password)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Password = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.Abilitato)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Abilitato = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.Badge)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.Badge = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.CodicePerson)
                    .Where(_ => BindingT != null)
                    .Subscribe(val => BindingT.CodicePerson = val)
                    .DisposeWith(d);
                this.WhenAnyValue(x => x.Abilitato)
                    .Select(val => val ? "Si" : "No") // Trasforma il bool in stringa
                    .Subscribe(text => AbilitatoText = text) // Assegna il risultato
                    .DisposeWith(d);
            });
        }
    

        private string nome = string.Empty;
        public string NomeOperatore
        {
            get => nome;
            set => this.RaiseAndSetIfChanged(ref nome, value);
        }

        private bool nomeoperatoreenabled = true;
        public bool NomeOperatoreEnabled
        {
            get => nomeoperatoreenabled;
            set => this.RaiseAndSetIfChanged(ref nomeoperatoreenabled, value);
        }

        private string password = string.Empty;
        public string Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        private bool abilitato = true;
        public bool Abilitato
        {
            get => abilitato;
            set => this.RaiseAndSetIfChanged(ref abilitato, value);
        }

        private string abilitatotext = string.Empty;
        public string AbilitatoText
        {
            get => abilitatotext;
            set => this.RaiseAndSetIfChanged(ref abilitatotext, value);
        }

        private int badge = 0;
        public int Badge
        {
            get => badge;
            set => this.RaiseAndSetIfChanged(ref badge, value);
        }

        private int _codiceperson = 0;
        public int CodicePerson
        {
            get => _codiceperson;
            set => this.RaiseAndSetIfChanged(ref _codiceperson, value);
        }

        private OperatoreMap bindingt = Create<OperatoreMap>.Instance();
        public OperatoreMap BindingT
        {
            get => bindingt;
            set
            {
                // 1. Aggiorna il riferimento (fondamentale per RaiseAndSetIfChanged)
                this.RaiseAndSetIfChanged(ref bindingt, value);

                // 2. Se carichi un socio, allinea la UI al modello
                if (value != null)
                {
                    this.NomeOperatore = value.NomeOperatore ?? "";
                    this.Password = value.Password ?? "";
                    this.Abilitato = value.Abilitato;
                    this.Badge = value.Badge;
                    this.CodicePerson = value.CodicePerson;
                }
            }
        }

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> PasswordFocus { get; } = new();
        
    }
}
