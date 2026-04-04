using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Leonardo;

public partial class PermessiView : ReactiveUserControl<PermessiViewModel>
{
    public PermessiView()
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

            Disposable.Create(() => {
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] PermessiView deattivata, DataContext rimosso.");
            }).DisposeWith(d);
        });

    }
}