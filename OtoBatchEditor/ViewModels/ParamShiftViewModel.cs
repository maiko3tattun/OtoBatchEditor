using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class ParamShiftViewModel : PageViewModel
    {
        [Reactive] public string Shift { get; set; } = "0";
        [Reactive] public string OriginalTempo { get; set; } = "120";
        [Reactive] public string NewTempo { get; set; } = "180";

        public ParamShiftViewModel()
        {
            this.WhenAnyValue(x => x.Shift)
                .Subscribe(value =>
                {
                    Shift = NumValidation.IntValidation(value, 0, -5000, 5000, out bool valid).ToString();
                });
            this.WhenAnyValue(x => x.OriginalTempo)
                .Subscribe(value =>
                {
                    OriginalTempo = NumValidation.IntValidation(value, 120, 50, 400, out bool valid).ToString();
                });
            this.WhenAnyValue(x => x.NewTempo)
                .Subscribe(value =>
                {
                    NewTempo = NumValidation.IntValidation(value, 180, 50, 400, out bool valid).ToString();
                });
        }
        
        public async void OK()
        {
            var shift = NumValidation.IntValidation(Shift, 0, -5000, 5000, out bool valid);
            if (!valid || shift == 0)
            {
                await MainWindowViewModel.MessageDialogOpen("0以外の整数を入力してください");
                return;
            }

            await Edit(async otoIni =>
            {
                try
                {
                    var wavFiles = Directory.GetFiles(otoIni.DirectoryPath, "*.wav")
                        .Select(path => new Wav(path, true));
                    var wavList = new List<Wav>();
                    await Parallel.ForEachAsync(
                        wavFiles,
                        new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, // 並列度制限
                        async (wav, ct) =>
                        {
                            if (await Task.Run(() => wav.TryRead()))
                            {
                                lock (wavList) // Listはスレッドセーフじゃないのでロック
                                {
                                    wavList.Add(wav);
                                }
                            }
                        }
                    );

                    otoIni.OtoList.ForEach(oto =>
                    {
                        double lengthMs = 0;
                        var wav = wavList.FirstOrDefault(w => w.FileNameNFD == oto.FileName.Normalize());
                        if (wav != null)
                        {
                            lengthMs = wav.LengthMs;
                        }

                        var newOffset = oto.Offset + shift;
                        if (newOffset < 0)
                        {
                            oto.Offset = 0;
                        }
                        else if (oto.Blank >= 0)
                        {
                            if (lengthMs > 0 && newOffset + oto.Consonant + oto.Blank > lengthMs)
                            {
                                oto.Offset = lengthMs - oto.Consonant - oto.Blank - 10;
                            }
                            else
                            {
                                oto.Offset = newOffset;
                            }
                        }
                        else
                        {
                            if (lengthMs > 0 && newOffset - oto.Blank > lengthMs)
                            {
                                oto.Offset = lengthMs - oto.Blank;
                            }
                            else
                            {
                                oto.Offset = newOffset;
                            }
                        }
                    });
                }
                catch (Exception e)
                {
                    throw new Exception($"予期せぬエラーが発生しました\n{otoIni.DirectoryPath}\n{e.Message}", e);
                }
                return true;
            });
        }

        public async void TempoChange()
        {
            var oTempo = NumValidation.IntValidation(OriginalTempo, 120, 50, 400, out bool valid1);
            var nTempo = NumValidation.IntValidation(NewTempo, 180, 50, 400, out bool valid2);
            if (!valid1 || !valid2)
            {
                await MainWindowViewModel.MessageDialogOpen("入力された数値が不正です");
                return;
            }
            if (oTempo == nTempo)
            {
                await MainWindowViewModel.MessageDialogOpen("変更後のテンポが元のテンポと同じです");
                return;
            }

            await Edit(otoIni =>
            {
                try
                {
                    otoIni.OtoList.ForEach(oto =>
                    {
                        oto.Offset = oto.Offset * oTempo / nTempo;
                        oto.Pre = oto.Pre * oTempo / nTempo;
                        oto.Consonant = oto.Consonant * oTempo / nTempo;
                        oto.Ovl = oto.Ovl * oTempo / nTempo;
                        oto.Blank = oto.Blank * oTempo / nTempo;
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
