using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageAddVC : UserControl
{
    public PageAddVC()
    {
        InitializeComponent();

        var viewModel = new AddVCViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.AddVC, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.AddVC) is AddVCPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new AddVCPreset(viewModel, "Latest"));
        }
    }
}