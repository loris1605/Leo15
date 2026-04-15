using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class ListiniView : BaseUserControl<ListiniViewModel>
{
    protected override string RootControlName => "MainGrid";

    public ListiniView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            // Esc Key Pressed
            Observable.FromEventPattern<EventHandler<KeyEventArgs>, KeyEventArgs>(
                        h => this.KeyUp += h,
                        h => this.KeyUp -= h)
                .Where(e => e.EventArgs.Key == Key.Escape)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => Unit.Default) // Il comando si aspetta Unit
                .InvokeCommand(ViewModel, x => x.EscPressedCommand)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                        vm => vm.Titolo,
                        v => v.lblTitolo.Text)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
            .DisposeWith(d);

        });
    }
}