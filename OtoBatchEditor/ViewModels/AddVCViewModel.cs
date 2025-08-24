using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace OtoBatchEditor.ViewModels
{
    public class AddVCViewModel : PageViewModel
    {
        public static AddVCViewModel? Instance;

        public ObservableCollectionExtended<AddVCItem> Consonant1 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant2 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant3 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant4 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        public ObservableCollectionExtended<AddVCItem> Consonant5 { get; set; } = new ObservableCollectionExtended<AddVCItem>();
        [Reactive] public bool? Check1 { get; set; }
        [Reactive] public bool? Check2 { get; set; }
        [Reactive] public bool? Check3 { get; set; }
        [Reactive] public bool? Check4 { get; set; }
        [Reactive] public bool? Check5 { get; set; } = false;
        [Reactive] public bool Overwrite { get; set; } = false;

        public async void OK()
        {
            if (Check1 == false && Check2 == false && Check3 == false && Check4 == false && Check5 == false)
            {
                await MainWindowViewModel.MessageDialogOpen("1つ以上チェックしてください");
                return;
            }

            await Edit(otoIni =>
            {
                try
                {
                    AddVC(Consonant1);
                    AddVC(Consonant2);
                    AddVC(Consonant3);
                    AddVC(Consonant4);
                    AddVC(Consonant5);

                    void AddVC(Collection<AddVCItem> consonants)
                    {
                        var newOtos = new List<Oto>();

                        foreach (var item in consonants)
                        {
                            if (!item.IsChecked) continue;

                            for (int i = 0; i < 7; i++)
                            {
                                string alias = "";
                                string search = "";

                                switch (i)
                                {
                                    case 0:
                                        alias = $"a {item.Consonant}{otoIni.Suffix}";
                                        search = $"a {item.Kana}{otoIni.Suffix}";
                                        break;
                                    case 1:
                                        alias = $"i {item.Consonant}{otoIni.Suffix}";
                                        search = $"i {item.Kana}{otoIni.Suffix}";
                                        break;
                                    case 2:
                                        alias = $"u {item.Consonant}{otoIni.Suffix}";
                                        search = $"u {item.Kana}{otoIni.Suffix}";
                                        break;
                                    case 3:
                                        alias = $"e {item.Consonant}{otoIni.Suffix}";
                                        search = $"e {item.Kana}{otoIni.Suffix}";
                                        break;
                                    case 4:
                                        alias = $"o {item.Consonant}{otoIni.Suffix}";
                                        search = $"o {item.Kana}{otoIni.Suffix}";
                                        break;
                                    case 5:
                                        alias = $"n {item.Consonant}{otoIni.Suffix}";
                                        search = $"n {item.Kana}{otoIni.Suffix}";
                                        break;
                                    case 6:
                                        alias = $"N {item.Consonant}{otoIni.Suffix}";
                                        search = $"N {item.Kana}{otoIni.Suffix}";
                                        break;
                                }

                                if (!otoIni.OtoList.Any(o => o.Alias == alias))
                                {
                                    AddNewLine();
                                }
                                else if (Overwrite && AddNewLine())
                                {
                                    otoIni.RemoveAll(o => o.Alias == alias);
                                }
                                bool AddNewLine()
                                {
                                    foreach (Oto oto in otoIni.OtoList)
                                    {
                                        if (oto.Alias == search)
                                        {
                                            var newOto = oto.Clone();
                                            newOto.Alias = alias;
                                            newOto.Offset = oto.Offset - (item.Length / 2);
                                            newOto.Pre = oto.Pre - (item.Length / 2);
                                            newOto.Ovl = newOto.Pre / 3;
                                            newOto.Consonant = newOto.Pre + 20;
                                            newOto.Blank = -(newOto.Pre + (item.IsPlosive ? 40 : item.Length - 10));

                                            newOtos.Add(newOto);
                                            return true;
                                        }
                                    }
                                    return false;
                                }
                            }
                        }

                        if (newOtos.Count > 0)
                        {
                            otoIni.AddRange(newOtos);
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

        public AddVCViewModel()
        {
            for (int i = 0; i < Phoneme.Consonants.Count; i++)
            {
                var phoneme = Phoneme.Consonants.ElementAt(i);
                if (i < 3) // 無声破裂音（ktp）
                {
                    Consonant1.Add(new AddVCItem(phoneme.Consonant, phoneme.Kana, UpdateCheck1, phoneme.Length, phoneme.IsPlosive, true));
                }
                else if (i < 5) // 無声破裂音
                {
                    Consonant1.Add(new AddVCItem(phoneme.Consonant, phoneme.Kana, UpdateCheck1, phoneme.Length, phoneme.IsPlosive));
                }
                else if (i < 8) // 有声破裂音
                {
                    Consonant2.Add(new AddVCItem(phoneme.Consonant, phoneme.Kana, UpdateCheck2, phoneme.Length, phoneme.IsPlosive));
                }
                else if (i < 20) // その他の子音
                {
                    Consonant3.Add(new AddVCItem(phoneme.Consonant, phoneme.Kana, UpdateCheck3, phoneme.Length, phoneme.IsPlosive));
                }
                else // 拗音・ハ行
                {
                    Consonant4.Add(new AddVCItem(phoneme.Consonant, phoneme.Kana, UpdateCheck4, phoneme.Length, phoneme.IsPlosive));
                }
            }
            Consonant5.Add(new AddVCItem("k", "く", UpdateCheck5, 80, false, false)); // Sample

            this.WhenAnyValue(x => x.Check1)
                .Subscribe(value =>
                {
                    if (value == null) return;
                    foreach (var consonant in Consonant1)
                    {
                        consonant.IsChecked = value ?? false;
                    }
                });

            this.WhenAnyValue(x => x.Check2)
                .Subscribe(value =>
                {
                    if (value == null) return;
                    foreach (var consonant in Consonant2)
                    {
                        consonant.IsChecked = value ?? false;
                    }
                });

            this.WhenAnyValue(x => x.Check3)
                .Subscribe(value =>
                {
                    if (value == null) return;
                    foreach (var consonant in Consonant3)
                    {
                        consonant.IsChecked = value ?? false;
                    }
                });

            this.WhenAnyValue(x => x.Check4)
                .Subscribe(value =>
                {
                    if (value == null) return;
                    foreach (var consonant in Consonant4)
                    {
                        consonant.IsChecked = value ?? false;
                    }
                });

            this.WhenAnyValue(x => x.Check5)
                .Subscribe(value =>
                {
                    if (value == null) return;
                    foreach (var consonant in Consonant5)
                    {
                        consonant.IsChecked = value ?? false;
                    }
                });

            Instance = this;
        }

        public void UpdateCheck1()
        {
            Check1 = UpdateCheck(Consonant1);
        }
        public void UpdateCheck2()
        {
            Check2 = UpdateCheck(Consonant2);
        }
        public void UpdateCheck3()
        {
            Check3 = UpdateCheck(Consonant3);
        }
        public void UpdateCheck4()
        {
            Check4 = UpdateCheck(Consonant4);
        }
        public void UpdateCheck5()
        {
            Check5 = UpdateCheck(Consonant5);
        }
        public bool? UpdateCheck(ObservableCollection<AddVCItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return false;
            }
            else if (items.All(c => c.IsChecked))
            {
                return true;
            }
            else if (items.All(c => !c.IsChecked))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public void AddRow()
        {
            Consonant5.Add(new AddVCItem("", "", UpdateCheck5, 80, false, false));
        }

        public void DeleteRow(AddVCItem item)
        {
            Consonant5.Remove(item);
        }
    }

    public class AddVCItem : ReactiveObject
    {
        [Reactive] public bool IsChecked { get; set; } = false;
        [YamlIgnore] public string Label { get; } = string.Empty;
        [Reactive] public string Consonant { get; set; } = string.Empty;
        [Reactive] public string Kana { get; set; } = string.Empty;
        [Reactive] public int Length { get; set; } = 80;
        [Reactive] public bool IsPlosive { get; set; } = false;
        [YamlIgnore] public AddVCViewModel? Parent => AddVCViewModel.Instance;

        public AddVCItem() { }
        public AddVCItem(string consonant, string kana, Action action, int length, bool prosive = false, bool isChecked = false)
        {
            Label = $"a {consonant}（{kana}）";
            Consonant = consonant;
            Kana = kana;
            Length = length;
            IsPlosive = prosive;
            IsChecked = isChecked;
            this.WhenAnyValue(x => x.IsChecked)
                .Subscribe(value =>
                {
                    action.Invoke();
                });
        }

        public void Set(AddVCItem item)
        {
            IsChecked = item.IsChecked;
            Consonant = item.Consonant;
            Kana = item.Kana;
            Length = item.Length;
            IsPlosive = item.IsPlosive;
        }
    }
}
