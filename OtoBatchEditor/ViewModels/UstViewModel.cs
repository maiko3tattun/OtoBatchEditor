using OtoBatchEditor.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OtoBatchEditor.ViewModels
{
    public class UstViewModel : PageViewModel
    {
        [Reactive] public bool LyricFromAlias { get; set; } = true;
        [Reactive] public string Tempo { get; set; } = "120";
        [Reactive] public string DisplayPath { get; set; } = string.Empty;
        [Reactive] public string directoryPath { get; set; } = string.Empty;
        [Reactive] public bool CanExe { get; set; } = false;
        public string HintText { get; }

        public UstViewModel()
        {
            HintText = "CVVC音源の場合やOpenUtauで使用する場合はこちらを選択してください。\n" +
                "語尾など特殊音素単体のwavはうまく吐き出せないことがあります。";

            this.WhenAnyValue(x => x.directoryPath)
                .Subscribe(value =>
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        DisplayPath = "フォルダを指定してください";
                        CanExe = false;
                    }
                    else
                    {
                        DisplayPath = $"保存先：{value}";
                        CanExe = true;
                    }
                });

            this.WhenAnyValue(x => x.Tempo)
                .Subscribe(value =>
                {
                    Tempo = NumValidation.IntValidation(value, 120, 50, 400, out bool valid).ToString();
                });
        }

        public void SetDirectory(string path)
        {
            directoryPath = path;
        }

        public async void OK()
        {
            if (!Directory.Exists(directoryPath))
            {
                await MainWindowViewModel.MessageDialogOpen("ustの保存先を選択してください");
                return;
            }
            var tempo = NumValidation.IntValidation(Tempo, 120, 50, 400, out bool valid);
            if (!valid)
            {
                await MainWindowViewModel.MessageDialogOpen("テンポの値が異常です");
                return;
            }

            await Open(async otoIni =>
            {
                var ustName = Path.GetFileName(otoIni.DirectoryPath).Replace(".", "_");
                var ustPath = Path.Combine(directoryPath, $"{ustName}.ust");
                if (File.Exists(ustPath))
                {
                    await MainWindowViewModel.MessageDialogOpen(ustPath + "が\n既に存在するため作成できません");
                    return;
                }
                int noteNum = 60;
                foreach (string key in NoteNum.Pitches)
                {
                    if (otoIni.Suffix.Contains(key))
                    {
                        noteNum = NoteNum.NoteNumDict[key];
                        break;
                    }
                }

                var ust = new List<string>
                            {
                                $"[#VERSION]",
                                $"UST Version1.2",
                                $"[#SETTING]",
                                $"Tempo={tempo}",
                                $"ProjectName={ustName}",
                                $"VoiceDir={GetVBPath(otoIni.DirectoryPath)}",
                                $"Mode2=True"
                            };
                ust.AddRange(InsertR(0, noteNum));
                if (LyricFromAlias)
                {
                    CreateWithAlias(ust);
                }
                else
                {
                    CreateWithFilename(ust);
                }
                ust.Add("[#TRACKEND]");

                try
                {
                    File.WriteAllLines(ustPath, ust, Encoding.GetEncoding("Shift_JIS"));
                }
                catch (IOException e)
                {
                    throw new IOException($"{ustPath} の書き込みに失敗しました。", e);
                }
                catch (UnauthorizedAccessException e)
                {
                    throw new UnauthorizedAccessException($"{ustPath} の書き込みに失敗しました。アクセス権限がありません。", e);
                }
                catch (Exception e)
                {
                    throw new Exception($"{ustPath} の書き込みに失敗しました。予期せぬエラーが発生しました。", e);
                }

                // ustの中身
                void CreateWithAlias(List<string> ust)
                {
                    var otoGroup = otoIni.OtoList.GroupBy(oto => oto.FileName);
                    int n = 1;
                    foreach (var group in otoGroup)
                    {
                        group.OrderBy(oto => oto.Offset);
                        foreach (Oto oto in group)
                        {
                            ust.Add($"[#{n.ToString("0000")}]");
                            ust.Add("Length=480");
                            ust.Add($"Lyric={oto.Alias}");
                            ust.Add($"NoteNum={noteNum}");
                            ust.Add("PreUtterance=");
                            ust.Add("Intensity=100");
                            ust.Add("Modulation=0");
                            n++;
                        }
                        ust.AddRange(InsertR(n, noteNum));
                        n++;
                    }
                }
                void CreateWithFilename(List<string> ust)
                {
                    var wavs = otoIni.OtoList.Select(oto => oto.FileName)
                        .Distinct()
                        .ToList();
                    
                    int n = 1;
                    foreach (string wav in wavs)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(wav);
                        fileName = Regex.Replace(fileName, "^[0-9]+", ""); // 先頭に数字があったら除去
                        fileName = Regex.Replace(fileName, "^_", ""); // 先頭_を除去
                        fileName = fileName.Replace(otoIni.Suffix, ""); // suffixを除去

                        string alt = string.Empty;
                        Match match = Regex.Match(fileName, @"[・'23456789]$");
                        if (match.Success)
                        {
                            alt = match.Value;
                            fileName = fileName.Replace(match.Value, "");
                        }
                        var lyrics = new List<string>();
                        foreach (var c in fileName)
                        {
                            if (lyrics.Count > 0 && Phoneme.YouonKanas.Contains(c))
                            {
                                lyrics[lyrics.Count - 1] = $"{lyrics.Last()}{c}";
                            }
                            else
                            {
                                lyrics.Add(c.ToString());
                            }
                        }

                        foreach (string str in lyrics)
                        {
                            if (str == "_")
                            {
                                ust.AddRange(InsertR(n, noteNum));
                                n++;
                            }
                            else
                            {

                                ust.Add($"[#{n.ToString("0000")}]");
                                ust.Add($"Length=480");
                                ust.Add($"Lyric={str}{alt}{otoIni.Suffix}");
                                ust.Add($"NoteNum={noteNum}");
                                ust.Add($"PreUtterance=");
                                ust.Add($"Intensity=100");
                                ust.Add($"Modulation=0");
                                n++;
                            }
                        }
                        ust.AddRange(InsertR(n, noteNum));
                        n++;
                    }
                }
            },
            "完了");
        }

        private string GetVBPath(string dirPath)
        {
            var vbPath = dirPath;
            if (File.Exists(Path.Combine(vbPath, "character.txt")))
            {
                // oto.iniが音源フォルダ直下
                return vbPath;
            }

            vbPath = Path.GetDirectoryName(vbPath)!;
            if (File.Exists(Path.Combine(vbPath, "character.txt")))
            {
                // oto.iniが子フォルダにある
                return vbPath;
            }

            vbPath = Path.GetDirectoryName(vbPath)!;
            if (File.Exists(Path.Combine(vbPath, "character.txt")))
            {
                // oto.iniが孫フォルダにある
                return vbPath;
            }

            // よくわからん
            return dirPath;
        }

        private List<string> InsertR(int num, int noteNum)
        {
            return new List<string>
            {
                $"[#{num.ToString("0000")}]",
                "Length=480",
                "Lyric=R",
                "NoteNum=" + noteNum,
                "PreUtterance="
            };
        }
    }
}
