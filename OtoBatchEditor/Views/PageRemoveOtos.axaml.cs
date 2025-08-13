using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageRemoveOtos : UserControl
{
    public PageRemoveOtos()
    {
        InitializeComponent();

        var viewModel = new RemoveOtosViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.RemoveOtos, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.RemoveOtos) is RemoveOtosPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new RemoveOtosPreset(viewModel, "Latest"));
        }
    }
}