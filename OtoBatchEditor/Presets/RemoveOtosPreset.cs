using OtoBatchEditor.ViewModels;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public class RemoveOtosPreset : Preset
    {
        public override PresetTypes PresetType => PresetTypes.RemoveOtos;
        [YamlIgnore] public RemoveOtosViewModel ViewModel { get; private set; }

        public string SearchText { get; set; } = string.Empty;
        public bool IsRegex { get; set; } = false;

        public RemoveOtosPreset() { }
        public RemoveOtosPreset(RemoveOtosViewModel viewModel, string name) : base(name)
        {
            ViewModel = viewModel;
        }

        public override void Load()
        {
            if (Name == "Default")
            {
                ViewModel.SearchText = SearchText;
                ViewModel.IsRegex = IsRegex;
                return;
            }

            string text = File.ReadAllText(FilePath, Encoding.UTF8);
            var preset = deserializer.Deserialize<RemoveOtosPreset>(text);

            ViewModel.SearchText = SearchText = preset.SearchText;
            ViewModel.IsRegex = IsRegex = preset.IsRegex;
        }

        public override void Init()
        {
            SearchText = ViewModel.SearchText;
            IsRegex = ViewModel.IsRegex;
        }
    }
}
