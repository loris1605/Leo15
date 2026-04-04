using Avalonia;
using Avalonia.Controls;

namespace ControlStyles
{
    public class UiButton : Button
    {
        //protected override Type StyleKeyOverride => typeof(Button);

        
        public UiButton()
        {
            this.Classes.Add("UiButtonStyle");
        }

        #region ToolTipText Property

        public static readonly StyledProperty<string> ToolTipTextProperty =
                               AvaloniaProperty.Register<UiButton, string>(nameof(ToolTipText), 
                                   defaultValue: string.Empty);

        public string ToolTipText
        {
            get => GetValue(ToolTipTextProperty);
            set
            {
                SetValue(ToolTipTextProperty, value);
                if (value != string.Empty)
                {
                    PseudoClasses.Set(":ToolTipText", true);
                }
                else PseudoClasses.Remove(":ToolTipText");
            }

        }

        //private void ToolTipTextChanged(UiButton titolo, AvaloniaPropertyChangedEventArgs args)
        //{
        //    // Recupera il nuovo valore dagli argomenti dell'evento
        //    var newValue = args.GetNewValue<string>();

        //    if (!string.IsNullOrWhiteSpace(newValue))
        //    {
        //        // Imposta il ToolTip solo sull'istanza 'titolo'
        //        ToolTip.SetTip(titolo, newValue);
        //    }
        //    else
        //    {
        //        titolo.ClearValue(ToolTip.TipProperty);
        //    }
        //}

        #endregion
    }
}
