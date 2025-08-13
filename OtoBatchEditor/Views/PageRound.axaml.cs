using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageRound : UserControl
{
    public PageRound()
    {
        InitializeComponent();

        var viewModel = new RoundViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.Round, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.Round) is RoundPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new RoundPreset(viewModel, "Latest"));
        }
    }
}