using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace ViewModels
{
    public abstract partial class InputViewModel : BaseViewModel
    {
                
        public int Id { get; set; }
        public virtual int ParentIndex { get; set; }

        public RoutingState Router => HostScreen.Router;

        public ReactiveCommand<Unit,Unit> EscPressedCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }

        public Interaction<Unit, Unit> EscFocus { get; } = new();

        
        public InputViewModel(IScreen host) : base(host)
        {
            SaveCommand = ReactiveCommand.CreateFromTask(OnSaving);
        }

        protected override Task OnLoading()
        {
            return Task.CompletedTask;
        }

        protected abstract Task OnSaving();

        public async Task OnEscFocus()
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);

            try
            {
                await EscFocus.Handle(Unit.Default);
            }
            catch (Exception ex)
            {
                // Evita crash se l'handler non è ancora pronto o la vista è già chiusa
                System.Diagnostics.Debug.WriteLine("Interaction Focus fallita: " + ex.Message);
            }
        }

    }

    public partial class InputViewModel
    {
        private bool fieldsenabled;
        public bool FieldsEnabled
        {
            get => fieldsenabled;
            set => this.RaiseAndSetIfChanged(ref fieldsenabled, value);
        }

        private bool fieldenabled;
        public bool FieldEnabled
        {
            get => fieldenabled;
            set => this.RaiseAndSetIfChanged(ref fieldenabled, value);
        }

        private bool fieldsvisibile;
        public bool FieldsVisibile
        {
            get => fieldsvisibile;
            set => this.RaiseAndSetIfChanged(ref fieldsvisibile, value);
        }

        private bool fieldvisibile = true;
        public bool FieldVisibile
        {
            get => fieldvisibile;
            set => this.RaiseAndSetIfChanged(ref fieldvisibile, value);
        }

        private string titolo = string.Empty;
        public string Titolo
        {
            get => titolo;
            set => this.RaiseAndSetIfChanged(ref titolo, value);
        }

        private string titolo1 = string.Empty;
        public string Titolo1
        {
            get => titolo1;
            set => this.RaiseAndSetIfChanged(ref titolo1, value);
        }

        private string infolabel = string.Empty;
        public string InfoLabel
        {
            get => infolabel;
            set => this.RaiseAndSetIfChanged(ref infolabel, value);
        }
    }
}
