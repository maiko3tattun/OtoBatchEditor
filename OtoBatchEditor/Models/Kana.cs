using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OtoBatchEditor.Models
{
    public class Kana
    {
        public string KanaChar { get; set; }
        public string Consonant { get; set; }
        public string Vowel { get; set; }
        public double Preutter { get; set; } = 50;
        public double Overlap { get; set; } = 20;

        public Kana(string kanaChar, string consonant, string vowel)
        {
            KanaChar = kanaChar;
            Consonant = consonant;
            Vowel = vowel;
            SetParams();
        }

        private static List<Kana> kanaList = new List<Kana>()
        {
            new Kana("あ", "", "a"),
            new Kana("い", "", "i"),
            new Kana("う", "", "u"),
            new Kana("え", "", "e"),
            new Kana("お", "", "o"),
            new Kana("か", "k", "a"),
            new Kana("き", "ky", "i"),
            new Kana("く", "k", "u"),
            new Kana("け", "k", "e"),
            new Kana("こ", "k", "o"),
            new Kana("さ", "s", "a"),
            new Kana("し", "sh", "i"),
            new Kana("す", "s", "u"),
            new Kana("せ", "s", "e"),
            new Kana("そ", "s", "o"),
            new Kana("た", "t", "a"),
            new Kana("ち", "ch", "i"),
            new Kana("つ", "ts", "u"),
            new Kana("て", "t", "e"),
            new Kana("と", "t", "o"),
            new Kana("な", "n", "a"),
            new Kana("に", "ny", "i"),
            new Kana("ぬ", "n", "u"),
            new Kana("ね", "n", "e"),
            new Kana("の", "n", "o"),
            new Kana("は", "h", "a"),
            new Kana("ひ", "hy", "i"),
            new Kana("ふ", "f", "u"),
            new Kana("へ", "h", "e"),
            new Kana("ほ", "h", "o"),
            new Kana("ま", "m", "a"),
            new Kana("み", "my", "i"),
            new Kana("む", "m", "u"),
            new Kana("め", "m", "e"),
            new Kana("も", "m", "o"),
            new Kana("や", "y", "a"),
            new Kana("ゆ", "y", "u"),
            new Kana("いぇ", "y", "e"),
            new Kana("よ", "y", "o"),
            new Kana("ら", "r", "a"),
            new Kana("り", "ry", "i"),
            new Kana("る", "r", "u"),
            new Kana("れ", "r", "e"),
            new Kana("ろ", "r", "o"),
            new Kana("わ", "w", "a"),
            new Kana("うぃ", "w", "i"),
            new Kana("うぅ", "w", "u"),
            new Kana("うぇ", "w", "e"),
            new Kana("うぉ", "w", "o"),
            new Kana("を", "", "o"),
            new Kana("ん", "", "n"),
            new Kana("が", "g", "a"),
            new Kana("ぎ", "gy", "i"),
            new Kana("ぐ", "g", "u"),
            new Kana("げ", "g", "e"),
            new Kana("ご", "g", "o"),
            new Kana("ざ", "z", "a"),
            new Kana("じ", "j", "i"),
            new Kana("ず", "z", "u"),
            new Kana("ぜ", "z", "e"),
            new Kana("ぞ", "z", "o"),
            new Kana("だ", "d", "a"),
            new Kana("ぢ", "j", "i"),
            new Kana("づ", "z", "u"),
            new Kana("で", "d", "e"),
            new Kana("ど", "d", "o"),
            new Kana("ば", "b", "a"),
            new Kana("び", "by", "i"),
            new Kana("ぶ", "b", "u"),
            new Kana("べ", "b", "e"),
            new Kana("ぼ", "b", "o"),
            new Kana("ぱ", "p", "a"),
            new Kana("ぴ", "py", "i"),
            new Kana("ぷ", "p", "u"),
            new Kana("ぺ", "p", "e"),
            new Kana("ぽ", "p", "o"),
            new Kana("ガ", "ng", "a"),
            new Kana("ギ", "ng", "i"),
            new Kana("グ", "ng", "u"),
            new Kana("ゲ", "ng", "e"),
            new Kana("ゴ", "ng", "o")
        };
        private static Dictionary<string,string> komoji = new Dictionary<string, string>()
        {
            { "ぁ", "a" },
            { "ぃ", "i" },
            { "ぅ", "u" },
            { "ぇ", "e" },
            { "ぉ", "o" },
            { "ゃ", "a" },
            { "ゅ", "u" },
            { "ょ", "o" }
        };
        public static Dictionary<string, Kana> KanaDict { get; } = kanaList.ToDictionary(k => k.KanaChar, k => k);

        private void SetParams()
        {
            switch (Consonant)
            {
                case "":
                    Preutter = 5;
                    Overlap = 0;
                    break;
                case "k":
                case "t":
                case "p":
                case "py":
                    Preutter = 60;
                    Overlap = 0;
                    break;
                case "ky":
                    Preutter = 80;
                    Overlap = 0;
                    break;
                case "ch":
                case "ts":
                    Preutter = 100;
                    Overlap = 0;
                    break;
                case "r":
                case "g":
                case "b":
                case "d":
                    Preutter = 30;
                    Overlap = 10;
                    break;
                case "ry":
                case "gy":
                case "by":
                case "dy":
                    Preutter = 40;
                    Overlap = 15;
                    break;
                case "y":
                case "w":
                    Preutter = 50;
                    Overlap = 20;
                    break;
                case "n":
                case "ny":
                case "m":
                case "my":
                case "h":
                case "f":
                case "z":
                case "j":
                    Preutter = 60;
                    Overlap = 20;
                    break;
                case "s":
                case "sh":
                case "hy":
                    Preutter = 80;
                    Overlap = 30;
                    break;
            }
        }

        public static bool TryGetKana(string kanaChar, out Kana? kana)
        {
            kana = null;
            string? key = kanaChar;
            // 子音が判定できない「うぉ」等を先に処理
            if (Regex.IsMatch(kanaChar, "[あいうえおん]"))
            {
                key = KanaDict.Keys.OrderByDescending(key => key.Length).FirstOrDefault(key => kanaChar.Contains(key));
                return key != null && KanaDict.TryGetValue(key, out kana);
            }
            // 拗音は1文字目から子音を、2文字目から母音を判定
            foreach (var pair in komoji)
            {
                if (kanaChar.Contains(pair.Key))
                {
                    key = kanaChar.Substring(0, 1);
                    if (KanaDict.TryGetValue(key, out var con))
                    {
                        kana = new Kana(key + pair.Key, con.Consonant, pair.Value);
                        return true;
                    }
                }
            }
            // その他
            key = KanaDict.Keys.OrderByDescending(key => key.Length).FirstOrDefault(key => kanaChar.Contains(key));
            return key != null && KanaDict.TryGetValue(key, out kana);
        }
    }
}
