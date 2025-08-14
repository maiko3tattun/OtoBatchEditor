using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OtoBatchEditor
{
    public class OtoIni
    {
        private static ObservableCollectionExtended<string>? otoIniList;
        public static string? Args { get; set; }
        public static bool IsPlugin => Args != null && !string.IsNullOrEmpty(Args);

        public string FilePath { get; } = string.Empty;
        public string DirectoryPath { get; } = string.Empty;
        private List<Oto> otoList { get; } = new List<Oto>();
        public List<Oto> OtoList { get => otoList.Where(oto => oto.Status != OtoStatus.CommentOrEmpty).ToList(); }
        public Encoding Encoding { get; set; } = Encoding.GetEncoding("Shift_JIS");
        public bool Changed { get; set; } = false;

        private string? _suffix;
        public string Suffix
        {
            get
            {
                if (_suffix != null) return _suffix;
                _suffix = GetSuffix();
                return _suffix;
            }
        }

        public OtoIni() { }
        public OtoIni(string filePath)
        {
            FilePath = filePath;
            DirectoryPath = Path.GetDirectoryName(filePath)!;
        }

        public static void SetOtoIniList(ObservableCollectionExtended<string> list)
        {
            otoIniList = list;
        }

        public static OtoIni[] GetOtoIniList()
        {
            if (otoIniList == null || otoIniList.Count <= 0)
            {
                throw new MinorException("左のパネルで処理するoto.iniを読み込んでください");
            }
            return otoIniList.Select(otoPath => new OtoIni(otoPath)).ToArray();
        }

        /// <summary>
        /// oto.iniを読み、Invalidなotoがあればfalseを返す。
        /// </summary>
        public bool Read()
        {
            try
            {
                string[] lines = File.ReadAllLines(FilePath, Encoding);
                if (lines.Length > 0 && lines[0] == "#Charset:UTF-8")
                {
                    lines = File.ReadAllLines(FilePath, Encoding.UTF8);
                    Encoding = Encoding.UTF8;
                }

                for (int i = 0; i < lines.Length; i++)
                {
                    Oto oto = new Oto(lines[i], i, DirectoryPath);
                    otoList.Add(oto);
                }
                if (otoList.Any(oto => oto.Status == OtoStatus.Invalid))
                {
                    return false;
                }
                return true;
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException($"{FilePath} が存在しません。", e);
            }
            catch (IOException e)
            {
                throw new IOException($"{FilePath} の読み込みに失敗しました。", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException($"{FilePath} の読み込みに失敗しました。アクセス権限がありません。", e);
            }
            catch (Exception e)
            {
                throw new Exception($"{FilePath} の読み込みに失敗しました。予期せぬエラーが発生しました。", e);
            }
        }

        public void Copy(bool overwrite = true, string fileName = "oto_backup.ini")
        {
            string backup = FilePath;
            try
            {
                backup = Path.Combine(DirectoryPath, fileName);
                if (!overwrite && File.Exists(backup))
                {
                    throw new MinorException($"{backup} は\nすでにあるのでスキップします");
                }
                File.Copy(FilePath, backup, overwrite);
            }
            catch (MinorException)
            {
                throw;
            }
            catch (IOException e)
            {
                throw new IOException($"{backup} の保存に失敗しました。", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException($"{backup} の保存に失敗しました。アクセス権限がありません。", e);
            }
            catch (Exception e)
            {
                throw new Exception($"{backup} の保存に失敗しました。予期せぬエラーが発生しました。", e);
            }
        }

        /// <summary>
        /// oto.iniを保存する。変更がなければfalseを返す。
        /// </summary>
        public bool Write()
        {
            if (!Changed && otoList.All(oto => oto.IsNotChanged))
            {
                return false;
            }
            Copy();
            try
            {
                otoList.ForEach(oto => oto.Round());
                File.WriteAllLines(FilePath, otoList.Select(oto => oto.ToString()), Encoding);
                return true;
            }
            catch (IOException e)
            {
                throw new IOException($"{FilePath} の書き込みに失敗しました。", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException($"{FilePath} の書き込みに失敗しました。アクセス権限がありません。", e);
            }
            catch (Exception e)
            {
                throw new Exception($"{FilePath} の書き込みに失敗しました。予期せぬエラーが発生しました。", e);
            }
        }

        public void AddRange(IEnumerable<Oto> otos)
        {
            otoList.AddRange(otos);
            Changed = true;
        }

        public void RemoveAll(Predicate<Oto> match)
        {
            otoList.RemoveAll(match);
            Changed = true;
        }

        private string GetSuffix()
        {
            try
            {
                if (otoList.Count <= 3)
                {
                    return string.Empty;
                }
                var SuffixCandidate = new Dictionary<string, int>();

                for (int n = 0; n < 100; n++) // 100行ループ
                {
                    if (otoList.Count <= n) break;
                    string alias = otoList[n].Alias;

                    // 1行目と後ろから照らし合わせる
                    for (int i = 1; i < otoList[0].Alias.Length; i++)
                    {
                        string str = otoList[0].Alias.Substring(i);
                        if (alias.Contains(str))
                        {
                            if (SuffixCandidate.ContainsKey(str))
                            {
                                SuffixCandidate[str]++;
                            }
                            else
                            {
                                SuffixCandidate.Add(str, 0);
                            }
                            break;
                        }
                    }

                    // 1/3行目と後ろから照らし合わせる
                    int line = otoList.Count / 3;
                    for (int i = 1; i < otoList[line].Alias.Length; i++)
                    {
                        string str = otoList[line].Alias.Substring(i);
                        if (alias.Contains(str))
                        {
                            if (SuffixCandidate.ContainsKey(str))
                            {
                                SuffixCandidate[str]++;
                            }
                            else
                            {
                                SuffixCandidate.Add(str, 0);
                            }
                            break;
                        }
                    }

                    // 2/3行目と後ろから照らし合わせる
                    line = otoList.Count * 2 / 3;
                    for (int i = 1; i < otoList[line].Alias.Length; i++)
                    {
                        string str = otoList[line].Alias.Substring(i);
                        if (alias.Contains(str))
                        {
                            if (SuffixCandidate.ContainsKey(str))
                            {
                                SuffixCandidate[str]++;
                            }
                            else
                            {
                                SuffixCandidate.Add(str, 0);
                            }
                            break;
                        }
                    }
                }
                KeyValuePair<string, int> max = SuffixCandidate.OrderByDescending(val => val.Value).First();
                if (max.Value <= otoList.Count / 2 && max.Value < 150)
                {
                    return string.Empty;
                }
                return max.Key;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
