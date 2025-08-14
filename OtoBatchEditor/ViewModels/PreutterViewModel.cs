using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class PreutterViewModel : PageViewModel
    {
        [Reactive] public bool Preutter { get; set; } = true;
        [Reactive] public bool PreFix { get; set; } = true;
        [Reactive] public string PreValue { get; set; } = "250";
        [Reactive] public bool Overlap { get; set; } = true;
        [Reactive] public string OvlValue { get; set; } = "3";
        [Reactive] public bool OvlRound { get; set; } = true;
        [Reactive] public bool Filter { get; set; } = true;
        [Reactive] public string FilterValue { get; set; } = "[aiueonN] ";

        public PreutterViewModel()
        {
            this.WhenAnyValue(x => x.PreValue)
                .Subscribe(value =>
                {
                    PreValue = NumValidation.DoubleValidation(value, 250, -1000, 1000, out bool valid).ToString();
                });

            this.WhenAnyValue(x => x.OvlValue)
                .Subscribe(value =>
                {
                    OvlValue = NumValidation.DoubleValidation(value, 3, 1, 10, out bool valid).ToString();
                });
        }

        public async void OK()
        {
            double preDouble = NumValidation.DoubleValidation(PreValue, 250, -1000, 1000, out bool valid);
            if (Preutter)
            {
                if (!valid)
                {
                    await MainWindowViewModel.MessageDialogOpen("先行発声の数値を入力してください");
                    return;
                }
                if (PreFix && preDouble <= 0)
                {
                    await MainWindowViewModel.MessageDialogOpen("先行発声は0より大きい数値を入力してください");
                    return;
                }
            }
            double ovlDouble = NumValidation.DoubleValidation(OvlValue, 3, 1, 10, out valid);
            if (Overlap)
            {
                if (!valid)
                {
                    await MainWindowViewModel.MessageDialogOpen("オーバーラップの数値を入力してください");
                    return;
                }
            }
            Regex filter;
            try
            {
                if (Filter)
                {
                    if (string.IsNullOrEmpty(FilterValue))
                    {
                        await MainWindowViewModel.MessageDialogOpen("フィルターに検索文字を入力してください");
                        return;
                    }
                    filter = new Regex(FilterValue);
                }
                else
                {
                    filter = new Regex(string.Empty);
                }
            }
            catch
            {
                await MainWindowViewModel.MessageDialogOpen("フィルターに使用できない文字が含まれています");
                return;
            }

            await Edit(otoIni =>
            {
                try
                {
                    otoIni.OtoList.ForEach(oto =>
                    {
                        if (Filter && !filter.IsMatch(oto.Alias))
                        {
                            return;
                        }

                        if (Preutter)
                        {
                            if (PreFix)
                            {
                                oto.Pre = preDouble;
                            }
                            else
                            {
                                oto.Pre += preDouble;
                            }
                        }
                        if (Overlap)
                        {
                            oto.Ovl = oto.Pre / ovlDouble;
                            if (OvlRound)
                            {
                                oto.Ovl = Math.Round(oto.Ovl);
                            }
                        }
                    });
                }
                catch (Exception e)
                {
                    throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                }
                return Task.FromResult(true);
            });
        }
    }
}
