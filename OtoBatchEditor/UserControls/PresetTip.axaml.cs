using Avalonia.Controls;
using OtoBatchEditor.ViewModels;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OtoBatchEditor;

public partial class PresetTip : UserControl
{
    public PresetTip()
    {
        InitializeComponent();
        menuButton = button;
    }

    private static DropDownButton? menuButton;
    public static void MenuClose()
    {
        menuButton?.Flyout?.Hide(); // Todo なんで動かない？？
    }
}

public class PresetTipViewModel : ViewModelBase
{
    public PresetTypes PresetType { get; }
    public ObservableCollection<object> PresetList { get; private set; } = new ObservableCollection<object>();

    public PresetTipViewModel(PresetTypes type, PageViewModel pageViewModel)
    {
        this.PresetType = type;
        InitPreset(pageViewModel);
    }

    public void InitPreset(PageViewModel pageViewModel)
    {
        var presets = Preset.GetPresets(PresetType, pageViewModel, PresetList.FirstOrDefault())
            .Select(preset => new PresetItemViewModel(preset));
        PresetList = new ObservableCollection<object>(presets);

        PresetList.Add(new Separator());
        PresetList.Add(new PresetItemViewModel("保存", ReactiveCommand.Create(async () =>
        {
            PresetTip.MenuClose();
            var content = new InputDialog("名前：", "キャンセル");
            await MainWindowViewModel.DialogOpen(content);
            if (content.Execute)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(content.Text) || content.Text == "Default" || content.Text == "Latest")
                    {
                        MainWindowViewModel.ShowSnackbar("使用できないプリセット名です");
                        return;
                    }
                    var text = "プリセットを保存しました";
                    if (PresetList.Any(preset => preset is PresetItemViewModel vm && content.Text == vm.PresetName))
                    {
                        text = "プリセットを上書き保存しました";
                    }
                    var preset = Preset.GetPreset(PresetType, pageViewModel, content.Text);
                    preset.Save();
                    InitPreset(pageViewModel);
                    MainWindowViewModel.ShowSnackbar(text);
                }
                catch (Exception e)
                {
                    DebugMode.AddError(e);
                    MainWindowViewModel.ShowSnackbar("プリセットの保存に失敗しました");
                }
            }
        })));
        PresetList.Add(new PresetItemViewModel("フォルダを開く", ReactiveCommand.Create(() =>
        {
            try
            {
                if (Directory.Exists(Preset.DirectoryPath))
                {
                    if (OperatingSystem.IsWindows())
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer",
                            Arguments = $"\"{Preset.DirectoryPath}\"",
                            UseShellExecute = true
                        });
                    }
                    else if (OperatingSystem.IsMacOS())
                    {
                        Process.Start("open", $"\"{Preset.DirectoryPath}\"");
                    }
                }
            }
            catch (Exception e)
            {
                DebugMode.AddError(e);
                MainWindowViewModel.ShowSnackbar("フォルダを開けませんでした");
            }
            PresetTip.MenuClose();
            return Task.CompletedTask;
        })));
    }
}

public class PresetItemViewModel : ViewModelBase
{
    public Preset? Preset { get; }
    public string? PresetName { get; }
    public string DisplayName { get; }
    public ICommand? Command { get; }

    public PresetItemViewModel(Preset preset)
    {
        this.Preset = preset;
        this.PresetName = preset.Name;
        this.DisplayName = preset.DisplayName;
        this.Command = ReactiveCommand.Create(() =>
        {
            try
            {
                preset.Load();
                MainWindowViewModel.ShowSnackbar("プリセットを読み込みました");
            }
            catch (Exception e)
            {
                DebugMode.AddError(e);
                MainWindowViewModel.ShowSnackbar("プリセットの読み込みに失敗しました");
            }
            PresetTip.MenuClose();
        });
    }
    public PresetItemViewModel(string name, ReactiveCommand<Unit, Task> command)
    {
        this.DisplayName = name;
        this.Command = command;
    }
}