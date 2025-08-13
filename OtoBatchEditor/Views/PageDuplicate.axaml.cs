using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageDuplicate : UserControl
{
    public PageDuplicate()
    {
        InitializeComponent();

        var viewModel = new DuplicateViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.Duplicate, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.Duplicate) is DuplicatePreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new DuplicatePreset(viewModel, "Latest"));
        }
    }
}