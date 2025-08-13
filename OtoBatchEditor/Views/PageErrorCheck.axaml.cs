using Avalonia.Controls;
using OtoBatchEditor.ViewModels;

namespace OtoBatchEditor.Views;

public partial class PageErrorCheck : UserControl
{
    public PageErrorCheck()
    {
        InitializeComponent();
        DataContext = new ErrorCheckViewModel();
    }
}