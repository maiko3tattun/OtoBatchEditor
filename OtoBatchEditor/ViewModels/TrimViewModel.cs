using System;
using System.Threading.Tasks;

namespace OtoBatchEditor.ViewModels
{
    public class TrimViewModel : PageViewModel
    {
        public TrimViewModel() { }

        public async void OK()
        {
            await Edit(otoIni =>
            {
                try
                {
                    otoIni.OtoList.ForEach(oto =>
                    {
                        oto.Alias = oto.Alias.Trim(' ', '　');
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
