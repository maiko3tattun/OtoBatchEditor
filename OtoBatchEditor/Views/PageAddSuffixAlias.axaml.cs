using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using System.Linq;

namespace OtoBatchEditor.Views;

public partial class PageAddSuffixAlias : UserControl
{
    public PageAddSuffixAlias()
    {
        InitializeComponent();

        var viewModel = new AddSuffixAliasViewModel();
        DataContext = viewModel;
        preset.DataContext = new PresetTipViewModel(PresetTypes.AddSuffixAlias, viewModel);

        if (Preset.LatestPresets.FirstOrDefault(p => p.PresetType == PresetTypes.AddSuffixAlias) is AddSuffixAliasPreset latest)
        {
            latest.Load();
        }
        else
        {
            Preset.LatestPresets.Add(new AddSuffixAliasPreset(viewModel, "Latest"));
        }
    }
}