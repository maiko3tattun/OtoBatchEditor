using OtoBatchEditor.ViewModels;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class DuplicatePreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.Duplicate;
        [YamlIgnore] public DuplicateViewModel ViewModel { get; private set; }

        public bool Each { get; set; } = true;
        public string LeaveCount { get; set; } = "1";
        public bool AddNum { get; set; } = true;

        public DuplicatePreset() { }
        public DuplicatePreset(DuplicateViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.Each = Each;
                ViewModel.LeaveCount = LeaveCount;
                ViewModel.AddNum = AddNum;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<DuplicatePreset>(text);

            ViewModel.Each = Each = preset.Each;
            ViewModel.LeaveCount = LeaveCount = preset.LeaveCount;
            ViewModel.AddNum = AddNum = preset.AddNum;
        }

        public override void Init()
        {
            Each = ViewModel.Each;
            LeaveCount = ViewModel.LeaveCount;
            AddNum = ViewModel.AddNum;
        }
    }
}
