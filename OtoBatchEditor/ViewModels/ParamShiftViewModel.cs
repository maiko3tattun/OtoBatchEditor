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

        public ParamShiftViewModel()
        {
            this.WhenAnyValue(x => x.Shift)
                .Subscribe(value =>
                {
                    Shift = NumValidation.IntValidation(value, 0, -5000, 5000, out bool valid).ToString();
                });
        }
        
        public async void OK()
        {
            if (!int.TryParse(Shift, out int shift) || shift == 0)
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
    }
}
