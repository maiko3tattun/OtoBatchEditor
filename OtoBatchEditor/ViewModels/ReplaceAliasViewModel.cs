using DynamicData.Aggregation;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace OtoBatchEditor.ViewModels
{
    public class ReplaceAliasViewModel : PageViewModel
    {
        public static ReplaceAliasViewModel? Instance;

        [Reactive] public ObservableCollectionExtended<ReplaceItem> ReplaceItems { get; set; } = new ObservableCollectionExtended<ReplaceItem>();
        [Reactive] public bool IsRegex { get; set; } = false;
        [Reactive] public bool NoConvertSuffix { get; set; } = false;
        [Reactive] public string TestText { get; set; } = "あ_C4";
        [Reactive] public string TestSuffix { get; set; } = "_C4";
        [Reactive] public string TestResult { get; set; } = string.Empty;

        public ReplaceAliasViewModel()
        {
            this.WhenAnyValue(x => x.TestText, x => x.TestSuffix)
                .Subscribe(x =>
                {
                    ReplaceTest();
                });
            ReplaceItems.Add(new ReplaceItem());

            Instance = this;
        }

        public async void OK()
        {
            if (ReplaceItems.Count == 0 || ReplaceItems.All(item => !item.IsChecked))
            {
                await MainWindowViewModel.MessageDialogOpen("変換するものがありません");
                return;
            }

            if (IsRegex) // 正規表現できるか確認
            {
                try
                {
                    foreach (var item in ReplaceItems)
                    {
                        var test = new Regex(item.Before.Replace("[Suffix]", string.Empty));
                    }
                }
                catch
                {
                    await MainWindowViewModel.MessageDialogOpen("Beforeに正規表現で使えない文字が入っています（\\など）");
                    return;
                }
            }

            await Edit(otoIni =>
            {
                try
                {
                    if (IsRegex)
                    {
                        foreach (var item in ReplaceItems)
                        {
                            if (item.IsChecked && !string.IsNullOrEmpty(item.Before))
                            {
                                int count = item.Before.Count(c => c == '(');
                                Regex regex = new Regex(item.Before.Replace("[Suffix]", $"({otoIni.Suffix})"));
                                string after = item.After.Replace("[Suffix]", $"(${count + 1})");

                                otoIni.OtoList.ForEach(oto =>
                                {
                                    if (NoConvertSuffix)
                                    {
                                        if (item.Before.Contains("[Suffix]"))
                                        {
                                            oto.Alias = regex.Replace(oto.Alias, after);
                                            oto.Alias = oto.Alias.Replace($"({otoIni.Suffix})", otoIni.Suffix);
                                        }
                                        else
                                        {
                                            oto.Alias = oto.Alias.Replace(otoIni.Suffix, "");
                                            oto.Alias = regex.Replace(oto.Alias, after);
                                            oto.Alias += otoIni.Suffix;
                                        }
                                    }
                                    else
                                    {
                                        oto.Alias = regex.Replace(oto.Alias, after);
                                        oto.Alias = oto.Alias.Replace($"({otoIni.Suffix})", otoIni.Suffix);
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in ReplaceItems)
                        {
                            if (item.IsChecked && !string.IsNullOrEmpty(item.Before))
                            {
                                string before = item.Before.Replace("[Suffix]", otoIni.Suffix);
                                string after = item.After.Replace("[Suffix]", otoIni.Suffix);

                                otoIni.OtoList.ForEach(oto =>
                                {
                                    if (NoConvertSuffix)
                                    {
                                        if (item.Before.Contains("[Suffix]"))
                                        {
                                            oto.Alias = oto.Alias.Replace(before, after);
                                        }
                                        else
                                        {
                                            oto.Alias = oto.Alias.Replace(otoIni.Suffix, "");
                                            oto.Alias = oto.Alias.Replace(before, after);
                                            oto.Alias += otoIni.Suffix;
                                        }
                                    }
                                    else
                                    {
                                        oto.Alias = oto.Alias.Replace(before, after);
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                }
                return Task.FromResult(true);
            });
        }

        public async Task CopyToClipboard()
        {
            try
            {
                await MainWindowViewModel.GetClipboard().SetTextAsync("[Suffix]");
                MainWindowViewModel.ShowSnackbar("クリップボードにコピーしました");
            }
            catch (Exception e)
            {
                DebagMode.AddError(e);
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                return;
            }
        }

        public void AddRow()
        {
            ReplaceItems.Add(new ReplaceItem());
        }

        public void DeleteRow(ReplaceItem item)
        {
            ReplaceItems.Remove(item);
        }

        public void ReplaceTest()
        {
            var text = TestText;
            if (ReplaceItems.Count == 0 || ReplaceItems.All(item => !item.IsChecked))
            {
                TestResult = TestText;
                return;
            }
            if (IsRegex) // 正規表現できるか確認
            {
                try
                {
                    foreach (var item in ReplaceItems)
                    {
                        var test = new Regex(item.Before.Replace("[Suffix]", string.Empty));
                    }
                }
                catch
                {
                    TestResult = "Beforeに正規表現で使えない文字が入っています（\\など）";
                    return;
                }
            }
            try
            {
                if (IsRegex)
                {
                    foreach (var item in ReplaceItems)
                    {
                        if (item.IsChecked && !string.IsNullOrEmpty(item.Before))
                        {
                            int count = item.Before.Count(c => c == '(');
                            Regex regex = new Regex(item.Before.Replace("[Suffix]", $"({TestSuffix})"));
                            string after = item.After.Replace("[Suffix]", $"(${count + 1})");

                            if (NoConvertSuffix)
                            {
                                if (item.Before.Contains("[Suffix]"))
                                {
                                    text = regex.Replace(text, after);
                                    text = text.Replace($"({TestSuffix})", TestSuffix);
                                }
                                else
                                {
                                    text = text.Replace(TestSuffix, "");
                                    text = regex.Replace(text, after);
                                    text += TestSuffix;
                                }
                            }
                            else
                            {
                                text = regex.Replace(text, after);
                                text = text.Replace($"({TestSuffix})", TestSuffix);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in ReplaceItems)
                    {
                        if (item.IsChecked && !string.IsNullOrEmpty(item.Before))
                        {
                            string before = item.Before.Replace("[Suffix]", TestSuffix);
                            string after = item.After.Replace("[Suffix]", TestSuffix);

                            if (NoConvertSuffix)
                            {
                                if (item.Before.Contains("[Suffix]"))
                                {
                                    text = text.Replace(before, after);
                                }
                                else
                                {
                                    text = text.Replace(TestSuffix, "");
                                    text = text.Replace(before, after);
                                    text += TestSuffix;
                                }
                            }
                            else
                            {
                                text = text.Replace(before, after);
                            }
                        }
                    }
                }
            }
            catch
            {
                TestResult = "変換に失敗しました";
                return;
            }
            
            TestResult = text;
        }
    }

    public class ReplaceItem
    {
        [Reactive] public bool IsChecked { get; set; } = true;
        [Reactive] public string Before { get; set; } = string.Empty;
        [Reactive] public string After { get; set; } = string.Empty;
        [YamlIgnore] public ReplaceAliasViewModel? Parent => ReplaceAliasViewModel.Instance;

        public ReplaceItem() { }
        public ReplaceItem(bool isChecked, string before, string after)
        {
            IsChecked = isChecked;
            Before = before;
            After = after;
        }
    }
}
