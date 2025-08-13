using Avalonia.Controls;
using OtoBatchEditor.ViewModels;

namespace OtoBatchEditor.Views;

public partial class PageTrim : UserControl
{
    public PageTrim()
    {
        InitializeComponent();
        DataContext = new TrimViewModel();
    }
}