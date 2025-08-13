using DynamicData.Binding;
using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class AddOtosPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.AddOtos;
        [YamlIgnore] public AddOtosViewModel ViewModel { get; private set; }

        public bool AddEnding { get; set; } = true;
        public string EndingAlias { get; set; } = "R";
        public bool AddVowel { get; set; } = true;
        public bool AddRentan { get; set; } = false;
        public bool AddDupli { get; set; } = false;
        public bool Overwrite { get; set; } = false;
        public ObservableCollectionExtended<DupliItem> DupliItems { get; set; } = new ObservableCollectionExtended<DupliItem>();

        public AddOtosPreset() { }
        public AddOtosPreset(AddOtosViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
            foreach (var item in viewModel.DupliItems)
            {
                DupliItems.Add(new DupliItem(item.IsChecked, item.Search, item.Alias));
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
            var preset = deserializer.Deserialize<AddOtosPreset>(text);
            Set(preset);
        }

        public override void Init()
        {
            AddEnding = ViewModel.AddEnding;
            EndingAlias = ViewModel.EndingAlias;
            AddVowel = ViewModel.AddVowel;
            AddRentan = ViewModel.AddRentan;
            AddDupli = ViewModel.AddDupli;
            Overwrite = ViewModel.Overwrite;
            DupliItems = ViewModel.DupliItems;
        }

        private void Set(AddOtosPreset preset)
        {
            ViewModel.AddEnding = preset.AddEnding;
            ViewModel.EndingAlias = preset.EndingAlias;
            ViewModel.AddVowel = preset.AddVowel;
            ViewModel.AddRentan = preset.AddRentan;
            ViewModel.AddDupli = preset.AddDupli;
            ViewModel.Overwrite = preset.Overwrite;
            ViewModel.DupliItems.Clear();
            foreach (var item in preset.DupliItems)
            {
                ViewModel.DupliItems.Add(new DupliItem(item.IsChecked, item.Search, item.Alias));
            }
        }
    }
}
