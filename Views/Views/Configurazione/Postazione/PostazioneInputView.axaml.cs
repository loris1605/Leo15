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

public partial class PostazioneInputView : ReactiveUserControl<PostazioneInputBase>
{
    public PostazioneInputView()
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
                      vm => vm.NomePostazione,
                      v => v.NomeBox.Text)
                .DisposeWith(d);

            

            //Bind SelectedValue To TipoPostazioneCombo
            this.Bind(ViewModel,
                      vm => vm.CodiceTipoPostazione,
                      v => v.TipoPostazioneCombo.SelectedValue)
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
           

            //Bind TipoRientroCombo IsEnabled to RientroVisibile
            this.OneWayBind(ViewModel,
                    vm => vm.RientroVisibile,
                    v => v.TipoRientroCombo.IsVisible)
            .DisposeWith(d);

            //this.OneWayBind(ViewModel,
            //        vm => vm.FieldsVisibile,
            //        v => v.CodiceSocioBox.IsVisible)
            //.DisposeWith(d);

            //this.OneWayBind(ViewModel,
            //        vm => vm.FieldsVisibile,
            //        v => v.NumeroTesseraBox.IsVisible)
            //.DisposeWith(d);

            //this.OneWayBind(ViewModel,
            //        vm => vm.FieldsVisibile,
            //        v => v.CodiceSocioLabel.IsVisible)
            //.DisposeWith(d);

            //this.OneWayBind(ViewModel,
            //        vm => vm.FieldsVisibile,
            //        v => v.NumeroTesseraLabel.IsVisible)
            //.DisposeWith(d);

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