using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class TariffaGroupView : BaseUserControl<TariffaGroupViewModel>
{
    protected override string RootControlName => "MainGrid";

    public TariffaGroupView()
    {
        InitializeComponent();

        

    }

}