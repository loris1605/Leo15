using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Leonardo;

public partial class TesseraInputView : ReactiveUserControl<TesseraInputBase>
{
    public TesseraInputView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            ViewModel?.NumeroTesseraFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            NumeroTesseraBox.Focus();
                            NumeroTesseraBox.SelectAll();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.EscFocus,
                    view => view.InputSaveBox.EscFocus)
                    .DisposeWith(d);

            // Esc Key Pressed
            Observable.FromEventPattern<EventHandler<KeyEventArgs>, KeyEventArgs>(
                        h => this.KeyUp += h,
                        h => this.KeyUp -= h)
            .Where(e => e.EventArgs.Key == Key.Escape)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => Unit.Default) // Il comando si aspetta Unit
            .InvokeCommand(ViewModel, x => x.EscPressedCommand)
            .DisposeWith(d);

            #region TwoWay

            //Bind Numero Tessera to TextBox
            this.Bind(ViewModel,
                      vm => vm.NumeroTessera,
                      v => v.NumeroTesseraBox.Text)
                .DisposeWith(d);


            #endregion

            #region OneWay

            this.OneWayBind(ViewModel,
                    vm => vm.Titolo,
                    v => v.lblTitolo.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.Titolo1,
                    v => v.lblTitolo1.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsEnabled,
                    v => v.InputGrid.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsVisibile,
                    v => v.InputGrid.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldVisibile,
                    v => v.NumeroTesseraLabel.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldVisibile,
                    v => v.NumeroTesseraBox.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
            .DisposeWith(d);


            #endregion

            #region Commands

            this.Bind(ViewModel,
                    vm => vm.EscPressedCommand,
                    v => v.InputSaveBox.ExitCommand).DisposeWith(d);

            this.Bind(ViewModel,
                vm => vm.SaveCommand,
                v => v.InputSaveBox.SaveCommand).DisposeWith(d);


            #endregion

            Disposable.Create(() => {
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] TesseraInputView deattivata, DataContext rimosso.");
            }).DisposeWith(d);


        });
    }
}