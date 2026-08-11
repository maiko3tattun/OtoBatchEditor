using OtoBatchEditor.Models;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class AddCVViewModel : PageViewModel
    {
        [Reactive] public int ModeIndex { get; set; } = 0;
        [Reactive] public bool AltVersion { get; set; } = true;
        [Reactive] public bool OverWrite { get; set; } = false;

        public async void OK()
        {
            await Edit(otoIni =>
            {
                try
                {
                    var newOtos = new List<Oto>();

                    var PreCVs = otoIni.OtoList
                        .Where(o => o.Alias.StartsWith("- "))
                        .ToList();
                    foreach (var preCV in PreCVs)
                    {
                        string alias = preCV.Alias.Substring(2);
                        if (newOtos.Any(o => o.Alias == alias)) continue;
                        if (!OverWrite && otoIni.OtoList.Any(o => o.Alias == alias)) continue;

                        string body;
                        string pattern = $@"^-\s+(?<body>.+?)(?<num>\d*){Regex.Escape(otoIni.Suffix)}$";
                        // (?<body>.+?)：「body」という名前のグループ。後続のパターンに引っかからない最小の文字列を抽出（最短一致）
                        // (?<num>\d*)：「num」という名前のグループ。数字0文字以上
                        Match match = Regex.Match(preCV.Alias, pattern);
                        if (match.Success)
                        {
                            body = match.Groups["body"].Value;
                            if (AltVersion && !string.IsNullOrEmpty(match.Groups["num"].Value)) continue;
                        }
                        else
                        {
                            body = alias.Replace(otoIni.Suffix, "");
                        }

                        Oto oto = null;
                        Oto newOto = null;
                        if (ModeIndex == 1)
                        {
                            oto = preCV;
                            newOto = preCV.Clone();
                        }
                        else
                        {
                            double preutter = 50;
                            double overlap = 20;

                            if (Kana.TryGetKana(body, out Kana? kana) && kana != null)
                            {
                                preutter = kana.Preutter;
                                overlap = kana.Overlap;

                                if (ModeIndex == 0)
                                {
                                    var search = otoIni.OtoList.FirstOrDefault(oto => oto.Alias == $"{kana.Vowel} {alias}");
                                    if (search != null)
                                    {
                                        oto = search;
                                    }
                                }
                            }
                            if (oto == null)
                            {
                                oto = preCV;
                            }
                            newOto = oto.Clone();

                            // パラメータ補正
                            var diff = oto.Pre - preutter;
                            newOto.Offset = oto.Offset + diff;
                            newOto.Pre = preutter;
                            newOto.Ovl = overlap;
                            newOto.Consonant = oto.Consonant - diff;
                            newOto.Blank = oto.Blank + diff;
                        }

                        newOto.Alias = alias;
                        newOtos.Add(newOto);
                        otoIni.RemoveAll(o => o.Alias == alias);
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
    }
}
