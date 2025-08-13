using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OtoBatchEditor.ViewModels
{
    public class RemoveOtosViewModel : PageViewModel
    {
        [Reactive] public string SearchText { get; set; } = string.Empty;
        [Reactive] public bool IsRegex { get; set; } = false;

        public RemoveOtosViewModel() { }

        public async void OK()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                await MainWindowViewModel.MessageDialogOpen("検索欄が空です");
                return;
            }

            if (IsRegex) // 正規表現できるか確認
            {
                try
                {
                    var regex = new Regex(SearchText);
                }
                catch
                {
                    await MainWindowViewModel.MessageDialogOpen("検索欄に正規表現で使えない文字が入っています（\\など）");
                    return;
                }
            }

            await Edit(async otoIni =>
            {
                try
                {
                    Oto[] removeList;
                    string search = SearchText.Replace("[APPEND]", otoIni.Suffix);

                    if (IsRegex)
                    {
                        removeList = otoIni.OtoList.Where(oto => Regex.IsMatch(oto.Alias, search)).ToArray();
                    }
                    else
                    {
                        removeList = otoIni.OtoList.Where(oto => oto.Alias.Contains(search)).ToArray();
                    }

                    if (removeList.Length > 0)
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

        public void AddAppend()
        {
            SearchText += "[APPEND]";
        }
    }
}
