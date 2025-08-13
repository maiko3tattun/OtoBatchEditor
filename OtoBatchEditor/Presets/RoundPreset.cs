using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class RoundPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.Round;
        [YamlIgnore] public RoundViewModel ViewModel { get; private set; }

        public bool Offset { get; set; } = true;
        public bool Pre { get; set; } = true;
        public bool Ovl { get; set; } = true;
        public bool Consonant { get; set; } = true;
        public bool Blank { get; set; } = true;

        public RoundPreset() { }
        public RoundPreset(RoundViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.Offset = Offset;
                ViewModel.Pre = Pre;
                ViewModel.Ovl = Ovl;
                ViewModel.Consonant = Consonant;
                ViewModel.Blank = Blank;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<RoundPreset>(text);

            ViewModel.Offset = Offset = preset.Offset;
            ViewModel.Pre = Pre = preset.Pre;
            ViewModel.Ovl = Ovl = preset.Ovl;
            ViewModel.Consonant = Consonant = preset.Consonant;
            ViewModel.Blank = Blank = preset.Blank;
        }

        public override void Init()
        {
            Offset = ViewModel.Offset;
            Pre = ViewModel.Pre;
            Ovl = ViewModel.Ovl;
            Consonant = ViewModel.Consonant;
            Blank = ViewModel.Blank;
        }
    }
}
