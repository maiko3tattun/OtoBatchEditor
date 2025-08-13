using ReactiveUI.Fody.Helpers;
using System;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class RoundViewModel : PageViewModel
    {
        [Reactive] public bool Offset { get; set; } = true;
        [Reactive] public bool Pre { get; set; } = true;
        [Reactive] public bool Ovl { get; set; } = true;
        [Reactive] public bool Consonant { get; set; } = true;
        [Reactive] public bool Blank { get; set; } = true;

        public RoundViewModel() { }

        public async void OK()
        {
            if (!Offset && !Pre && !Ovl && !Consonant && !Blank)
            {
                await MainWindowViewModel.MessageDialogOpen("1つ以上選択してください");
                return;
            }

            await Edit(otoIni =>
            {
                try
                {
                    otoIni.OtoList.ForEach(oto =>
                    {
                        if (Offset)
                        {
                            oto.Offset = Math.Round(oto.Offset);
                        }
                        if (Pre)
                        {
                            oto.Pre = Math.Round(oto.Pre);
                        }
                        if (Ovl)
                        {
                            oto.Ovl = Math.Round(oto.Ovl);
                        }
                        if (Consonant)
                        {
                            oto.Consonant = Math.Round(oto.Consonant);
                        }
                        if (Blank)
                        {
                            oto.Blank = Math.Round(oto.Blank);
                        }
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
