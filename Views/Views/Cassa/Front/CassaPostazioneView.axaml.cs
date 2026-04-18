using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class CassaPostazioneView : BaseUserControl<CassaPostazioneViewModel>
{
    protected override string RootControlName => "MainGrid";

    public CassaPostazioneView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {


            #region OneWay

            this.OneWayBind(ViewModel,
                            vm => vm.Titolo,
                            v => v.Title.TitoloPagina)
                .DisposeWith(d);

            #endregion


        });
    }
}