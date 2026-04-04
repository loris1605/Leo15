using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;

namespace Leonardo;


public partial class TitleView : UserControl
{

    public TitleView()
    {
        InitializeComponent();

    }

    public static readonly StyledProperty<string> TitoloPaginaProperty =
        AvaloniaProperty.Register<TitleView, string>(nameof(TitoloPagina), defaultValue: string.Empty);

    public string TitoloPagina
    {
        get => GetValue(TitoloPaginaProperty);
        set => SetValue(TitoloPaginaProperty, value);
    }

    public static readonly StyledProperty<string> ButtonTextProperty =
        AvaloniaProperty.Register<TitleView, string>(nameof(ButtonText), defaultValue: string.Empty);

    public string ButtonText
    {
        get => GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    // Proprietà Comando (Usa ReactiveCommand invece di ICommand)
    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>?> ExitCommandProperty =
        AvaloniaProperty.Register<TitleView, ReactiveCommand<Unit, Unit>?>(nameof(ExitCommand));

    public ReactiveCommand<Unit, Unit>? ExitCommand
    {
        get => GetValue(ExitCommandProperty);
        set => SetValue(ExitCommandProperty, value);
    }

    

}