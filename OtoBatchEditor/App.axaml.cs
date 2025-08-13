using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OtoBatchEditor.ViewModels;
using OtoBatchEditor.Views;
using System.IO;
using System.Text;

namespace OtoBatchEditor
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var vm = new MainWindowViewModel();
                if (desktop.Args != null && desktop.Args.Length > 1 && Path.GetFileName(desktop.Args[0]) == "oto-autoEstimation.ini")
                {
                    OtoIni.Args = desktop.Args[0];
                }
                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm
                };

            }

            base.OnFrameworkInitializationCompleted();
        }

    }
}