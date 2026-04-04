using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Leonardo;

public partial class SociView : ReactiveUserControl<SociViewModel>
{
    public SociView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            // Esc Key Pressed


            // Enter Key Pressed

            #region TwoWay

            //Bind PasswordText to TextBox


            #endregion

            #region OneWay

            this.OneWayBind(ViewModel,
                            vm => vm.GroupEnabled,
                            v => v.RouterHost.IsEnabled)
                .DisposeWith(d);

            #endregion

            #region Commands

            this.Bind(ViewModel,
                vm => vm.EsciCommand,
                v => v.Title.ExitCommand).DisposeWith(d);

            #endregion

            Disposable.Create(() => {
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] SociView deattivata, DataContext rimosso.");
            }).DisposeWith(d);

        });
    }
}