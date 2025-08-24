using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OtoBatchEditor.ViewModels
{
    public class NFDViewModel : PageViewModel
    {
        [Reactive] public bool MoveAll { get; set; } = true;

        public NFDViewModel() { }

        public async void OK()
        {
            await Open(async otoIni =>
            {
                // oto.iniのうちwavが存在しないかNFDのもの
                var otoWavs = otoIni.OtoList
                    .Select(oto => Path.Combine(otoIni.DirectoryPath, oto.FileName))
                    .Distinct()
                    .Where(path => !File.Exists(path) || Path.GetFileName(path) != Path.GetFileName(path).Normalize());
                var wavs = Directory.GetFiles(otoIni.DirectoryPath, "*.wav");
                var errors = new List<string>() { "ファイル名を書き換えることができませんでした：" };

                foreach (var otoWavPath in otoWavs)
                {
                    var otoWavName = Path.GetFileName(otoWavPath);
                    var nfdName = otoWavName.Normalize();

                    var wavPath = wavs.FirstOrDefault(wavPath => Path.GetFileName(wavPath).Normalize() == nfdName);
                    if (wavPath != null)
                    {
                        try
                        {
                            File.Move(wavPath, Path.Combine(otoIni.DirectoryPath, nfdName));
                        }
                        catch (Exception e)
                        {
                            DebugMode.AddError(e);
                            errors.Add(Path.GetFileName(wavPath));
                        }
                        otoIni.OtoList.ForEach(oto =>
                        {
                            if (oto.FileName == otoWavName)
                            {
                                oto.FileName = nfdName;
                            }
                        });
                    }
                }

                if (MoveAll)
                {
                    wavs = Directory.GetFiles(otoIni.DirectoryPath, "*.wav");
                    foreach (var wav in wavs)
                    {
                        if (Path.GetFileName(wav) != Path.GetFileName(wav).Normalize())
                        {

                            try
                            {
                                File.Move(wav, Path.Combine(otoIni.DirectoryPath, Path.GetFileName(wav).Normalize()));
                            }
                            catch (Exception e)
                            {
                                DebugMode.AddError(e);
                                errors.Add(Path.GetFileName(wav));
                            }
                        }
                    }
                }

                if (errors.Count > 1)
                {
                    await MainWindowViewModel.MessageDialogOpen(string.Join("\n", errors));
                }
                otoIni.Write();
            },
            "完了");
        }
    }
}
