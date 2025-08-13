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
    public class AddPreCViewModel : PageViewModel
    {
        public static AddPreCViewModel? Instance;

        public ObservableCollectionExtended<AddPreCItem> Consonants { get; set; } = new ObservableCollectionExtended<AddPreCItem>();
        [Reactive] public bool? CheckAll { get; set; }
        [Reactive] public bool Overwrite { get; set; } = false;

        public async void OK()
        {
            if (CheckAll == false)
            {
                await MainWindowViewModel.MessageDialogOpen("1つ以上チェックしてください");
                return;
            }

            await Edit(otoIni =>
            {
                try
                {
                    var newOtos = new List<Oto>();

                    foreach (var item in Consonants)
                    {
                        if (!item.IsChecked) continue;

                        string alias = $"- {item.Consonant}{otoIni.Suffix}";
                        string search = $"- {item.Kana}{otoIni.Suffix}";

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
                                    newOto.Offset = oto.Offset - 20;
                                    newOto.Pre = 20;
                                    newOto.Ovl = 0;
                                    newOto.Consonant = item.Length - 20;
                                    newOto.Blank = -item.Length;

                                    newOtos.Add(newOto);
                                    return true;
                                }
                            }
                            return false;
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

        public AddPreCViewModel()
        {
            for (int i = 0; i < Phoneme.Consonants.Count; i++)
            {
                var phoneme = Phoneme.Consonants.ElementAt(i);
                if (i > 7 && i < 20)
                {
                    Consonants.Add(new AddPreCItem(phoneme.Consonant, phoneme.Kana, phoneme.Length, true));
                }
                else
                {
                    Consonants.Add(new AddPreCItem(phoneme.Consonant, phoneme.Kana, phoneme.Length));
                }
            }

            this.WhenAnyValue(x => x.CheckAll)
                .Subscribe(value =>
                {
                    if (value == null) return;
                    foreach (var consonant in Consonants)
                    {
                        consonant.IsChecked = value ?? false;
                    }
                });

            Instance = this;
        }

        public void UpdateCheck()
        {
            if (Consonants == null || Consonants.Count == 0)
            {
                CheckAll = false;
            }
            else if (Consonants.All(c => c.IsChecked))
            {
                CheckAll = true;
            }
            else if (Consonants.All(c => !c.IsChecked))
            {
                CheckAll = false;
            }
            else
            {
                CheckAll = null;
            }
        }

        public void AddRow()
        {
            Consonants.Add(new AddPreCItem("", "", 80));
        }

        public void DeleteRow(AddPreCItem item)
        {
            Consonants.Remove(item);
        }
    }

    public class AddPreCItem : ReactiveObject
    {
        [Reactive] public bool IsChecked { get; set; } = false;
        [Reactive] public string Consonant { get; set; } = string.Empty;
        [Reactive] public string Kana { get; set; } = string.Empty;
        [Reactive] public int Length { get; set; } = 80;
        [YamlIgnore] public AddPreCViewModel? Parent => AddPreCViewModel.Instance;

        public AddPreCItem() { }
        public AddPreCItem(string consonant, string kana, int length, bool isChecked = false)
        {
            Consonant = consonant;
            Kana = kana;
            Length = length;
            IsChecked = isChecked;
            this.WhenAnyValue(x => x.IsChecked)
                .Subscribe(value =>
                {
                    Parent?.UpdateCheck();
                });
        }
    }
}
