using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class AddCVPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.AddCV;
        [YamlIgnore] public AddCVViewModel ViewModel { get; private set; }

        public int ModeIndex { get; set; } = 0;
        public bool AltVersion { get; set; } = true;
        public bool OverWrite { get; set; } = false;

        public AddCVPreset() { }
        public AddCVPreset(AddCVViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.ModeIndex = ModeIndex;
                ViewModel.AltVersion = AltVersion;
                ViewModel.OverWrite = OverWrite;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<AddCVPreset>(text);

            ViewModel.ModeIndex = ModeIndex = preset.ModeIndex;
            ViewModel.AltVersion = AltVersion = preset.AltVersion;
            ViewModel.OverWrite = OverWrite = preset.OverWrite;
        }

        public override void Init()
        {
            ModeIndex = ViewModel.ModeIndex;
            AltVersion = ViewModel.AltVersion;
            OverWrite = ViewModel.OverWrite;
        }
    }
}
