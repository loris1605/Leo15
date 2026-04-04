using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;
using ViewModels;

namespace Views;

public partial class TestView : ReactiveUserControl<TestViewModel>
{
    public TestView()
    {
#if DEBUG

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();

#endif

        InitializeComponent();
    }
}