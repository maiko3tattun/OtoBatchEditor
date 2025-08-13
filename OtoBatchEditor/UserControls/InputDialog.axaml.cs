using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OtoBatchEditor;

public partial class InputDialog : UserControl
{
    public bool Execute { get; set; } = false;
    public string Text { get; set; } = string.Empty;

    public InputDialog()
    {
        InitializeComponent();
    }
    public InputDialog(string title, string cancelText = "", string defaultText = "")
    {
        InitializeComponent();
        this.textBlock.Text = title;
        if (!string.IsNullOrEmpty(cancelText))
        {
            cancelButton.IsVisible = true;
            cancelButton.Content = cancelText;
        }
        if (!string.IsNullOrEmpty(defaultText))
        {
            textBox.Text = defaultText;
        }
    }

    private void OK(object sender, RoutedEventArgs e)
    {
        Execute = true;
        Text = textBox.Text ?? string.Empty;
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
        Execute = false;
    }
}