using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OtoBatchEditor.ViewModels
{
    public class UnderScoreViewModel : PageViewModel
    {
        public UnderScoreViewModel() { }

        public async void OK()
        {
            await Open(async otoIni =>
            {
                var otoWavs = otoIni.OtoList
                    .Select(oto => Path.Combine(otoIni.DirectoryPath, oto.FileName))
                    .Distinct()
                    .Where(path => File.Exists(path) && !Path.GetFileName(path).StartsWith('_'))
                    .OrderByDescending(path => path.Length);
                var wavs = Directory.GetFiles(otoIni.DirectoryPath, "*.wav");
                var errors = new List<string>() { "ファイル名を書き換えることができませんでした：" };

                foreach (var otoWavPath in otoWavs)
                {
                    var otoWavName = Path.GetFileName(otoWavPath);

                    var wavPath = wavs.FirstOrDefault(wavPath => Path.GetFileName(wavPath) == otoWavName);
                    if (wavPath != null)
                    {
                        // "ファイル名.*"と"ファイル名_wav.*"を検索
                        var files = Directory.GetFiles(otoIni.DirectoryPath, $"{Path.GetFileNameWithoutExtension(otoWavPath)}.*")
                                    .Union(Directory.GetFiles(otoIni.DirectoryPath, $"{Path.GetFileNameWithoutExtension(otoWavPath)}_wav.*"));
                        foreach (var file in files)
                        {
                            try
                            {
                                File.Move(file, Path.Combine(otoIni.DirectoryPath, $"_{Path.GetFileName(file)}"));
                            }
                            catch (Exception e)
                            {
                                DebagMode.AddError(e);
                                errors.Add(Path.GetFileName(file));
                            }
                        }
                        otoIni.OtoList.ForEach(oto =>
                        {
                            if (oto.FileName == otoWavName)
                            {
                                oto.FileName = $"_{otoWavName}";
                            }
                        });
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
