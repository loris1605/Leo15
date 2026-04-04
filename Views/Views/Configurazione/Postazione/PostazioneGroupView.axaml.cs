using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Leonardo;

public partial class PostazioneGroupView : ReactiveUserControl<PostazioneGroupViewModel>
{
    public PostazioneGroupView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (PostazioneDataGrid != null)
            {
                PostazioneDataGrid.LoadingRowGroup += OnLoadingRowGroup;

                Disposable.Create(() => PostazioneDataGrid.LoadingRowGroup -= OnLoadingRowGroup)
                    .DisposeWith(d);
            }

            // Enter Key Pressed

            #region TwoWay

            //Bind PasswordText to TextBox


            #endregion

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
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] PostazioneGroupView deattivata, DataContext rimosso.");
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