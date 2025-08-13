using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class ParamShiftPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.ParamShift;
        [YamlIgnore] public ParamShiftViewModel ViewModel { get; private set; }

        public string Shift { get; set; } = "0";

        public ParamShiftPreset() { }
        public ParamShiftPreset(ParamShiftViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.Shift = Shift;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<ParamShiftPreset>(text);

            ViewModel.Shift = Shift = preset.Shift;
        }

        public override void Init()
        {
            Shift = ViewModel.Shift;
        }
    }
}
