using Avalonia;
using Avalonia.Controls;

namespace OtoBatchEditor;

public partial class SubTitle : UserControl
{
    public static readonly StyledProperty<string> IconProperty = AvaloniaProperty.Register<SubTitle, string>(nameof(Icon));
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<SubTitle, string>(nameof(Text));

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public SubTitle()
    {
        InitializeComponent();
    }
}