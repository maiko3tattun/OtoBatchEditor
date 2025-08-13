using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class AddSuffixAliasViewModel : PageViewModel
    {
        [Reactive] public bool IsAll { get; set; } = true;
        [Reactive] public bool SkipEndWith { get; set; } = true;
        [Reactive] public string Append { get; set; } = string.Empty;

        public AddSuffixAliasViewModel() { }

        public async void OK()
        {
            if (IsAll)
            {
                if (string.IsNullOrEmpty(Append))
                {
                    await MainWindowViewModel.MessageDialogOpen("Suffixが空欄です");
                    return;
                }

                await Edit(otoIni =>
                {
                    try
                    {
                        foreach (var oto in otoIni.OtoList)
                        {
                            if (SkipEndWith && oto.Alias.EndsWith(Append))
                            {
                                continue;
                            }
                            oto.Alias += Append;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                    }
                    return Task.FromResult(true);
                });
            }
            else
            {
                await Edit(async otoIni =>
                {
                    try
                    {
                        var content = new InputDialog($"\"{Path.GetFileNameWithoutExtension(otoIni.DirectoryPath)}\"のSuffix", "スキップ", Append);
                        await MainWindowViewModel.DialogOpen(content);
                        if (!content.Execute)
                        {
                            return false;
                        }
                        foreach (var oto in otoIni.OtoList)
                        {
                            if (SkipEndWith && oto.Alias.EndsWith(content.Text))
                            {
                                continue;
                            }
                            oto.Alias += content.Text;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                    }
                    return true;
                });
            }
        }
    }
}
