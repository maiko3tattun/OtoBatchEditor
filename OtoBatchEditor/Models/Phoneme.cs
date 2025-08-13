using System.Collections.Generic;
using System.Linq;

namespace OtoBatchEditor
{
    public class Phoneme
    {
        public string Consonant { get; set; } = string.Empty;
        public string Kana { get; set; } = string.Empty;
        public int Length { get; set; } = 80;
        public bool IsPlosive { get; set; } = false;

        public Phoneme() { }
        public Phoneme(string consonant, string kana, int length = 80, bool isPlosive = false)
        {
            Consonant = consonant;
            Kana = kana;
            Length = length;
            IsPlosive = isPlosive;
        }

        public static List<Phoneme> Consonants { get; } = new List<Phoneme>()
        {
            new Phoneme("k", "く", 110, true),
            new Phoneme("t", "と", 110, true),
            new Phoneme("p", "ぷ", 110, true),
            new Phoneme("ch", "ち", 120, true),
            new Phoneme("ts", "つ", 120, true),

            new Phoneme("g", "ぐ", 110, true),
            new Phoneme("d", "ど", 110, true),
            new Phoneme("b", "ぶ", 110, true),

            new Phoneme("s", "す", 100),
            new Phoneme("sh", "し", 100),
            new Phoneme("z", "ず", 100),
            new Phoneme("j", "じ", 100),
            new Phoneme("f", "ふ", 80),
            new Phoneme("hy", "ひ", 100),
            new Phoneme("r", "る", 60),
            new Phoneme("v", "ヴ", 80),
            new Phoneme("n", "ぬ", 60),
            new Phoneme("m", "む", 60),
            new Phoneme("y", "ゆ", 60),
            new Phoneme("w", "わ", 60),

            new Phoneme("ky", "き", 120, true),
            new Phoneme("gy", "ぎ", 120, true),
            new Phoneme("ty", "てぃ", 110, true),
            new Phoneme("dy", "でぃ", 110, true),
            new Phoneme("by", "び", 110, true),
            new Phoneme("py", "ぴ", 110, true),
            new Phoneme("ry", "り", 80),
            new Phoneme("ny", "に", 60),
            new Phoneme("my", "み", 60),
            new Phoneme("h", "は", 80),
        };

        public static Phoneme? GetPhoneme(string consonant)
        {
            return Consonants.First(p => p.Consonant == consonant);
        }

        public override string ToString()
        {
            return Consonant;
        }

        public static char[] YouonKanas { get; } =
        {
            'ゃ',
            'ゅ',
            'ょ',
            'ぁ',
            'ぃ',
            'ぅ',
            'ぇ',
            'ぉ'
        };
    }
}
