using Avalonia.Controls;
using Avalonia.Input;
using OtoBatchEditor.ViewModels;
using System.IO;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageUst : UserControl
{
    private UstViewModel viewModel;

    public PageUst()
    {
        InitializeComponent();

        viewModel = new UstViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.Ust, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.Ust) is UstPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new UstPreset(viewModel, "Latest"));
        }
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        var drop = e.Data.GetFiles();
        var first = drop?.Select(f => f.Path.LocalPath).First();
        if (first == null)
        {
            await MainWindowViewModel.MessageDialogOpen("ドロップの中身がありません");
            return;
        }

        if (File.Exists(first))
        {
            first = Path.GetDirectoryName(first)!;
        }
        if (!Directory.Exists(first))
        {
            await MainWindowViewModel.MessageDialogOpen("フォルダがありません");
            return;
        }
        viewModel.SetDirectory(first);
    }
}