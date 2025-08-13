using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OtoBatchEditor;

public partial class MessageDialog : UserControl
{
    public bool Execute { get; set; } = false;

    public MessageDialog()
    {
        InitializeComponent();
    }
    public MessageDialog(string text)
    {
        InitializeComponent();
        this.textBlock.Text = text;
    }
    public MessageDialog(string text, string cancelText)
    {
        InitializeComponent();
        this.textBlock.Text = text;
        cancelButton.IsVisible = true;
        cancelButton.Content = cancelText;
    }
    public MessageDialog(string text, string okText, string cancelText)
    {
        InitializeComponent();
        this.textBlock.Text = text;
        okButton.Content = okText;
        cancelButton.IsVisible = true;
        cancelButton.Content = cancelText;
    }

    private void OK(object sender, RoutedEventArgs e)
    {
        Execute = true;
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
        Execute = false;
    }
}