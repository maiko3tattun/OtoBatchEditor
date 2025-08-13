using DynamicData.Binding;
using OtoBatchEditor.ViewModels;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class AddVCPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.AddVC;
        [YamlIgnore] public AddVCViewModel ViewModel { get; private set; }

        public ObservableCollectionExtended<AddVCItem> Consonant1 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant2 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant3 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant4 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant5 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public bool Overwrite { get; set; } = false;

        public AddVCPreset() { }
        public AddVCPreset(AddVCViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
            foreach (var item in viewModel.Consonant1)
            {
                Consonant1.Add(new AddVCItem(item.Consonant, item.Kana, viewModel.UpdateCheck1, item.Length, item.IsPlosive, item.IsChecked));
            }
            foreach (var item in viewModel.Consonant2)
            {
                Consonant2.Add(new AddVCItem(item.Consonant, item.Kana, viewModel.UpdateCheck2, item.Length, item.IsPlosive, item.IsChecked));
            }
            foreach (var item in viewModel.Consonant3)
            {
                Consonant3.Add(new AddVCItem(item.Consonant, item.Kana, viewModel.UpdateCheck3, item.Length, item.IsPlosive, item.IsChecked));
            }
            foreach (var item in viewModel.Consonant4)
            {
                Consonant4.Add(new AddVCItem(item.Consonant, item.Kana, viewModel.UpdateCheck4, item.Length, item.IsPlosive, item.IsChecked));
            }
            foreach (var item in viewModel.Consonant5)
            {
                Consonant5.Add(new AddVCItem(item.Consonant, item.Kana, viewModel.UpdateCheck5, item.Length, item.IsPlosive, item.IsChecked));
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
            var preset = deserializer.Deserialize<AddVCPreset>(text);
            Set(preset);
        }

        public override void Init()
        {
            Consonant1 = ViewModel.Consonant1;
            Consonant2 = ViewModel.Consonant2;
            Consonant3 = ViewModel.Consonant3;
            Consonant4 = ViewModel.Consonant4;
            Consonant5 = ViewModel.Consonant5;
            Overwrite = ViewModel.Overwrite;
        }

        private void Set(AddVCPreset preset)
        {
            for (int i = 0; i < preset.Consonant1.Count; i++)
            {
                ViewModel.Consonant1[i].Set(preset.Consonant1[i]);
            }

            for (int i = 0; i < preset.Consonant2.Count; i++)
            {
                ViewModel.Consonant2[i].Set(preset.Consonant2[i]);
            }

            for (int i = 0; i < preset.Consonant3.Count; i++)
            {
                ViewModel.Consonant3[i].Set(preset.Consonant3[i]);
            }

            for (int i = 0; i < preset.Consonant4.Count; i++)
            {
                ViewModel.Consonant4[i].Set(preset.Consonant4[i]);
            }

            ViewModel.Consonant5.Clear();
            foreach (var item in preset.Consonant5)
            {
                ViewModel.Consonant5.Add(new AddVCItem(item.Consonant, item.Kana, ViewModel.UpdateCheck5, item.Length, item.IsPlosive, item.IsChecked));
            }
            ViewModel.UpdateCheck5();
            ViewModel.Overwrite = Overwrite = preset.Overwrite;
        }
    }
}
