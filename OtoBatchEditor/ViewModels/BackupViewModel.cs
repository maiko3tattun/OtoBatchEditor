using ReactiveUI.Fody.Helpers;
using System;

namespace OtoBatchEditor.ViewModels
{
    public class BackupViewModel : PageViewModel
    {
        [Reactive] public int NameIndex { get; set; } = 0;
        [Reactive] public bool OverWrite { get; set; } = false;

        public BackupViewModel() { }
        
        public async void OK()
        {
            try
            {
                var list = OtoIni.GetOtoIniList();
                string name;
                switch (NameIndex)
                {
                    case 1:
                        name = "oto_backup.ini";
                        break;
                    case 2:
                        string now = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        name = $"oto_{now}.ini";
                        break;
                    default:
                        name = "oto_original.ini";
                        break;
                }
                foreach (var otoIni in list)
                {
                    try
                    {
                        otoIni.Copy(OverWrite, name);
                    }
                    catch (MinorException e)
                    {
                        await MainWindowViewModel.MessageDialogOpen(e.Message);
                        continue;
                    }
                    catch (Exception e)
                    {
                        var result = await MainWindowViewModel.MessageDialogOpen(e.Message, "続行", "中止");
                        if (result)
                        {
                            continue;
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                DebugMode.AddError(e);
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                await DebugMode.Export(LogOutputType.Error);
            }
            MainWindowViewModel.ShowSnackbar("完了！");
            await DebugMode.Export(LogOutputType.Completed);
        }
    }
}
