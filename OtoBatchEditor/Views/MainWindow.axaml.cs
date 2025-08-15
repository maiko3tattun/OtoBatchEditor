using Avalonia.Controls;
using Avalonia.Input;
using OtoBatchEditor.ViewModels;
using System;

namespace OtoBatchEditor.Views
{
    public partial class MainWindow : Window
    {
        private static MainWindow? instance;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
        }

        public static void SetProgressIcon(bool visible)
        {
            instance!.ProgressIcon.IsVisible = visible;
        }

        private void WhenClosing(object? sender, WindowClosingEventArgs e)
        {
            try
            {
                Preset.SaveLatests();
            }
            catch (Exception ex)
            {
                DebagMode.AddError(ex);
#if DEBUG
                MainWindowViewModel.ShowSnackbar("前回値の保存に失敗しました");
                e.Cancel = true;
#endif
            }
        }

        private async void MainWindowsKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Alt) && e.Key == Key.D)
            {
                DebagMode.ToggleLogExport();
                if (DebagMode.DebagModeIsEnable)
                {
                    MainWindowViewModel.ShowSnackbar("実行後、不具合報告用のログファイルを出力します");
                }
                else
                {
                    MainWindowViewModel.ShowSnackbar("ログファイル出力をオフにしました");
                }
                e.Handled = true;
            }
            else if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift) && e.Key == Key.D)
            {
                var result = await MainWindowViewModel.MessageDialogOpen("不具合報告用のログファイルを出力します。\nこのモードはエラーが起きたあとに起動してください。", "キャンセル");
                if (result)
                {
                    await DebagMode.Export(LogOutputType.Manual);
                }
                e.Handled = true;
            }
        }
    }
}