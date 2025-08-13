using DynamicData.Binding;
using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class ReplaceAliasPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.ReplaceAlias;
        [YamlIgnore] public ReplaceAliasViewModel ViewModel { get; private set; }

        public ObservableCollectionExtended<ReplaceItem> ReplaceItems { get; set; } = new ObservableCollectionExtended<ReplaceItem>();
        public bool IsRegex { get; set; } = false;
        public bool NoConvertSuffix { get; set; } = false;
        public string TestText { get; set; } = "あ_C4";
        public string TestSuffix { get; set; } = "_C4";

        public ReplaceAliasPreset() { }
        public ReplaceAliasPreset(ReplaceAliasViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
            foreach (var item in viewModel.ReplaceItems)
            {
                ReplaceItems.Add(new ReplaceItem(item.IsChecked, item.Before, item.After));
            }
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                Set(this);
                ViewModel.ReplaceTest();
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<ReplaceAliasPreset>(text);
            Set(preset);
            ViewModel.ReplaceTest();
        }

        public override void Init()
        {
            ReplaceItems = ViewModel.ReplaceItems;
            IsRegex = ViewModel.IsRegex;
            NoConvertSuffix = ViewModel.NoConvertSuffix;
            TestText = ViewModel.TestText;
            TestSuffix = ViewModel.TestSuffix;
        }

        private void Set(ReplaceAliasPreset preset)
        {
            ViewModel.IsRegex = preset.IsRegex;
            ViewModel.NoConvertSuffix = preset.NoConvertSuffix;
            ViewModel.TestText = preset.TestText;
            ViewModel.TestSuffix = preset.TestSuffix;
            ViewModel.ReplaceItems.Clear();
            foreach (var item in preset.ReplaceItems)
            {
                ViewModel.ReplaceItems.Add(new ReplaceItem(item.IsChecked, item.Before, item.After));
            }
        }
    }
}
