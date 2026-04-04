using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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

namespace Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {

        InitializeComponent();

        this.WhenActivated(d =>
        {
            //ViewModel?.PasswordFocus
            //        .RegisterHandler(interaction =>
            //        {
            //            Dispatcher.UIThread.Post(() =>
            //            {
            //                PasswordBox.Focus();
            //                PasswordBox.SelectAll();
            //            });
            //            interaction.SetOutput(Unit.Default);
            //        })
            //        .DisposeWith(d);

            // Esc Key Pressed
            //Observable.FromEventPattern<KeyEventArgs>(this, nameof(this.KeyDown))
            //        .Where(e => e.EventArgs.Key == Key.Escape)
            //        .Select(_ => Unit.Default) // <--- AGGIUNGI QUESTA RIGA
            //        .InvokeCommand(ViewModel, vm => vm.AppExitCommand)
            //        .DisposeWith(d);

            //// Enter Key Pressed
            //Observable.FromEventPattern<KeyEventArgs>(PasswordBox, nameof(PasswordBox.KeyUp))
            //        .Where(e => e.EventArgs.Key == Key.Enter)
            //        .Select(_ => Unit.Default) // <--- AGGIUNGI QUESTA RIGA
            //        .InvokeCommand(ViewModel, vm => vm.EntraCommand)
            //.DisposeWith(d);

            #region TwoWay

            //Bind PasswordText to TextBox
            this.Bind(ViewModel,
                      vm => vm.PasswordText,
                      v => v.PasswordBox.Text)
                .DisposeWith(d);

            // Bind SelectedItem to ComboBox
            this.Bind(ViewModel,
                      vm => vm.BindingT,
                      v => v.OperatoreCombo.SelectedItem)
                .DisposeWith(d);

            #endregion

            #region OneWay


            #endregion

            #region Commands


            #endregion

            //Evento DropDownClose sulla Combo
            //Observable.FromEventPattern<EventHandler, EventArgs>(
            //            h => OperatoreCombo.DropDownClosed += h,
            //            h => OperatoreCombo.DropDownClosed -= h)
            //.Subscribe(_ =>
            //{
            //    // Rimando al dispatcher per sicurezza
            //    Dispatcher.UIThread.InvokeAsync(() =>
            //    {
            //        PasswordBox.Focus();
            //        PasswordBox.SelectAll();
            //    });
            //})
            //.DisposeWith(d);


            Disposable.Create(() =>
            {
                RootGrid.Children.Clear();

                // Oppure, se vuoi essere radicale ma sicuro:
                RootGrid.Children.Add(new Panel()); // Sostituisci tutto con un pannello vuoto leggerissimo

                System.Diagnostics.Debug.WriteLine(">>> [DETACH] LoginView svuotata e pronta per GC.");

                Task.Run(async () =>
                {
                    await Task.Delay(500); // Diamo tempo ad Avalonia di completare la navigazione
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.GC.Collect();
                    System.Diagnostics.Debug.WriteLine(">>> [DEBUG] GC Eseguito post-distacco.");
                });
            }).DisposeWith(d);

        });
    }

#if DEBUG
    // Questo viene chiamato solo quando l'oggetto viene rimosso dalla RAM
    ~LoginView()
    {
        System.Diagnostics.Debug.WriteLine(">>>>>> [GC SUCCESS] La LoginView DISTRUTTA");
    }
#endif


}