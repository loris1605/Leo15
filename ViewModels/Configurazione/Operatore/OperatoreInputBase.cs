using ReactiveUI;
using SysNet;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class OperatoreInputBase : InputViewModel
    {
        int CodiceOperatore => BindingT is null ? 0 : BindingT.Id;

        public int GetCodiceOperatore => CodiceOperatore;

        public bool IsNicknameEmpty => string.IsNullOrWhiteSpace(BindingT?.NomeOperatore);
        public bool IsPasswordEmpty => string.IsNullOrWhiteSpace(BindingT?.Password);

        public bool CheckLess2Nickname => BindingT.NomeOperatore.Length < 2;
        public bool CheckLess2Password => BindingT.Password.Length < 2;

        protected async override Task OnSaving() { await Task.CompletedTask; }
        protected async override Task OnLoading() { await Task.CompletedTask; }


        protected bool ValidaDati()
        {
            if (IsNicknameEmpty)
            {
                InfoLabel = "Inserire il nome dell'operatore";
                SetFocus(NomeFocus);
                return false;
            }

            if (IsPasswordEmpty)
            {
                InfoLabel = "Inserire la password di accesso";
                SetFocus(PasswordFocus);
                return false;
            }

            if (CheckLess2Nickname)
            {
                InfoLabel = "Formato nome non valido (min. 2 caratteri)";
                SetFocus(NomeFocus);
                return false;
            }

            if (CheckLess2Password)
            {
                InfoLabel = "Formato password non valido (min. 2 caratteri)";
                SetFocus(PasswordFocus);
                return false;
            }


            InfoLabel = ""; // Pulisce eventuali errori precedenti
            return true;
        }

        protected async override Task OnEsc()
        {
            if (HostScreen is IGroupScreen Host)
            {
                RxApp.MainThreadScheduler.Schedule(() => {
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
                Host.InputRouter.NavigateBack.Execute();
                Host.InputRouter.NavigationStack.Clear();
                Host.AggiornaGridByInt(value);
                Host.GroupEnabled = true;
            }
        }
    }

    public partial class OperatoreInputBase
    {
        public OperatoreInputBase(IScreen host) : base(host)
        {
            
            this.WhenActivated(d =>
            {
                
                this.WhenAnyValue(x => x.BindingT.Abilitato)
                    .Select(val => val ? "Si" : "No") // Trasforma il bool in stringa
                    .Subscribe(text => AbilitatoText = text) // Assegna il risultato
                    .DisposeWith(d);
            });
        }
    

        private string abilitatotext = string.Empty;
        public string AbilitatoText
        {
            get => abilitatotext;
            set => this.RaiseAndSetIfChanged(ref abilitatotext, value);
        }

        private bool nomeoperatoreenabled = true;
        public bool NomeOperatoreEnabled
        {
            get => nomeoperatoreenabled;
            set => this.RaiseAndSetIfChanged(ref nomeoperatoreenabled, value);
        }

        private OperatoreMap bindingt = Create<OperatoreMap>.Instance();
        public OperatoreMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
       
        }

        public Interaction<Unit, Unit> NomeFocus { get; } = new();
        public Interaction<Unit, Unit> PasswordFocus { get; } = new();
        
    }
}
