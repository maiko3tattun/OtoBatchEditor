using Avalonia.Controls;
using OtoBatchEditor.ViewModels;

namespace OtoBatchEditor.Views;

public partial class PageNFD : UserControl
{
    public PageNFD()
    {
        InitializeComponent();
        DataContext = new NFDViewModel();
    }
}