using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class BackupPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.Backup;
        [YamlIgnore] public BackupViewModel ViewModel { get; private set; }

        public int NameIndex { get; set; } = 0;
        public bool OverWrite { get; set; } = false;

        public BackupPreset() { }
        public BackupPreset(BackupViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.NameIndex = NameIndex;
                ViewModel.OverWrite = OverWrite;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<BackupPreset>(text);

            ViewModel.NameIndex = NameIndex = preset.NameIndex;
            ViewModel.OverWrite = OverWrite = preset.OverWrite;
        }

        public override void Init()
        {
            NameIndex = ViewModel.NameIndex;
            OverWrite = ViewModel.OverWrite;
        }
    }
}
