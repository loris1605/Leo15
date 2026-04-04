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

public partial class PersonInputView : ReactiveUserControl<PersonInputBase>
{
    public PersonInputView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            ViewModel?.CognomeFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            CognomeBox.Focus();
                            CognomeBox.SelectAll();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            ViewModel?.NomeFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            NomeBox.Focus();
                            NomeBox.SelectAll();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            ViewModel?.NatoFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            DataNascitaPicker.Focus();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            ViewModel?.TesseraFocus
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

            ViewModel?.SocioFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            CodiceSocioBox.Focus();
                            CodiceSocioBox.SelectAll();
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


            // Enter Key Pressed

            #region TwoWay

            //Bind Cognome to TextBox
            this.Bind(ViewModel,
                      vm => vm.Cognome,
                      v => v.CognomeBox.Text)
                .DisposeWith(d);

            //Bind Nome to TextBox
            this.Bind(ViewModel,
                      vm => vm.Nome,
                      v => v.NomeBox.Text)
                .DisposeWith(d);

            //Bind DataNascitaOffset to DataNascitaPicker
            this.Bind(ViewModel,
                      vm => vm.DataNascitaOffSet,
                      v => v.DataNascitaPicker.SelectedDate)
                .DisposeWith(d);

            //Bind Codice Socio to TextBox
            this.Bind(ViewModel,
                      vm => vm.NumeroSocio,
                      v => v.CodiceSocioBox.Text)
                .DisposeWith(d);

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
                    vm => vm.FieldsEnabled,
                    v => v.InputGrid.IsEnabled)
            .DisposeWith(d);
           

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsVisibile,
                    v => v.CodiceSocioBox.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsVisibile,
                    v => v.NumeroTesseraBox.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsVisibile,
                    v => v.CodiceSocioLabel.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsVisibile,
                    v => v.NumeroTesseraLabel.IsVisible)
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
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] PersonInputView deattivata, DataContext rimosso.");
            }).DisposeWith(d);

        });
    }
}