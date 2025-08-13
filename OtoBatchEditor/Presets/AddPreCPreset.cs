using DynamicData.Binding;
using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class AddPreCPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.AddPreC;
        [YamlIgnore] public AddPreCViewModel ViewModel { get; private set; }

        public ObservableCollectionExtended<AddPreCItem> Consonants { get; set; } = new ObservableCollectionExtended<AddPreCItem>();
        public bool Overwrite { get; set; } = false;

        public AddPreCPreset() { }
        public AddPreCPreset(AddPreCViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
            foreach (var item in viewModel.Consonants)
            {
                Consonants.Add(new AddPreCItem(item.Consonant, item.Kana, item.Length, item.IsChecked));
            }
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                Set(this);
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<AddPreCPreset>(text);
            Set(preset);
        }

        public override void Init()
        {
            Consonants = ViewModel.Consonants;
            Overwrite = ViewModel.Overwrite;
        }

        private void Set(AddPreCPreset preset)
        {
            ViewModel.Overwrite = preset.Overwrite;
            ViewModel.Consonants.Clear();
            foreach (var item in preset.Consonants)
            {
                ViewModel.Consonants.Add(new AddPreCItem(item.Consonant, item.Kana, item.Length, item.IsChecked));
            }
        }
    }
}
