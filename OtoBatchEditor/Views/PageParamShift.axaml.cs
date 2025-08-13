using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageParamShift : UserControl
{
    public PageParamShift()
    {
        InitializeComponent();

        var viewModel = new ParamShiftViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.ParamShift, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.ParamShift) is ParamShiftPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new ParamShiftPreset(viewModel, "Latest"));
        }
    }
}