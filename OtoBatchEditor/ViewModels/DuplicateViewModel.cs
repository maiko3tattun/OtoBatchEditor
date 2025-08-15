using OtoBatchEditor.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class DuplicateViewModel : PageViewModel
    {
        [Reactive] public bool Each { get; set; } = true;
        [Reactive] public string LeaveCount { get; set; } = "1";
        [Reactive] public bool AddNum { get; set; } = true;

        public DuplicateViewModel()
        {
            this.WhenAnyValue(x => x.LeaveCount)
                .Subscribe(value =>
                {
                    LeaveCount = NumValidation.IntValidation(value, 1, 1, 100, out bool valid).ToString();
                });
        }

        public async void Search()
        {
            try
            {
                var inis = await TryGetInis();
                if (inis == null)
                {
                    return;
                }
                if (Each)
                {
                    foreach (var ini in inis)
                    {
                        var groups = GetDupliGroups(ini);
                        if (groups.Count == 0)
                        {
                            await MainWindowViewModel.MessageDialogOpen($"{ini.FilePath} に重複エイリアスはありませんでした");
                            continue;
                        }
                        var list = new List<string>();
                        list.Add($"{ini.FilePath} の重複エイリアス");
                        foreach (var group in groups)
                        {
                            list.Add($"{group.Key}：\n{string.Join("\n", group.Items.Select(item => $"line{item.Oto.LineNumber}：\"{item.Oto}\""))}");
                        }
                        await MainWindowViewModel.MessageDialogOpen(string.Join("\n\n", list));
                    }
                }
                else
                {
                    var groups = GetDupliGroups(inis);
                    if (groups.Count == 0)
                    {
                        await MainWindowViewModel.MessageDialogOpen($"重複エイリアスはありませんでした");
                        return;
                    }

                    var list = new List<string>();
                    foreach (var group in groups)
                    {
                        list.Add($"{group.Key}：\n{string.Join("\n", group.Items.Select(item => $"{item.DirectorName} line{item.Oto.LineNumber}：\"{item.Oto}\""))}");
                    }
                    await MainWindowViewModel.MessageDialogOpen(string.Join("\n\n", list));
                }
            }
            catch (Exception e)
            {
                DebagMode.AddError(e);
                await MainWindowViewModel.MessageDialogOpen($"予期せぬエラーが発生しました\n{e.Message}");
                await DebagMode.Export(LogOutputType.Error);
            }
            await DebagMode.Export(LogOutputType.Completed);
        }

        public async void Remove()
        {
            var leave = NumValidation.IntValidation(LeaveCount, 1, 1, 100, out bool valid);
            if (!valid)
            {
                await MainWindowViewModel.MessageDialogOpen("残す個数には0以上の整数を入力してください");
                return;
            }

            MainWindow.SetProgressIcon(true);
            if (Each)
            {
                await Edit(async otoIni =>
                {
                    try
                    {
                        var groups = GetDupliGroups(otoIni);
                        if (groups.Count == 0)
                        {
                            await MainWindowViewModel.MessageDialogOpen($"{otoIni.FilePath} に重複エイリアスはありませんでした");
                            return false;
                        }
                        ProcessGroup(groups, leave, otoIni.OtoList);

                        // 確認を出す
                        var content = new DupliDialog(groups);
                        await MainWindowViewModel.DialogOpen(content);
                        if (content.Cancel)
                        {
                            return false;
                        }

                        RemoveOtos(otoIni, groups);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                    }
                    return true;
                });
            }
            else
            {
                try
                {
                    var inis = await TryGetInis();
                    if (inis == null)
                    {
                        MainWindow.SetProgressIcon(false);
                        return;
                    }
                    var groups = GetDupliGroups(inis);
                    if (groups.Count == 0)
                    {
                        MainWindow.SetProgressIcon(false);
                        await MainWindowViewModel.MessageDialogOpen($"重複エイリアスはありませんでした");
                        return;
                    }
                    ProcessGroup(groups, leave, inis.SelectMany(ini => ini.OtoList));

                    // 確認を出す
                    var content = new DupliDialog(groups);
                    await MainWindowViewModel.DialogOpen(content);
                    if (content.Cancel)
                    {
                        MainWindow.SetProgressIcon(false);
                        return;
                    }

                    await Edit(otoIni =>
                    {
                        try
                        {
                            RemoveOtos(otoIni, groups);
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                        }
                        return Task.FromResult(true);
                    }, true);
                }
                catch (Exception e)
                {
                    DebagMode.AddError(e);
                    MainWindow.SetProgressIcon(false);
                    await MainWindowViewModel.MessageDialogOpen($"予期せぬエラーが発生しました\n{e.Message}");
                    return;
                }
            }
            MainWindow.SetProgressIcon(false);
        }

        public async void RemoveNum()
        {
            var leave = NumValidation.IntValidation(LeaveCount, 1, 1, 100, out bool valid);
            if (!valid)
            {
                await MainWindowViewModel.MessageDialogOpen("残す個数には0以上の整数を入力してください");
                return;
            }

            await Edit(async otoIni =>
            {
                try
                {
                    List<Oto> removeList = new List<Oto>();
                    otoIni.OtoList.ForEach(oto =>
                    {
                        var alias = oto.Alias.Replace(otoIni.Suffix, "");
                        var match = Regex.Match(alias, @"[0-9]+$");
                        if (match.Success && !Regex.IsMatch(alias, " [0-9]"))
                        {
                            if (int.TryParse(match.Value, out int num) && num > leave)
                            {
                                removeList.Add(oto);
                            }
                        }
                    });

                    if (removeList.Count > 0)
                    {
                        var remove = string.Join("\n", removeList.Select(oto => oto.Alias));
                        string text = $"{otoIni.FilePath}から\n{remove}\nを削除します";

                        var result = await MainWindowViewModel.MessageDialogOpen(text, "スキップ");
                        if (result)
                        {
                            otoIni.RemoveAll(oto => removeList.Contains(oto));
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                }
                return true;
            });
        }

        private List<DuplicateItemGroup> GetDupliGroups(OtoIni otoIni)
        {
            return GetDupliGroups(otoIni.OtoList.Select(oto => new DuplicateItem(otoIni, oto)));
        }
        private List<DuplicateItemGroup> GetDupliGroups(OtoIni[] inis)
        {
            return GetDupliGroups(inis.SelectMany(ini => ini.OtoList.Select(oto => new DuplicateItem(ini, oto))));
        }
        private List<DuplicateItemGroup> GetDupliGroups(IEnumerable<DuplicateItem> list)
        {
            return list.GroupBy(item => item.Oto.Alias)
                .Where(group => group.Count() >= 2)
                .Select(group => new DuplicateItemGroup(group))
                .ToList();
        }

        private void ProcessGroup(List<DuplicateItemGroup> groups, int leave, IEnumerable<Oto> otos)
        {
            groups.ForEach(group =>
            {
                for (int i = 0; i < group.Items.Count; i++)
                {
                    var item = group.Items[i];

                    if (i < leave)
                    {
                        item.Selected = true;
                        if (i > 0 && AddNum)
                        {
                            var newAlias = string.Empty;
                            if (!string.IsNullOrWhiteSpace(item.Suffix) && item.Oto.Alias.EndsWith(item.Suffix))
                            {
                                newAlias = item.Oto.Alias.Replace(item.Suffix, $"{i + 1}{item.Suffix}");
                            }
                            else
                            {
                                newAlias = $"{item.Oto.Alias}{i + 1}";
                            }

                            if (otos.Any(oto => oto.Alias == newAlias))
                            {
                                item.Selected = false;
                            }
                            else
                            {
                                item.NewAlias = newAlias;
                            }
                        }
                    }
                }
            });
        }

        private void RemoveOtos(OtoIni otoIni, List<DuplicateItemGroup> groups)
        {
            var items = groups.SelectMany(group => group.Items);
            List<Oto> removeList = new List<Oto>();
            otoIni.OtoList.ForEach(oto =>
            {
                var item = items.FirstOrDefault(item => item.DirectoryPath == otoIni.DirectoryPath && item.Oto.ToString() == oto.ToString() && item.Oto.LineNumber == oto.LineNumber);
                if (item != null)
                {
                    if (item.Selected)
                    {
                        if (!string.IsNullOrWhiteSpace(item.NewAlias))
                        {
                            oto.Alias = item.NewAlias;
                        }
                    }
                    else
                    {
                        removeList.Add(oto);
                    }
                }
            });
            if (removeList.Count > 0)
            {
                otoIni.RemoveAll(oto => removeList.Contains(oto));
            }
        }
    }

    public class DuplicateItem
    {
        public string DirectoryPath { get; }
        public string DirectorName { get; }
        public Oto Oto { get; }
        public string Suffix { get; }
        public int Score { get; }
        [Reactive] public bool Selected { get; set; } = false;
        [Reactive] public string NewAlias { get; set; }

        public DuplicateItem(OtoIni otoIni, Oto oto)
        {
            DirectoryPath = otoIni.DirectoryPath;
            DirectorName = Path.GetFileName(DirectoryPath);
            Suffix = otoIni.Suffix;
            Oto = oto;
            NewAlias = oto.Alias;
            string wav = Path.GetFileNameWithoutExtension(oto.FileName);
            for (int i = 0; i < wav.Length; i++)
            {
                if (Phoneme.YouonKanas.Contains(wav[i]))
                {
                    Score++;
                }
                else if (Regex.IsMatch(wav[i].ToString(), @"[あいうえお]"))
                {
                    Score--;
                }
            }
        }
    }

    public class DuplicateItemGroup
    {
        public string Key { get; }
        public List<DuplicateItem> Items { get; } = new List<DuplicateItem>();

        public DuplicateItemGroup(IGrouping<string, DuplicateItem> group)
        {
            Key = group.Key;
            Items = group.OrderBy(item => item.Score)
                .ThenBy(item => item.DirectoryPath)
                .ThenBy(item => item.Oto.FileName)
                .ThenBy(item => item.Oto.Offset)
                .ToList();
        }
    }
}
