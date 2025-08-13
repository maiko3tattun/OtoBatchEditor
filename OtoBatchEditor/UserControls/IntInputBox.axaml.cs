using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.ComponentModel;

namespace OtoBatchEditor;

public partial class IntInputBox : UserControl
{
    public static readonly StyledProperty<int> ValueProperty = AvaloniaProperty.Register<IntInputBox, int>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<int> MinProperty = AvaloniaProperty.Register<IntInputBox, int>(nameof(Min), 0);
    public static readonly StyledProperty<int> MaxProperty = AvaloniaProperty.Register<IntInputBox, int>(nameof(Max), 100);
    public static readonly StyledProperty<int> DefaultValueProperty = AvaloniaProperty.Register<IntInputBox, int>(nameof(DefaultValue), 0);

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Min
    {
        get => GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public int Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public int DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set
        {
            SetValue(DefaultValueProperty, value);
            TextValue = value.ToString();
        }
    }

    public string TextValue
    {
        get => _textValue;
        set
        {
            if (int.TryParse(value, out int parsed))
            {
                if (parsed < Min || parsed > Max)
                {
                    Value = DefaultValue;
                    _textValue = DefaultValue.ToString();
                }
                else
                {
                    Value = parsed;
                    _textValue = value;
                }
            }
            else
            {
                Value = DefaultValue;
                _textValue = DefaultValue.ToString();
            }

            RaisePropertyChanged(nameof(TextValue));
        }
    }

    private string _textValue = string.Empty;

    public IntInputBox()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void RaisePropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
