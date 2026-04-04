using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Leonardo;

public partial class TariffaGroupView : ReactiveUserControl<TariffaGroupViewModel>
{
    public TariffaGroupView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            // Enter Key Pressed
            #region TwoWay
            //Bind PasswordText to TextBox
            #endregion


            Disposable.Create(() => {
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] TariffaGroupView deattivata, DataContext rimosso.");
            }).DisposeWith(d);
        });

    }

}