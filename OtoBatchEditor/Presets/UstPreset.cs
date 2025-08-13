using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class UstPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.Ust;
        [YamlIgnore] public UstViewModel ViewModel { get; private set; }

        public bool LyricFromAlias { get; set; } = true;
        public string Tempo { get; set; } = "120";

        public UstPreset() { }
        public UstPreset(UstViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.LyricFromAlias = LyricFromAlias;
                ViewModel.Tempo = Tempo;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<UstPreset>(text);

            ViewModel.LyricFromAlias = LyricFromAlias = preset.LyricFromAlias;
            ViewModel.Tempo = Tempo = preset.Tempo;
        }

        public override void Init()
        {
            LyricFromAlias = ViewModel.LyricFromAlias;
            Tempo = ViewModel.Tempo;
        }
    }
}
