using OtoBatchEditor.Utility;
using OtoBatchEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace OtoBatchEditor
{
    public abstract class Preset
    {
        public abstract PresetTypes PresetType { get; }
        [YamlIgnore] public string Name { get; set; } = string.Empty;
        [YamlIgnore] public string DisplayName { get; set; } = string.Empty;

        [YamlIgnore] public string FilePath => Path.Combine(DirectoryPath, $"{PresetType}_{Name}.yaml");
        [YamlIgnore] public static string DirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Maiko", "OtoBatchEditor", "Presets");

        private readonly ISerializer serializer = new SerializerBuilder()
            .IgnoreFields() // フィールドは無視してプロパティだけ保存する
            // .WithTypeResolver(new DynamicTypeResolver()) // 基底クラスをシリアライズしたとき派生クラスのプロパティも保存する（デフォルト）
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull) // nullを省略
            .WithQuotingNecessaryStrings() // ""で囲う
            .WithTypeInspector(inner => new IgnoreReactiveObjectProperties(inner)) // ReactiveObjectのプロパティを無視する
            .Build();
        public readonly IDeserializer deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties() // yaml内に不明なプロパティがあるとき無視する
            .Build();

        public Preset() { }
        public Preset(string name)
        {
            Name = name;

            if (name == "Default")
            {
                DisplayName = "デフォルト";
            }
            else if (name == "Latest")
            {
                DisplayName = "前回値";
            }
            else
            {
                DisplayName = name;
            }
        }

        public abstract void Load();
        public abstract void Init();

        public void Save()
        {
            if (Name == "Default")
            {
                throw new ArgumentException("デフォルトは保存できません");
            }
            Init();
            Write(this);
        }

        private void Write(Preset preset)
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            File.WriteAllText(FilePath, serializer.Serialize(preset), Encoding.UTF8);
        }

        public static List<Preset> GetPresets(PresetTypes type, PageViewModel viewModel, object? def)
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            var list = Directory.GetFiles(DirectoryPath, $"{type}_*.yaml")
                .Select(file => Path.GetFileNameWithoutExtension(file).Replace($"{type}_", ""))
                .Select(name => GetPreset(type, viewModel, name))
                .ToList();
            var latest = list.FirstOrDefault(preset => preset.Name == "Latest");
            if (latest != null)
            {
                list.Remove(latest);
                list.Insert(0, latest);
                LatestPresets.Add(latest);
            }

            if (def is PresetItemViewModel vm && vm.Preset != null)
            {
                list.Insert(0, vm.Preset);
            }
            else
            {
                list.Insert(0, GetPreset(type, viewModel, "Default"));
            }

            return list;
        }
        public static Preset GetPreset(PresetTypes type, PageViewModel viewModel, string name)
        {
            switch (type)
            {
                case PresetTypes.Round:
                    return new RoundPreset((viewModel as RoundViewModel)!, name);
                case PresetTypes.Backup:
                    return new BackupPreset((viewModel as BackupViewModel)!, name);
                case PresetTypes.ParamShift:
                    return new ParamShiftPreset((viewModel as ParamShiftViewModel)!, name);
                case PresetTypes.Preutter:
                    return new PreutterPreset((viewModel as PreutterViewModel)!, name);
                case PresetTypes.AddOtos:
                    return new AddOtosPreset((viewModel as AddOtosViewModel)!, name);
                case PresetTypes.AddVC:
                    return new AddVCPreset((viewModel as AddVCViewModel)!, name);
                case PresetTypes.AddPreC:
                    return new AddPreCPreset((viewModel as AddPreCViewModel)!, name);
                case PresetTypes.RemoveOtos:
                    return new RemoveOtosPreset((viewModel as RemoveOtosViewModel)!, name);
                case PresetTypes.Duplicate:
                    return new DuplicatePreset((viewModel as DuplicateViewModel)!, name);
                case PresetTypes.ReplaceAlias:
                    return new ReplaceAliasPreset((viewModel as ReplaceAliasViewModel)!, name);
                case PresetTypes.AddSuffixAlias:
                    return new AddSuffixAliasPreset((viewModel as AddSuffixAliasViewModel)!, name);
                case PresetTypes.Ust:
                    return new UstPreset((viewModel as UstViewModel)!, name);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static List<Preset> LatestPresets { get; } = new List<Preset>();
        public static void SaveLatests()
        {
            LatestPresets.ForEach(preset => preset.Save());
        }

        public override string ToString()
        {
            return $"PresetType: {PresetType}, Name: {Name}";
        }
    }

    public enum PresetTypes
    {
        Backup,
        Round,
        ParamShift,
        Preutter,
        AddOtos,
        AddVC,
        AddPreC,
        RemoveOtos,
        Duplicate,
        ReplaceAlias,
        AddSuffixAlias,
        Ust
    }
}
