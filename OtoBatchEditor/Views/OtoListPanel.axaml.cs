using Avalonia.Controls;
using Avalonia.Input;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class OtoListPanel : UserControl
{
    private OtoListViewModel vm;

    public OtoListPanel()
    {
        InitializeComponent();
        DataContext = vm = new OtoListViewModel();
    }

    public async void OnDropInis(object sender, DragEventArgs e)
    {
        var drop = e.Data.GetFiles();
        if (drop == null)
        {
            await MainWindowViewModel.MessageDialogOpen("ドロップの中身がありません");
            return;
        }
        vm.OnDropInis(drop.Select(f => f.Path.LocalPath).ToArray());
    }
}