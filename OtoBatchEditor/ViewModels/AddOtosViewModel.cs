using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace OtoBatchEditor.ViewModels
{
    public class AddOtosViewModel : PageViewModel
    {
        public static AddOtosViewModel? Instance;

        [Reactive] public bool AddEnding { get; set; } = true;
        [Reactive] public string EndingAlias { get; set; } = "R";
        [Reactive] public bool AddVowel { get; set; } = true;
        [Reactive] public bool AddRentan { get; set; } = false;
        [Reactive] public bool AddDupli { get; set; } = false;
        [Reactive] public bool Overwrite { get; set; } = false;
        [Reactive] public ObservableCollectionExtended<DupliItem> DupliItems { get; set; } = new ObservableCollectionExtended<DupliItem>();

        public AddOtosViewModel() {
            DupliItems.Add(new DupliItem(true, "お", "を"));
            DupliItems.Add(new DupliItem(false, "うぉ", "を"));
            DupliItems.Add(new DupliItem(false, "ず", "づ"));
            DupliItems.Add(new DupliItem(false, "じ", "ぢ"));
            Instance = this;
        }

        public async void OK()
        {
            if (AddEnding && string.IsNullOrWhiteSpace(EndingAlias))
            {
                await MainWindowViewModel.MessageDialogOpen("語尾エイリアスを設定してください");
                return;
            }

            await Edit(otoIni =>
            {
                try
                {
                    var newOtos = new List<Oto>();
                    if (AddEnding)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            string alias;
                            string search1;
                            string search2 = string.Empty;

                            switch (i)
                            {
                                case 0:
                                    alias = "a " + EndingAlias + otoIni.Suffix;
                                    search1 = "あ";
                                    search2 = "か";
                                    break;
                                case 1:
                                    alias = "i " + EndingAlias + otoIni.Suffix;
                                    search1 = "い";
                                    search2 = "き";
                                    break;
                                case 2:
                                    alias = "u " + EndingAlias + otoIni.Suffix;
                                    search1 = "う";
                                    search2 = "く";
                                    break;
                                case 3:
                                    alias = "e " + EndingAlias + otoIni.Suffix;
                                    search1 = "え";
                                    search2 = "け";
                                    break;
                                case 4:
                                    alias = "o " + EndingAlias + otoIni.Suffix;
                                    search1 = "お";
                                    search2 = "こ";
                                    break;
                                case 5:
                                    alias = "n " + EndingAlias + otoIni.Suffix;
                                    search1 = "ん";
                                    break;
                                default:
                                    alias = "N " + EndingAlias + otoIni.Suffix;
                                    search1 = "ン";
                                    break;
                            }

                            if (!otoIni.OtoList.Any(o => o.Alias == alias))
                            {
                                AddEnding();
                            }
                            else if (Overwrite && AddEnding())
                            {
                                otoIni.RemoveAll(o => o.Alias == alias);
                            }

                            bool AddEnding()
                            {
                                var groups = otoIni.OtoList.GroupBy(oto => oto.FileName);
                                foreach (var group in groups)
                                {
                                    if (group.Key.Contains($"{search1}.") || group.Key.Contains($"{search1}{otoIni.Suffix}."))
                                    {
                                        AddEndLine(group);
                                        return true;
                                    }
                                }
                                if (string.IsNullOrWhiteSpace(search2)) return false;
                                foreach (var group in groups)
                                {
                                    if (group.Key.Contains($"{search2}.") || group.Key.Contains($"{search2}{otoIni.Suffix}."))
                                    {
                                        AddEndLine(group);
                                        return true;
                                    }
                                }
                                return false;
                            }
                            void AddEndLine(IGrouping<string, Oto> group)
                            {
                                var orderd = group.OrderBy(oto => oto.Offset);
                                double interval = 750;
                                if (group.Count() > 1)
                                {
                                    interval = double.Round(orderd.Zip(orderd.Skip(1), (a, b) => b.Offset - a.Offset).Average() * 1.5);
                                }
                                var oto = orderd.Last();
                                newOtos.Add(new Oto(oto.FileName, alias, oto.Offset + interval, 400, -450, 300, 100));
                            }
                        }
                    }
                    if (AddVowel)
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            string alias;
                            string search1;
                            string search2;
                            string search3;

                            switch (i)
                            {
                                case 0:
                                    alias = "* あ" + otoIni.Suffix;
                                    search1 = "a あ" + otoIni.Suffix;
                                    search2 = "- あ" + otoIni.Suffix;
                                    search3 = "あ" + otoIni.Suffix;
                                    break;
                                case 1:
                                    alias = "* い" + otoIni.Suffix;
                                    search1 = "i い" + otoIni.Suffix;
                                    search2 = "- い" + otoIni.Suffix;
                                    search3 = "い" + otoIni.Suffix;
                                    break;
                                case 2:
                                    alias = "* う" + otoIni.Suffix;
                                    search1 = "u う" + otoIni.Suffix;
                                    search2 = "- う" + otoIni.Suffix;
                                    search3 = "う" + otoIni.Suffix;
                                    break;
                                case 3:
                                    alias = "* え" + otoIni.Suffix;
                                    search1 = "e え" + otoIni.Suffix;
                                    search2 = "- え" + otoIni.Suffix;
                                    search3 = "え" + otoIni.Suffix;
                                    break;
                                case 4:
                                    alias = "* お" + otoIni.Suffix;
                                    search1 = "o お" + otoIni.Suffix;
                                    search2 = "- お" + otoIni.Suffix;
                                    search3 = "お" + otoIni.Suffix;
                                    break;
                                case 5:
                                    alias = "* ん" + otoIni.Suffix;
                                    search1 = "n ん" + otoIni.Suffix;
                                    search2 = "- ん" + otoIni.Suffix;
                                    search3 = "ん" + otoIni.Suffix;
                                    break;
                                default:
                                    alias = "* ン" + otoIni.Suffix;
                                    search1 = "N ン" + otoIni.Suffix;
                                    search2 = "- ン" + otoIni.Suffix;
                                    search3 = "ン" + otoIni.Suffix;
                                    break;
                            }

                            if (!otoIni.OtoList.Any(o => o.Alias == alias))
                            {
                                AddVowel();
                            }
                            else if (Overwrite && AddVowel())
                            {
                                otoIni.RemoveAll(o => o.Alias == alias);
                            }
                            bool AddVowel()
                            {
                                string search;
                                if (otoIni.OtoList.Any(o => o.Alias == search1))
                                    search = search1;
                                else if (otoIni.OtoList.Any(o => o.Alias == search2))
                                    search = search2;
                                else if (otoIni.OtoList.Any(o => o.Alias == search3))
                                    search = search3;
                                else return false;

                                foreach (Oto oto in otoIni.OtoList)
                                {
                                    if (oto.Alias == search)
                                    {
                                        newOtos.Add(new Oto(oto.FileName, alias, oto.Offset + oto.Pre - 40, 100, oto.Blank + oto.Pre - 40, 40, 80));
                                        return true;
                                    }
                                }
                                return false;
                            }
                        }
                    }
                    if (AddRentan)
                    {
                        List<string> removeList = new List<string>();
                        foreach (Oto oto in otoIni.OtoList)
                        {
                            if (!oto.Alias.Contains(' '))
                            {
                                if (!otoIni.OtoList.Any(o => o.Alias == $"- {oto.Alias}") || Overwrite)
                                {
                                    var newoto = oto.Clone();
                                    newoto.Alias = $"- {oto.Alias}";
                                    newOtos.Add(newoto);
                                    removeList.Add(oto.Alias);
                                }
                            }
                        }
                        otoIni.RemoveAll(oto => removeList.Contains(oto.Alias));
                    }
                    if (AddDupli && DupliItems != null)
                    {
                        foreach (var item in DupliItems)
                        {
                            if (!item.IsChecked || string.IsNullOrWhiteSpace(item.Search)) continue;
                            List<string> removeList = new List<string>();
                            foreach (Oto oto in otoIni.OtoList)
                            {
                                if (oto.Alias.Contains(item.Search))
                                {
                                    var alias = oto.Alias.Replace(item.Search, item.Alias);
                                    if (!otoIni.OtoList.Any(o => o.Alias == alias) || Overwrite)
                                    {
                                        var newoto = oto.Clone();
                                        newoto.Alias = alias;
                                        newOtos.Add(newoto);
                                        removeList.Add(oto.Alias);
                                    }
                                }
                            }
                            otoIni.RemoveAll(oto => removeList.Contains(oto.Alias));
                        }
                    }
                    if (newOtos.Count > 0)
                    {
                        otoIni.AddRange(newOtos);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                }
                return Task.FromResult(true);
            });
        }

        public void AddRow()
        {
            DupliItems.Add(new DupliItem(false, "", ""));
        }

        public void DeleteRow(DupliItem item)
        {
            DupliItems.Remove(item);
        }
    }

    public class DupliItem
    {
        [Reactive] public bool IsChecked { get; set; } = true;
        [Reactive] public string Search { get; set; } = "";
        [Reactive] public string Alias { get; set; } = "";
        [YamlIgnore] public AddOtosViewModel? Parent => AddOtosViewModel.Instance;

        public DupliItem() { }
        public DupliItem(bool isCheck, string search, string alias)
        {
            IsChecked = isCheck;
            Search = search;
            Alias = alias;
        }
    }
}
