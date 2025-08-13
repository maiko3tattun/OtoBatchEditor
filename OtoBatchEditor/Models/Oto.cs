using System.Collections.Generic;
using System.IO;

namespace OtoBatchEditor
{
    public class Oto
    {
        public string DirectoryPath { get; } = string.Empty;
        public string OriginalLine { get; } = string.Empty;
        public int LineNumber { get; set; } = -1;
        public string FileName { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public double Offset { get; set; } = 0;
        public double Pre { get; set; } = 0;
        public double Ovl { get; set; } = 0;
        public double Consonant { get; set; } = 0;
        public double Blank { get; set; } = 0;
        public OtoStatus Status { get; private set; } = OtoStatus.Valid;
        public List<string> Error { get; private set; } = new List<string>();
        public bool IsNotChanged => OriginalLine == ToString();

        public Oto() { }
        public Oto(string line, int lineNumber, string path)
        {
            DirectoryPath = path;
            OriginalLine = line;
            LineNumber = lineNumber;

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';') || line == "#Charset:UTF-8") // コメント行
            {
                Status = OtoStatus.CommentOrEmpty;
                return;
            }
            string[] tmp = line.Split('=');
            if (tmp.Length != 2)
            {
                Error.Add("フォーマットに不備があります");
                Status = OtoStatus.CommentOrEmpty;
                return;
            }
            string[] splitData = tmp[1].Split(',');
            if (splitData.Length != 6 && splitData.Length != 7) // Todo 後方固定範囲
            {
                Error.Add("フォーマットに不備があります");
                Status = OtoStatus.CommentOrEmpty;
                return;
            }

            FileName = tmp[0];
            if (!string.IsNullOrWhiteSpace(splitData[0]))
            {
                Alias = splitData[0];
            }
            else
            {
                Alias = Path.GetFileNameWithoutExtension(FileName);
            }

            try
            {
                Offset = double.Parse(splitData[1]);
            }
            catch
            {
                Error.Add("左ブランクの値が不正です");
                Status = OtoStatus.Invalid;
                Offset = 0;
            }
            try
            {
                Consonant = double.Parse(splitData[2]);
            }
            catch
            {
                Error.Add("子音範囲の値が不正です");
                Status = OtoStatus.Invalid;
                Consonant = 0;
            }
            try
            {
                Blank = double.Parse(splitData[3]);
            }
            catch
            {
                Error.Add("右ブランクの値が不正です");
                Status = OtoStatus.Invalid;
                Blank = 0;
            }
            try
            {
                Pre = double.Parse(splitData[4]);
            }
            catch
            {
                Error.Add("先行発声の値が不正です");
                Status = OtoStatus.Invalid;
                Pre = 0;
            }
            try
            {
                Ovl = double.Parse(splitData[5]);
            }
            catch
            {
                Error.Add("オーバーラップの値が不正です");
                Status = OtoStatus.Invalid;
                Ovl = 0;
            }
        }
        public Oto(string fileName, string alias, double offset, double consonant, double blank, double pre, double ovl)
        {
            FileName = fileName;
            Alias = alias;
            Offset = offset;
            Consonant = consonant;
            Blank = blank;
            Pre = pre;
            Ovl = ovl;
        }

        public Oto Clone()
        {
            return new Oto()
            {
                FileName = this.FileName,
                Alias = this.Alias,
                Offset = this.Offset,
                Consonant = this.Consonant,
                Pre = this.Pre,
                Ovl = this.Ovl,
                Status = this.Status
            };
        }

        public override string ToString()
        {
            if (Status == OtoStatus.CommentOrEmpty)
            {
                return OriginalLine;
            }
            else
            {
                return $"{FileName}={Alias},{Offset},{Consonant},{Blank},{Pre},{Ovl}";
            }
        }
    }

    public enum OtoStatus
    {
        Valid,
        Invalid,
        CommentOrEmpty
    }
}
