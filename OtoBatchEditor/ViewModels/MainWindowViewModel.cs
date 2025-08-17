using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Threading;
using DialogHostAvalonia;
using Material.Styles.Controls;
using Material.Styles.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive] public bool IsRightDrawerOpen { get; set; } = false;
        [Reactive] public int DrawerSelectedIndex { get; set; } = -1;
        public int PageIndex { get => DrawerSelectedIndex + 1; }

        [Reactive] public bool IsDialogOpen { get; set; } = false;
        public string[] RightPages { get; } =
        [
            "一般：バックアップを作成",
            "数値：四捨五入",
            "数値：パラメータをまとめて横にずらす",
            "数値：先行発声とオーバーラップを調整",
            "追加：語尾・母音結合・を等を追加",
            "追加：連続音にVCを追加",
            "追加：先頭子音 - C を追加",
            "削除：不要な行を検索して削除",
            "削除：重複エイリアスの確認と削除",
            "文字：エイリアス前後のスペースを削除",
            "文字：エイリアスを変換",
            "文字：エイリアス末尾に音階名などを追加",
            "文字：wav名に_をつける",
            "文字：サイレント文字化け（濁点分離）を修正",
            "その他：エラーチェック",
            "その他：原音設定チェック用ustを作成"
        ];
        [Reactive] public bool LeftDrawerForceClose { get; set; } = false;

        public MainWindowViewModel()
        {
            this.WhenAnyValue(x => x.DrawerSelectedIndex)
                .Subscribe(x =>
                {
                    this.RaisePropertyChanged(nameof(PageIndex));
                    IsRightDrawerOpen = false;
                });
            this.WhenAnyValue(x => x.LeftDrawerForceClose)
                .Subscribe(x =>
                {
                    LeftDrawerForceClose = false;
                });

            // 初回起動時、プリセットをコピー
            try
            {
                var defPrePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, "Presets");
                if (!Directory.Exists(defPrePath))
                {
                    Directory.CreateDirectory(defPrePath);
                }
                var list = Directory.GetFiles(defPrePath, $"*.yaml");
                if (!Directory.Exists(Preset.DirectoryPath))
                {
                    Directory.CreateDirectory(Preset.DirectoryPath);
                }
                foreach (var file in list)
                {
                    var path = Path.Combine(Preset.DirectoryPath, Path.GetFileName(file));
                    if (!File.Exists(path))
                    {
                        File.Copy(file, path);
                    }
                }
            }
            catch (Exception ex)
            {
                DebagMode.AddError(ex);
            }

            var resources = Application.Current?.Resources;
            var theme = Application.Current?.ActualThemeVariant;
            if (theme == Avalonia.Styling.ThemeVariant.Dark)
            {
                resources["CardBackGroundBrush"] = new SolidColorBrush(Color.Parse("#008ba3"));
            }
        }

        public static IClipboard GetClipboard()
        {
            var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return TopLevel.GetTopLevel(lifetime!.MainWindow)!.Clipboard!;
        }

        // Drawer
        public void ToggleDrawer()
        {
            IsRightDrawerOpen = !IsRightDrawerOpen;
        }

        // Dialog
        public static async Task MessageDialogOpen(string text)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await DialogHost.Show(new MessageDialog(text));
                await Task.Delay(100); // 閉じるの待たないと次のダイアログが出ない
            });
        }
        public static async Task<bool> MessageDialogOpen(string text, string cancelText)
        {
            return await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var content = new MessageDialog(text, cancelText);
                await DialogHost.Show(content);
                await Task.Delay(100);
                return content.Execute;
            });
        }
        public static async Task<bool> MessageDialogOpen(string text, string okText, string cancelText)
        {
            return await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var content = new MessageDialog(text, okText, cancelText);
                await DialogHost.Show(content);
                await Task.Delay(100);
                return content.Execute;
            });
        }
        public static async Task DialogOpen(object content)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await DialogHost.Show(content);
                await Task.Delay(100);
            });
        }

        // Snackbar
        public static void ShowSnackbar(string text, long time = 3)
        {
            SnackbarHost.Post(
                new SnackbarModel(text, TimeSpan.FromSeconds(time)),
                "MainWindowSnackbar",
                DispatcherPriority.Normal);
        }
    }
}
