using NAudio.Wave;
using OtoBatchEditor.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class ErrorCheckViewModel : PageViewModel
    {
        public ErrorCheckViewModel() { }

        public async void OK()
        {
            MainWindow.SetProgressIcon(true);
            try
            {
                var inis = OtoIni.GetOtoIniList();
                List<OtoIniError> errors = inis.Select(ini => new OtoIniError(ini)).ToList();
                await Parallel.ForEachAsync(
                    errors,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, // 並列度制限
                    async (error, ct) => { await error.ErrorCheck(); }
                );

                var DuplicateOtos = inis.SelectMany(ini => ini.OtoList)
                    .GroupBy(oto => oto.Alias)
                    .Where(group => group.Count() >= 2)
                    .ToList();

                if (!errors.Any(error => error.HasErrors) && DuplicateOtos.Count == 0)
                {
                    await MainWindowViewModel.MessageDialogOpen("エラーはありませんでした");
                    return;
                }

                var list = new List<string>();
                if (errors.Count > 0)
                {
                    errors.ForEach(error =>
                    {
                        if (error.HasErrors)
                        {
                            var text = $"# {error.FilePath} のエラー";
                            if (error.OtherErrors.Count > 0)
                            {
                                text += $"\n{string.Join("\n", error.OtherErrors)}";
                            }
                            if (error.InvaldOtos.Count > 0)
                            {
                                text += "\n\n- - - otoフォーマットエラー - - -";
                                error.InvaldOtos.ForEach(oto =>
                                {
                                    text += $"\nline{oto.LineNumber} \"{oto.Alias}\"：{string.Join(", ", oto.Error)}";
                                });
                            }
                            if (error.NotFoundWavs.Count > 0)
                            {
                                text += "\n\n- - - wavが見つからないか、ファイル名がUnicode正規化されていません（NFD） - - -";
                                text += $"\n{string.Join("\n", error.NotFoundWavs)}";
                            }
                            if (error.InvaldFileNames.Count > 0)
                            {
                                text += "\n\n- - - このwavファイル名はWindowsで使用できません - - -";
                                text += $"\n{string.Join("\n", error.InvaldFileNames)}";
                            }
                            if (error.NFDFiles.Count > 0)
                            {
                                text += "\n\n- - - Unicode正規化されていないwavファイル（NFD）が含まれています - - -";
                                text += $"\n{string.Join("\n", error.NFDFiles)}";
                            }
                            if (error.NotIncludedWavs.Count > 0)
                            {
                                text += "\n\n- - - oto.ini内に含まれていないwavファイルがあります - - -";
                                text += $"\n{string.Join("\n", error.NotIncludedWavs)}";
                            }
                            if (error.InvaldWavs.Count > 0)
                            {
                                text += "\n\n- - - wavフォーマットエラー - - -";
                                error.InvaldWavs.ForEach(wav =>
                                {
                                    text += $"\n{wav.FileName}：{string.Join(", ", wav.Errors)}";
                                });
                            }
                            list.Add(text);
                        }
                    });
                    
                }
                if (DuplicateOtos.Count > 0)
                {
                    var text = "# 以下のエイリアスが重複しています";
                    DuplicateOtos.ForEach(group =>
                    {
                        text += $"\n\n{group.Key}：\n{string.Join("\n", group.Select(oto => $"{Path.GetFileName(oto.DirectoryPath)} line{oto.LineNumber}：\"{oto.FileName}\"" ))}";
                    });
                    list.Add(text);
                }
                var join = string.Join("\n\n\n", list);
                var result = await MainWindowViewModel.MessageDialogOpen(join, "クリップボードにコピーして閉じる", "閉じる");
                if (result)
                {
                    await MainWindowViewModel.GetClipboard().SetTextAsync(join);
                    MainWindowViewModel.ShowSnackbar("クリップボードにコピーしました");
                }
            }
            catch (Exception e)
            {
                MainWindow.SetProgressIcon(false);
                DebugMode.AddError(e);
                await MainWindowViewModel.MessageDialogOpen(e.Message);
                await DebugMode.Export(LogOutputType.Error);
                return;

            }
            MainWindow.SetProgressIcon(false);
            await DebugMode.Export(LogOutputType.Completed);
        }
    }

    public class OtoIniError
    {
        public string FilePath { get; } = string.Empty;
        public List<Oto> InvaldOtos { get; private set; } = new List<Oto>();
        public List<string> NotFoundWavs { get; private set; } = new List<string>();
        public List<string> InvaldFileNames { get; private set; } = new List<string>();
        public List<string> NFDFiles { get; private set; } = new List<string>();
        public List<string> NotIncludedWavs { get; private set; } = new List<string>();
        public List<Wav> InvaldWavs { get; private set; } = new List<Wav>();
        public List<string> OtherErrors { get; private set; } = new List<string>();
        public bool HasErrors { get => InvaldOtos.Any() || NotFoundWavs.Any() || InvaldFileNames.Any() || NFDFiles.Any() || NotIncludedWavs.Any() || InvaldWavs.Any() || OtherErrors.Any(); }
        private OtoIni otoIni;

        public OtoIniError(OtoIni otoIni)
        {
            FilePath = otoIni.FilePath;
            this.otoIni = otoIni;
        }

        public async Task ErrorCheck()
        {
            // 読み込み
            try
            {
                otoIni.Read();
                if (otoIni.Encoding != Encoding.GetEncoding("Shift_JIS"))
                {
                    OtherErrors.Add($"テキストエンコードは {otoIni.Encoding.EncodingName} です");
                }
                if (!otoIni.DirectoryPath.IsNormalized())
                {
                    OtherErrors.Add($"フォルダパスがUnicode正規化されていません");
                }
            }
            catch (Exception e)
            {
                OtherErrors.Add(e.Message);
                return;
            }

            // 書き込み
            try
            {
                otoIni.Copy(true, "oto_copytest.ini");
                File.Delete(Path.Combine(otoIni.DirectoryPath, "oto_copytest.ini"));
            }
            catch (Exception e)
            {
                OtherErrors.Add($"フォルダへの書き込みに失敗しました：{e.Message}");
            }

            // フォーマットチェック
            try
            {
                var otoWavs = otoIni.OtoList.Select(oto => oto.FileName).Distinct();
                var wavFileNames = Directory.GetFiles(otoIni.DirectoryPath, "*.wav").Select(path => Path.GetFileName(path));
                var wavs = otoWavs
                    .Select(wav => Path.Combine(otoIni.DirectoryPath, wav))
                    .Where(path => File.Exists(path))
                    .Select(path => new Wav(path, true))
                    .ToList();
                wavs.AsParallel().ForAll(wav => { wav.TryRead(); });


                await Parallel.ForEachAsync(
                    wavs,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, // 並列度制限
                    async (wav, ct) =>
                    {
                        await Task.Run(() => wav.TryRead());
                    }
                );

                // oto.ini読み込み時に気づけるエラー
                InvaldOtos = otoIni.OtoList.Where(oto => oto.Status == OtoStatus.Invalid).ToList();

                // otoフォーマットのチェック
                InvaldOtos.AddRange(ParameterCheck(otoIni, wavs));

                // 不正エイリアスのチェック
                InvaldOtos.AddRange(AliasCheck(otoIni));
                InvaldOtos = InvaldOtos.Distinct().ToList();

                // ファイルが存在しない or NFD（濁点分離）
                NotFoundWavs = otoWavs
                    .Where(fileName => !File.Exists(Path.Combine(otoIni.DirectoryPath, fileName)))
                    .ToList();

                // ファイル名が不正
                char[] winInvalid = Path.GetInvalidFileNameChars(); // Windowsで使えないファイル名文字
                string[] reservedNames = // Windows予約語
                {
                    "CON","PRN","AUX","NUL",
                    "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9",
                    "LPT1","LPT2","LPT3","LPT4","LPT5","LPT6","LPT7","LPT8","LPT9"
                };
                InvaldFileNames = otoWavs
                    .Where(fileName => fileName.IndexOfAny(winInvalid) >= 0 || reservedNames.Contains(fileName.ToUpper()) || fileName.EndsWith(" ") || fileName.EndsWith("."))
                    .ToList();

                // oto.ini内のファイル名がNFD
                NFDFiles = otoWavs
                    .Where(path => !path.IsNormalized())
                    .ToList();

                // iniに含まれていないwavファイル or NFD（濁点分離）
                NotIncludedWavs = wavFileNames
                    .Where(wav => !otoWavs.Contains(wav))
                    .ToList();

                // wavフォーマットのチェック
                InvaldWavs = WavCheck(wavs);
            }
            catch (Exception e)
            {
                OtherErrors.Add(e.Message);
                return;
            }
        }

        private List<Oto> AliasCheck(OtoIni otoIni)
        {
            var invalidAlias = new List<Oto>();
            otoIni.OtoList.ForEach(oto =>
            {
                if (oto.Alias.StartsWith(" ") || oto.Alias.EndsWith(" "))
                {
                    oto.Error.Add("エイリアスの前後に空白があります");
                    invalidAlias.Add(oto);
                }
                else if (Regex.Count(oto.Alias, " ") > 1)
                {
                    oto.Error.Add("エイリアスにスペースが2つ以上含まれています");
                    invalidAlias.Add(oto);
                }
                if (oto.Alias.Contains('　'))
                {
                    oto.Error.Add("エイリアスに全角スペースが含まれています");
                    invalidAlias.Add(oto);
                }
                if (!oto.Alias.IsNormalized())
                {
                    oto.Error.Add("エイリアスがUnicode正規化されていません");
                    invalidAlias.Add(oto);
                }
                if (oto.Alias == "R" || oto.Alias == "r")
                {
                    oto.Error.Add("エイリアス「R」や「r」は休符との区別がつきません");
                    invalidAlias.Add(oto);
                }
            });
            return invalidAlias.Distinct().ToList();
        }

        private List<Wav> WavCheck(List<Wav> wavs)
        {
            var invalidWavs = new List<Wav>();
            foreach (var wav in wavs)
            {
                if (wav.WaveFormat == null)
                {
                    invalidWavs.Add(wav);
                }
                else
                {
                    if (wav.WaveFormat.Encoding != WaveFormatEncoding.Pcm && wav.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                    {
                        wav.Errors.Add("ファイルのエンコードがPCMまたはIeeeFloatではありません");
                        invalidWavs.Add(wav);
                    }
                    if (wav.WaveFormat.BitsPerSample != 16)
                    {
                        wav.Errors.Add("wavのビット深度が16bitではありません");
                        invalidWavs.Add(wav);
                    }
                    if (wav.WaveFormat.SampleRate != 44100)
                    {
                        wav.Errors.Add("wavのサンプルレートが44100Hzではありません");
                        invalidWavs.Add(wav);
                    }
                    if (wav.WaveFormat.Channels != 1)
                    {
                        wav.Errors.Add("wavがモノラルではありません");
                        invalidWavs.Add(wav);
                    }
                    if (wav.LengthMs < 0)
                    {
                        wav.Errors.Add("wavファイルの長さが0です");
                        invalidWavs.Add(wav);
                    }
                }
            }
            return invalidWavs.Distinct().ToList();
        }

        private IEnumerable<Oto> ParameterCheck(OtoIni otoIni, List<Wav> wavs)
        {
            var invalidOtos = new List<Oto>();
            otoIni.OtoList.ForEach(oto =>
            {
                if (oto.Offset < 0)
                {
                    oto.Error.Add("左ブランクが負の値です");
                    invalidOtos.Add(oto);
                }
                if (oto.Pre < 0)
                {
                    oto.Error.Add("先行発声が負の値です");
                    invalidOtos.Add(oto);
                }
                if (oto.Consonant < oto.Pre)
                {
                    oto.Error.Add("子音範囲が先行発声よりも短くなっています");
                    invalidOtos.Add(oto);
                }
                if (oto.Blank < 0 && - oto.Blank <= oto.Consonant)
                {
                    oto.Error.Add("伸縮範囲がありません");
                    invalidOtos.Add(oto);
                }
                
                if (wavs.FirstOrDefault(wav => wav.FileName == oto.FileName) is Wav wav)
                {
                    if (wav.LengthMs == 0) return;

                    if (oto.Offset > wav.LengthMs)
                    {
                        oto.Error.Add("左ブランクがwavファイルの長さを超えています");
                        invalidOtos.Add(oto);
                    }
                    else if (oto.Offset + oto.Pre > wav.LengthMs)
                    {
                        oto.Error.Add("先行発声がwavファイルの長さを超えています");
                        invalidOtos.Add(oto);
                    }
                    else if (oto.Offset + oto.Consonant > wav.LengthMs)
                    {
                        oto.Error.Add("子音範囲がwavファイルの長さを超えています");
                        invalidOtos.Add(oto);
                    }

                    if (oto.Blank < 0)
                    {
                        if (oto.Offset - oto.Blank > wav.LengthMs + 1)
                        {
                            oto.Error.Add("右ブランクがwavファイルの長さを超えています");
                            invalidOtos.Add(oto);
                        }
                    }
                    else
                    {
                        if (oto.Blank > wav.LengthMs)
                        {
                            oto.Error.Add("右ブランクがwavファイルの開始よりも前にあります");
                            invalidOtos.Add(oto);
                        }
                        if (wav.LengthMs - oto.Blank < oto.Offset + oto.Consonant)
                        {
                            oto.Error.Add("伸縮範囲がありません");
                            invalidOtos.Add(oto);
                        }
                    }
                }
            });
            return invalidOtos.Distinct();
        }
    }
}
