using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace OtoBatchEditor
{
    public class Wav// : IEquatable<Wav>
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileNameNFD { get; set; } = string.Empty;
        public bool? Exist { get; set; } = null;
        public double LengthMs { get; private set; } = 0;
        public WaveFormat? WaveFormat { get; private set; }
        public List<string> Errors { get; } = new List<string>();

        public Wav(string path, bool? exist = null)
        {
            FilePath = path;
            FileName = Path.GetFileName(path);
            FileNameNFD = FileName.Normalize();
            Exist = exist;
        }

        public bool TryRead()
        {
            if (Exist == false)
            {
                Errors.Add("ファイルが存在しません");
                return false;
            }
            try
            {
                using (var waveStream = new WaveFileReader(FilePath))
                {
                    WaveFormat = waveStream.WaveFormat;
                    TimeSpan duration = waveStream.TotalTime;
                    LengthMs = duration.TotalMilliseconds;
                }
                Exist = true;
                return true;
            }
            catch (Exception e)
            {
                DebagMode.AddError(e);
                Errors.Add($"ファイルの読み込みに失敗しました：{e.Message}");
                return false;
            }
        }

        //public bool Equals(Wav? other)
        //{
        //    if (other is null) return false;
        //    if (ReferenceEquals(this, other)) return true;
        //    return string.Equals(FilePath, other.FilePath);
        //}
    }
}
