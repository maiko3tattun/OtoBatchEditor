using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageAddCV : UserControl
{
    public PageAddCV()
    {
        InitializeComponent();

        var viewModel = new AddCVViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.AddCV, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.AddCV) is AddCVPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new AddCVPreset(viewModel, "Latest"));
        }
    }
}