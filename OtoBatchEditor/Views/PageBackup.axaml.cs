using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageBackup : UserControl
{
    public PageBackup()
    {
        InitializeComponent();

        var viewModel = new BackupViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.Backup, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.Backup) is BackupPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new BackupPreset(viewModel, "Latest"));
        }
    }
}