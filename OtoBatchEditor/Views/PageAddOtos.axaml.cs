using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageAddOtos : UserControl
{
    public PageAddOtos()
    {
        InitializeComponent();

        var viewModel = new AddOtosViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.AddOtos, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.AddOtos) is AddOtosPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new AddOtosPreset(viewModel, "Latest"));
        }
    }
}