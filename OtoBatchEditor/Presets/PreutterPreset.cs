using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class PreutterPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.Preutter;
        [YamlIgnore] public PreutterViewModel ViewModel { get; private set; }

        public bool Preutter { get; set; } = true;
        public bool PreFix { get; set; } = true;
        public string PreValue { get; set; } = "250";
        public bool Overlap { get; set; } = true;
        public string OvlValue { get; set; } = "3";
        public bool OvlRound { get; set; } = true;
        public bool Filter { get; set; } = true;
        public string FilterValue { get; set; } = "[aiueonN] ";

        public PreutterPreset() { }
        public PreutterPreset(PreutterViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.Preutter = Preutter;
                ViewModel.PreFix = PreFix;
                ViewModel.PreValue = PreValue;
                ViewModel.Overlap = Overlap;
                ViewModel.OvlValue = OvlValue;
                ViewModel.OvlRound = OvlRound;
                ViewModel.Filter = Filter;
                ViewModel.FilterValue = FilterValue;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<PreutterPreset>(text);

            ViewModel.Preutter = Preutter = preset.Preutter;
            ViewModel.PreFix = PreFix = preset.PreFix;
            ViewModel.PreValue = PreValue = preset.PreValue;
            ViewModel.Overlap = Overlap = preset.Overlap;
            ViewModel.OvlValue = OvlValue = preset.OvlValue;
            ViewModel.OvlRound = OvlRound = preset.OvlRound;
            ViewModel.Filter = Filter = preset.Filter;
            ViewModel.FilterValue = FilterValue = preset.FilterValue;
        }

        public override void Init()
        {
            Preutter = ViewModel.Preutter;
            PreFix = ViewModel.PreFix;
            PreValue = ViewModel.PreValue;
            Overlap = ViewModel.Overlap;
            OvlValue = ViewModel.OvlValue;
            OvlRound = ViewModel.OvlRound;
            Filter = ViewModel.Filter;
            FilterValue = ViewModel.FilterValue;
        }
    }
}
