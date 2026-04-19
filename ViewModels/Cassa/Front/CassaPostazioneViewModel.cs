using DTO.Repository;
using ReactiveUI;
using Splat;
using SysNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class CassaPostazioneViewModel : BaseViewModel
    {
        private PostazioneMap cassaPostazione;

        public ReactiveCommand<Unit, Unit> EntraSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> EsceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> ListaSociCommand { get; }
        public ReactiveCommand<Unit, Unit> PosizioneEnterCommand { get; }


        public CassaPostazioneViewModel(IScreen host, PostazioneMap postazione) : base(host)
        {
            cassaPostazione = postazione;
            Titolo = $"Postazione {cassaPostazione.NomePostazione}";

            _isOpen = _isOpenManualTrigger.ToProperty(this, x => x.IsOpen);

            PosizioneEnterCommand = ReactiveCommand.CreateFromTask(OnApriScheda);
            //{
            //    if (string.IsNullOrWhiteSpace(BindingT.Posizione))
            //    {
            //        _isOpenManualTrigger.OnNext(true);
            //        return;
            //    }
            //    // Logica per entrare nella postazione
            //    // Esempio: await PostazioneService.EntraPostazioneAsync(BindingT.Posizione);
            //});

            this.WhenActivated(d =>
            {
                EntraSocioCommand?.DisposeWith(d);
                EsceSocioCommand?.DisposeWith(d);
                ListaSociCommand?.DisposeWith(d);
                PosizioneEnterCommand?.DisposeWith(d);
                _isOpenManualTrigger?.DisposeWith(d);
            });
        }

        protected async override Task OnEsc() => await HostScreen
                                                        .Router
                                                        .NavigateAndReset
                                                        .Execute(new MenuViewModel(HostScreen,
                                                            Locator.Current.GetService<IMenuRepository>()));

        protected override async Task OnLoading()
        {
            SetFocus(PosizioneFocus);
            await Task.CompletedTask;
        }

        protected override async Task OnSaving()
        {
            await Task.CompletedTask;
        }

        private async Task OnApriScheda()
        {
            if (string.IsNullOrWhiteSpace(BindingT.Posizione))
            {
                _isOpenManualTrigger.OnNext(false);
                return;
            }

            BindingT.Nome = "Loris"; // Simulazione di un nome associato alla posizione, da sostituire con la logica reale
            BindingT.Cognome = "Rossi"; // Simulazione di un cognome associato alla posizione, da sostituire con la logica reale

            _isOpenManualTrigger.OnNext(true);
            // Logica per entrare nella postazione
            // Esempio: await PostazioneService.EntraPostazioneAsync(BindingT.Posizione);
            await Task.CompletedTask;
        }
    }

    public partial class CassaPostazioneViewModel
    {
        private string _titolo = string.Empty;
        public string Titolo
        {
            get => _titolo;
            set => this.RaiseAndSetIfChanged(ref _titolo, value);
        }

        private SchedaMap bindingt = Create<SchedaMap>.Instance();
        public SchedaMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);

        }

        public Interaction<Unit, Unit> PosizioneFocus { get; } = new();

        readonly ObservableAsPropertyHelper<bool> _isOpen;
        public bool IsOpen => _isOpen.Value;
        private readonly Subject<bool> _isOpenManualTrigger = new Subject<bool>();

    }
}
