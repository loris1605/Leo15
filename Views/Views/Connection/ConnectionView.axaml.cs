using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class ConnectionView : ReactiveUserControl<ConnectionViewModel>
{
    public ConnectionView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            
            if (ViewModel != null)
            {
                ViewModel.UserIdFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            UserNameText.Focus();
                            UserNameText.SelectAll();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);
            }

            #region TwoWay

            //Bind UserIdText to TextBox
            this.Bind(ViewModel,
                      vm => vm.UserIdText,
                      v => v.UserNameText.Text)
                .DisposeWith(d);

            ////Bind PasswordText to TextBox
            this.Bind(ViewModel,
                      vm => vm.PasswordText,
                      v => v.PasswordTextBox.Text)
                .DisposeWith(d);

            ////Bind DatabaseText to TextBox
            this.Bind(ViewModel,
                      vm => vm.DatabaseText,
                      v => v.DatabaseTextBox.Text)
                .DisposeWith(d);

            // Bind SelectedItem to ComboBox
            this.Bind(ViewModel,
                      vm => vm.SelectedInstance,
                      v => v.SqlCombo.SelectedItem)
                .DisposeWith(d);


            #endregion

            #region OneWay
            //ComboBox ItemSource
            this.OneWayBind(ViewModel,
                                vm => vm.SqlInstances,
                                v => v.SqlCombo.ItemsSource)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.IsLoading,
                            v => v.InputGrid.IsVisible,
                            isLoading => !isLoading)
                .DisposeWith(d);


            this.OneWayBind(ViewModel,
                                vm => vm.IsLoading,
                                v => v.InfoLabel.IsVisible,
                                isLoading => isLoading)
                 .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.IsLoading,
                                v => v.ButtonGrid.IsVisible,
                                isLoading => !isLoading)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.EnabledCheck,
                                v => v.CheckButton.IsEnabled,
                                l => l)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.IsLoading,
                                v => v.CheckButton.IsEnabled,
                                loading => !loading) // Converte true -> false e viceversa
                                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.AvviaVisibile,
                                v => v.AvviaButton.IsEnabled,
                                l => l)
                .DisposeWith(d);


            #endregion

            #region Commands

            this.BindCommand(ViewModel,
                             vm => vm.LoadCommand,
                             v => v.CercaButton).DisposeWith(d);

            this.BindCommand(ViewModel,
                             vm => vm.CheckCommand,
                             v => v.CheckButton).DisposeWith(d);

            this.BindCommand(ViewModel,
                             vm => vm.AvviaCommand,
                             v => v.AvviaButton).DisposeWith(d);

            #endregion

            //Evento DropDownClose sulla Combo
            Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => SqlCombo.DropDownClosed += h,
                        h => SqlCombo.DropDownClosed -= h)
            .Subscribe(_ =>
            {
                // Rimando al dispatcher per sicurezza
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    UserNameText.Focus();
                });
            })
            .DisposeWith(d);

            Disposable.Create(() => {
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] ConnectionView deattivata, DataContext rimosso.");
            }).DisposeWith(d);



        });
    }
}