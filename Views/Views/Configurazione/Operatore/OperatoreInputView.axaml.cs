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

public partial class OperatoreInputView : ReactiveUserControl<OperatoreInputBase>
{
    public OperatoreInputView()
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
                
                this.OneWayBind(ViewModel,
                    vm => vm.EscFocus,
                    view => view.InputSaveBox.EscFocus)
                .DisposeWith(d);

                
                ViewModel?.PasswordFocus
                        .RegisterHandler(interaction =>
                        {
                            PasswordBox.Focus();
                            PasswordBox.SelectAll();
                            interaction.SetOutput(Unit.Default);
                        })
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
                          vm => vm.NomeOperatore,
                          v => v.NomeBox.Text)
                    .DisposeWith(d);

                //Bind Nome to TextBox
                this.Bind(ViewModel,
                          vm => vm.Password,
                          v => v.PasswordBox.Text)
                    .DisposeWith(d);

                //Bind Nome to TextBox
                this.Bind(ViewModel,
                          vm => vm.Badge,
                          v => v.BadgeBox.Text,
                          vmToView => vmToView.ToString(),          // Da int a string
                          viewToVm => int.TryParse(viewToVm, out var res) ? res : 0) // Da string a int
                    .DisposeWith(d);

                //Bind Nome to TextBox
                this.Bind(ViewModel,
                          vm => vm.Abilitato,
                          v => v.AbilitatoCheckBox.IsChecked)
                    .DisposeWith(d);


                #endregion

                #region OneWay

                this.OneWayBind(ViewModel,
                        vm => vm.Titolo,
                        v => v.lblTitolo.Text)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.AbilitatoText,
                        v => v.AbilitatoText.Text)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.FieldsEnabled,
                        v => v.InputGrid.IsEnabled)
                .DisposeWith(d);

                //Bind Enabled to NomeBox
                this.OneWayBind(ViewModel,
                          vm => vm.NomeOperatoreEnabled,
                          v => v.NomeBox.IsVisible)
                    .DisposeWith(d);

                //Bind Enabled to NomeBox
                this.OneWayBind(ViewModel,
                          vm => vm.NomeOperatoreEnabled,
                          v => v.AbilitatoCheckBox.IsVisible)
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
                    System.Diagnostics.Debug.WriteLine(">>> [VIEW] OperatoreInputView deattivata, DataContext rimosso.");
                }).DisposeWith(d);
            });
    }
}