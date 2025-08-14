using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PagePreutter : UserControl
{
    public PagePreutter()
    {
        InitializeComponent();

        var viewModel = new PreutterViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.Preutter, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.Preutter) is PreutterPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new PreutterPreset(viewModel, "Latest"));
        }
    }
}