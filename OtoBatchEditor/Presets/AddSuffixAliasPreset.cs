using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class AddSuffixAliasPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.AddSuffixAlias;
        [YamlIgnore] public AddSuffixAliasViewModel ViewModel { get; private set; }

        public bool IsAll { get; set; } = true;
        public bool SkipEndWith { get; set; } = true;
        public string Append { get; set; } = string.Empty;

        public AddSuffixAliasPreset() { }
        public AddSuffixAliasPreset(AddSuffixAliasViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.IsAll = IsAll;
                ViewModel.SkipEndWith = SkipEndWith;
                ViewModel.Append = Append;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<AddSuffixAliasPreset>(text);

            ViewModel.IsAll = IsAll = preset.IsAll;
            ViewModel.SkipEndWith = SkipEndWith = preset.SkipEndWith;
            ViewModel.Append = Append = preset.Append;
        }

        public override void Init()
        {
            IsAll = ViewModel.IsAll;
            SkipEndWith = ViewModel.SkipEndWith;
            Append = ViewModel.Append;
        }
    }
}
