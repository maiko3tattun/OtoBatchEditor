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
        public string OriginalTempo { get; set; } = "120";
        public string NewTempo { get; set; } = "180";

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
                ViewModel.OriginalTempo = OriginalTempo;
                ViewModel.NewTempo = NewTempo;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<ParamShiftPreset>(text);

            ViewModel.Shift = Shift = preset.Shift;
            ViewModel.OriginalTempo = OriginalTempo = preset.OriginalTempo;
            ViewModel.NewTempo = NewTempo = preset.NewTempo;
        }

        public override void Init()
        {
            Shift = ViewModel.Shift;
            OriginalTempo = ViewModel.OriginalTempo;
            NewTempo = ViewModel.NewTempo;
        }
    }
}
