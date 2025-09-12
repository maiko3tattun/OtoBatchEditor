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
                                GetRegexVariables(item, otoIni.Suffix, out Regex regex, out string after);
                                otoIni.OtoList.ForEach(oto =>
                                {
                                    oto.Alias = ConvertRegex(oto.Alias, item.Before, otoIni.Suffix, regex, after);
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
                                GetVariables(item, otoIni.Suffix, out string before, out string after);
                                otoIni.OtoList.ForEach(oto =>
                                {
                                    oto.Alias = ConvertString(oto.Alias, before, after, otoIni.Suffix);
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
                            GetRegexVariables(item, TestSuffix, out Regex regex, out string after);
                            text = ConvertRegex(text, item.Before, TestSuffix, regex, after);
                        }
                    }
                }
                else
                {
                    foreach (var item in ReplaceItems)
                    {
                        if (item.IsChecked && !string.IsNullOrEmpty(item.Before))
                        {
                            GetVariables(item, TestSuffix, out string before, out string after);
                            text = ConvertString(text, before, after, TestSuffix);
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

        #region Convert
        private void GetRegexVariables(ReplaceItem item, string suffix, out Regex regex, out string after)
        {
            int count = item.Before.Count(c => c == '(');
            regex = new Regex(item.Before.Replace("[Suffix]", $"({suffix})"));
            after = item.After.Replace("[Suffix]", $"(${count + 1})");
        }

        private string ConvertRegex(string input, string before, string suffix, Regex regex, string after)
        {
            if (NoConvertSuffix)
            {
                if (before.Contains("[Suffix]"))
                {
                    input = ReplaceWithCase(input, regex, after);
                    input = input.Replace($"({suffix})", suffix);
                }
                else
                {
                    if (input.EndsWith(suffix))
                    {
                        input = input.Replace(suffix, "");
                        input = ReplaceWithCase(input, regex, after);
                        input += suffix;
                    }
                    else
                    {
                        input = ReplaceWithCase(input, regex, after);
                    }
                }
            }
            else
            {
                input = ReplaceWithCase(input, regex, after);
                input = input.Replace($"({suffix})", suffix);
            }
            return input;
        }

        // after内の\u,\l等に対応
        private string ReplaceWithCase(string input, Regex regex, string after)
        {
            return regex.Replace(input, match =>
            {
                return Regex.Replace(after, @"(\\[uUlL])?\$(\d+)", m =>
                {
                    string directive = m.Groups[1].Value;
                    int groupIndex = int.Parse(m.Groups[2].Value);
                    if (groupIndex >= match.Groups.Count) return "";
                    string value = match.Groups[groupIndex].Value;

                    return directive switch
                    {
                        @"\U" => value.ToUpper(),
                        @"\u" => value.Length > 0 ? char.ToUpper(value[0]) + value.Substring(1) : value,
                        @"\L" => value.ToLower(),
                        @"\l" => value.Length > 0 ? char.ToLower(value[0]) + value.Substring(1) : value,
                        _ => value
                    };
                });
            });
        }

        private void GetVariables(ReplaceItem item, string suffix, out string before, out string after)
        {
            before = item.Before.Replace("[Suffix]", suffix);
            after = item.After.Replace("[Suffix]", suffix);
        }

        private string ConvertString(string input, string before, string after, string suffix)
        {
            if (NoConvertSuffix)
            {
                if (before.Contains("[Suffix]"))
                {
                    input = input.Replace(before, after);
                }
                else
                {

                    if (input.EndsWith(suffix))
                    {
                        input = input.Replace(suffix, "");
                        input = input.Replace(before, after);
                        input += suffix;
                    }
                    else
                    {
                        input = input.Replace(before, after);
                    }
                }
            }
            else
            {
                input = input.Replace(before, after);
            }
            return input;
        }
        #endregion

        #region Others
        public async Task CopyToClipboard()
        {
            try
            {
                await MainWindowViewModel.GetClipboard().SetTextAsync("[Suffix]");
                MainWindowViewModel.ShowSnackbar("クリップボードにコピーしました");
            }
            catch (Exception e)
            {
                DebugMode.AddError(e);
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
        #endregion
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
