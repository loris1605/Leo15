using Avalonia;
using Avalonia.Input;
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

public partial class SettoreInputView : ReactiveUserControl<SettoreInputBase>
{
    public SettoreInputView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            ViewModel?.NomeFocus
                    .RegisterHandler(interaction =>
                    {
                        NomeBox.Focus();
                        NomeBox.SelectAll();
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            ViewModel?.LabelFocus
                    .RegisterHandler(interaction =>
                    {
                        EtichettaBox.Focus();
                        EtichettaBox.SelectAll();
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

            //Bind Nome to TextBox
            this.Bind(ViewModel,
                      vm => vm.NomeSettore,
                      v => v.NomeBox.Text)
                .DisposeWith(d);

            //Bind Label to TextBox
            this.Bind(ViewModel,
                      vm => vm.EtichettaSettore,
                      v => v.EtichettaBox.Text)
                .DisposeWith(d);



            //Bind SelectedValue To TipoPostazioneCombo
            this.Bind(ViewModel,
                      vm => vm.CodiceTipoSettore,
                      v => v.TipoSettoreCombo.SelectedValue)
                .DisposeWith(d);


            #endregion

            #region OneWay

            this.OneWayBind(ViewModel,
                    vm => vm.Titolo,
                    v => v.lblTitolo.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsEnabled,
                    v => v.InputGrid.IsEnabled)
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
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] SettoreInputView deattivata, DataContext rimosso.");
            }).DisposeWith(d);
        });
    }
}