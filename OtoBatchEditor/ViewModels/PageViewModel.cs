using OtoBatchEditor.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class PageViewModel : ViewModelBase
    {
        public List<OtoIni> Completed = new List<OtoIni>();
        public List<OtoIni> NotChanged = new List<OtoIni>();

        public async Task Edit(Func<OtoIni, Task<bool>> func, bool ignoreErrors = false)
        {
            MainWindow.SetProgressIcon(true);
            try
            {
                Completed.Clear();
                NotChanged.Clear();
                var list = OtoIni.GetOtoIniList();
                foreach (var otoIni in list)
                {
                    try
                    {
                        if (!otoIni.Read() && !ignoreErrors)
                        {
                            string text = $"{otoIni.FilePath} には不備がある可能性があります。続行しますか？\n（詳細はエラーチェック機能で確認できます）";
                            var result = await MainWindowViewModel.MessageDialogOpen(text, "中止");
                            if (!result)
                            {
                                break;
                            }
                        }


                        var write = await Task.Run(() => func(otoIni));

                        if (write && otoIni.Write())
                        {
                            Completed.Add(otoIni);
                        }
                        else
                        {
                            NotChanged.Add(otoIni);
                        }
                    }
                    catch (MinorException e)
                    {
                        await MainWindowViewModel.MessageDialogOpen(e.Message);
                        continue;
                    }
                    catch (Exception e)
                    {
                        DebugMode.AddError(e);
                        var result = await MainWindowViewModel.MessageDialogOpen(e.Message, "スキップ", "中止");
                        if (result)
                        {
                            continue;
                        }
                        break;
                    }
                }
            }
            catch (MinorException e)
            {
                MainWindow.SetProgressIcon(false);
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                await DebugMode.Export(LogOutputType.Error);
                return;
            }
            catch (Exception e)
            {
                MainWindow.SetProgressIcon(false);
                DebugMode.AddError(e);
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                await DebugMode.Export(LogOutputType.Error);
                return;
            }

            MainWindow.SetProgressIcon(false);
            await WhenCompleted();
            await DebugMode.Export(LogOutputType.Completed);
        }

        public async Task WhenCompleted()
        {
            List<string> text = new List<string>();
            if (Completed.Count > 0)
            {
                text.Add("完了：");
                foreach (var otoIni in Completed)
                {
                    text.Add(otoIni.FilePath);
                }
            }
            if (Completed.Count > 0 && NotChanged.Count > 0)
            {
                text.Add(string.Empty);
            }
            if (NotChanged.Count > 0)
            {
                text.Add("以下のファイルは変更がありませんでした：");
                foreach (var otoIni in NotChanged)
                {
                    text.Add(otoIni.FilePath);
                }
            }
            if (text.Count == 0)
            {
                text.Add("変更されたファイルはありませんでした。");
            }
            await MainWindowViewModel.MessageDialogOpen(string.Join('\n', text));
        }

        public async Task Open(Func<OtoIni, Task> func, string completeMessage, bool ignoreErrors = false)
        {
            MainWindow.SetProgressIcon(true);
            try
            {
                var list = OtoIni.GetOtoIniList();
                foreach (var otoIni in list)
                {
                    try
                    {
                        if (!otoIni.Read() && !ignoreErrors)
                        {
                            string text = $"{otoIni.FilePath} には不備がある可能性があります。続行しますか？\n（詳細はエラーチェック機能で確認できます）";
                            var result = await MainWindowViewModel.MessageDialogOpen(text, "中止");
                            if (!result)
                            {
                                break;
                            }
                        }

                        await func(otoIni);
                    }
                    catch (MinorException e)
                    {
                        await MainWindowViewModel.MessageDialogOpen(e.Message);
                        continue;
                    }
                    catch (Exception e)
                    {
                        DebugMode.AddError(e);
                        var result = await MainWindowViewModel.MessageDialogOpen(e.Message, "スキップ", "中止");
                        if (result)
                        {
                            continue;
                        }
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(completeMessage))
                {
                    await MainWindowViewModel.MessageDialogOpen(completeMessage);
                }
            }
            catch (MinorException e)
            {
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                MainWindow.SetProgressIcon(false);
                await DebugMode.Export(LogOutputType.Error);
                return;
            }
            catch (Exception e)
            {
                DebugMode.AddError(e);
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                MainWindow.SetProgressIcon(false);
                await DebugMode.Export(LogOutputType.Error);
                return;
            }
            MainWindow.SetProgressIcon(false);
            await DebugMode.Export(LogOutputType.Completed);
        }

        public async Task<OtoIni[]?> TryGetInis(bool ignoreErrors = false)
        {
            try
            {
                var list = OtoIni.GetOtoIniList();
                foreach (var otoIni in list)
                {
                    if (!otoIni.Read() && !ignoreErrors)
                    {
                        string text = $"{otoIni.FilePath} には不備がある可能性があります。続行しますか？\n（詳細はエラーチェック機能で確認できます）";
                        var result = await MainWindowViewModel.MessageDialogOpen(text, "中止");
                        if (!result)
                        {
                            return null;
                        }
                    }
                }
                return list;
            }
            catch (MinorException e)
            {
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                return null;
            }
            catch (Exception e)
            {
                DebugMode.AddError(e);
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                return null;
            }
        }
    }
}
