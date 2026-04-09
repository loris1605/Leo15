using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;


namespace Views;

public partial class OperatoreGroupView : BaseUserControl<OperatoreGroupViewModel>
{
    protected override string RootControlName => "MainGrid";

    public OperatoreGroupView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (OperatoreDataGrid != null)
            {
                OperatoreDataGrid.LoadingRowGroup += OnLoadingRowGroup;

                Disposable.Create(() => OperatoreDataGrid.LoadingRowGroup -= OnLoadingRowGroup)
                    .DisposeWith(d);
            }


            // Enter Key Pressed

            

            #region OneWay

            

            //this.OneWayBind(ViewModel,
            //        vm => vm.EnabledButton,
            //        v => v.CodiceSocio.IsEnabled,
            //        l => l)
            //.DisposeWith(d);

            #endregion

            #region Commands
       


            #endregion

            Disposable.Create(() => {
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] OperatoreGroupView deattivata, DataContext rimosso.");
            }).DisposeWith(d);

        });
    }

    private void OnLoadingRowGroup(object? sender, DataGridRowGroupHeaderEventArgs e)
    {
        if (sender is DataGrid grid && e.RowGroupHeader.DataContext is DataGridCollectionViewGroup group)
        {
            // In Avalonia 11 si usa ExpandRowGroup con 'false' per chiudere
            // Il secondo parametro 'false' indica "NON espandere" -> quindi CHIUDI
            Dispatcher.UIThread.Post(() =>
            {
               grid.CollapseRowGroup(group, true);
            }, DispatcherPriority.Render);
        }
    }
}