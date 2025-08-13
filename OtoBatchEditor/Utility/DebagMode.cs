using OtoBatchEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OtoBatchEditor
{
    public static class DebagMode
    {
        private static string ProcessName => Path.GetFileNameWithoutExtension(Environment.ProcessPath);
        private static string DirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Maiko", ProcessName, "Logs");
        public static List<string> Log { get; private set; } = new List<string>();
        public static bool DebagModeIsEnable { get; private set; } = false;

        /// <summary>
        /// デバッグモードオンオフ
        /// </summary>
        public static void ToggleLogExport()
        {
            DebagModeIsEnable = !DebagModeIsEnable;
        }

        public static void AddError(Exception e)
        {
            if (e.InnerException != null)
            {
                e = e.InnerException;
            }
            if (e is AggregateException ae)
            {
                e = ae.Flatten();
            }
            Log.Add(e.Message);
            Log.Add(e.StackTrace!);
            Log.Add(string.Empty);
            Debug.Write(e.Message);
            Debug.Write(e.StackTrace);
        }
        public static void AddError(string str)
        {
            Log.Add(str);
            Log.Add(string.Empty);
            Debug.Write(str);
        }
        public static void AddError(string str, Exception e)
        {
            if (e.InnerException != null)
            {
                e = e.InnerException;
            }
            if (e is AggregateException ae)
            {
                e = ae.Flatten();
            }
            Log.Add(str);
            Log.Add(e.StackTrace!);
            Log.Add(string.Empty);
            Debug.Write(e.Message);
            Debug.Write(e.StackTrace);
        }

        public static async Task Export(LogOutputType type)
        {
            if (!DebagModeIsEnable && type != LogOutputType.Manual)
            {
                return;
            }
            switch (type)
            {
                case LogOutputType.Manual:
                    Log.Add("Manually output log...");
                    break;
                case LogOutputType.Completed:
                    Log.Add("Complete and output log...");
                    break;
                case LogOutputType.Error:
                    Log.Add("Output log with exception...");
                    break;
            }
            await Write();
        }

        private static async Task Write()
        {
            DateTime dt = DateTime.Now;
            string filename = $"debag_{ProcessName}_{dt.ToString("yyyyMMdd_HHmmss")}.txt";
            string filePath = Path.Combine(DirectoryPath, filename);

            try
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                var lines = new List<string>();

                // 実行パス
                lines.Add("-- Path");
                lines.Add($"Process Path: {Environment.ProcessPath}: {HasAccessPermissionToFolder(Environment.ProcessPath)}");
                lines.Add($"Preset Directory: {Preset.DirectoryPath}: {HasAccessPermissionToFolder(Preset.DirectoryPath)}");

                // oto.ini
                lines.Add("\n-- oto.ini");
                try
                {
                    foreach (var ini in OtoIni.GetOtoIniList())
                    {
                        lines.Add($"{ini.FilePath}: {HasAccessPermissionToFolder(ini.FilePath)}");
                    }
                }
                catch (MinorException)
                {
                    lines.Add("No oto loaded");
                }
                catch (Exception ex)
                {
                    AddError(ex);
                }

                // Log
                lines.Add("\n-- Log");
                lines.AddRange(Log);

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
                MainWindowViewModel.ShowSnackbar("出力に成功しました");
            }
            catch (Exception ex)
            {
                await MainWindowViewModel.MessageDialogOpen($"出力に失敗しました\n{ex.Message}");
                return;
            }
            try
            {
                // フォルダを開く
                if (OperatingSystem.IsWindows())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer",
                        Arguments = $"\"{DirectoryPath}\"",
                        UseShellExecute = true
                    });
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", $"\"{DirectoryPath}\"");
                }
            }
            catch (Exception ex)
            {
                await MainWindowViewModel.MessageDialogOpen($"フォルダを開けませんでした\n{ex.Message}");
                return;
            }
        }

        /// <summary>
        /// フォルダ内に書き込み権限があるか確認<br />
        /// パスがファイルの場合、親ディレクトリで判定
        /// </summary>
        public static AccessPermission HasAccessPermissionToFolder(string? folderPath)
        {
            string? path = folderPath;
            if (string.IsNullOrEmpty(path))
            {
                return AccessPermission.NotExist;
            }
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(folderPath)!;
            }
            if (!Directory.Exists(path))
            {
                return AccessPermission.NotExist;
            }

            string filePath = Path.Combine(path, "test");
            var writeAccess = AccessPermission.Unauthorized;

            try
            {
                using (File.Create(filePath)) { }
                writeAccess = AccessPermission.Authorized;
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return writeAccess;
        }
    }

    public class MinorException : Exception
    {
        public MinorException() { }
        public MinorException(string message) : base(message) { }
    }

    public enum LogOutputType
    {
        Manual,
        Completed,
        Error
    }

    public enum AccessPermission
    {
        Authorized,
        Unauthorized,
        NotExist
    }
}
