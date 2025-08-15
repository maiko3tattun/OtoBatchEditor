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
                MainWindowViewModel.ShowSnackbar("�O��l�̕ۑ��Ɏ��s���܂���");
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
                    MainWindowViewModel.ShowSnackbar("���s��A�s��񍐗p�̃��O�t�@�C�����o�͂��܂�");
                }
                else
                {
                    MainWindowViewModel.ShowSnackbar("���O�t�@�C���o�͂��I�t�ɂ��܂���");
                }
                e.Handled = true;
            }
            else if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift) && e.Key == Key.D)
            {
                var result = await MainWindowViewModel.MessageDialogOpen("�s��񍐗p�̃��O�t�@�C�����o�͂��܂��B\n���̃��[�h�̓G���[���N�������ƂɋN�����Ă��������B", "�L�����Z��");
                if (result)
                {
                    await DebagMode.Export(LogOutputType.Manual);
                }
                e.Handled = true;
            }
        }
    }
}