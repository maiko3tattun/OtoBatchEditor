using Avalonia.Controls;
using OtoBatchEditor.ViewModels;

namespace OtoBatchEditor.Views;

public partial class PageUnderScore : UserControl
{
    public PageUnderScore()
    {
        InitializeComponent();
        DataContext = new UnderScoreViewModel();
    }
}