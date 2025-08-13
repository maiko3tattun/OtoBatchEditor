using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageAddPreC : UserControl
{
    public PageAddPreC()
    {
        InitializeComponent();

        var viewModel = new AddPreCViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.AddPreC, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.AddPreC) is AddPreCPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new AddPreCPreset(viewModel, "Latest"));
        }
    }
}