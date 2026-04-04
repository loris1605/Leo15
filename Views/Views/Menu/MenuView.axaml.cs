using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class MenuView : ReactiveUserControl<MenuViewModel>
{
    public MenuView()
    {

#if DEBUG

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();

#endif

        InitializeComponent();


        this.WhenActivated(d =>
        {
            #region TwoWay



            #endregion

            #region OneWay

            this.OneWayBind(ViewModel,
                            vm => vm.AmministratoreVisible,
                            v => v.AmministratoreItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.CassaVisible,
                            v => v.CassaItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.CassaItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.BarVisible,
                            v => v.BarItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.BarItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.PulizieVisible,
                            v => v.PulizieItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.PulizieItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.GuardarobaVisible,
                            v => v.GuardarobaItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.GuardarobaItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ReportVisible,
                            v => v.ReportItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ApriGiornataEnabled,
                            v => v.ApriGiornataItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.ChiudiGiornataItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.ApriTurnoItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.ChiudiTurnoItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.IsMenuReady,
                            v => v.MainMenu.IsVisible)
                .DisposeWith(d);

            #endregion

            #region Commands


            #endregion

            Disposable.Create(() =>
            {
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] MenuView deattivata, DataContext rimosso.");
            }).DisposeWith(d);


        });
    }

#if DEBUG
    // Questo viene chiamato solo quando l'oggetto viene rimosso dalla RAM
    ~MenuView()
    {
        System.Diagnostics.Debug.WriteLine(">>>>>> [GC SUCCESS] La MenuView è stata distrutta fisicamente dalla memoria.");
    }
#endif


}