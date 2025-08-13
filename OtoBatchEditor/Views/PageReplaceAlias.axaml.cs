using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageReplaceAlias : UserControl
{
    public PageReplaceAlias()
    {
        InitializeComponent();

        var viewModel = new ReplaceAliasViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.ReplaceAlias, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.ReplaceAlias) is ReplaceAliasPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new ReplaceAliasPreset(viewModel, "Latest"));
        }
    }
}