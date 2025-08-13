using System.Collections.Generic;
using System.Linq;

namespace OtoBatchEditor.Models
{
    public class NoteNum
    {
        public static Dictionary<string, int> NoteNumDict = new Dictionary<string, int>
        {
            { "C2", 36 },
            { "C#2", 37 },
            { "D2", 38 },
            { "D#2", 39 },
            { "E2", 40 },
            { "F2", 41 },
            { "F#2", 42 },
            { "G2", 43 },
            { "G#2", 44 },
            { "A2", 45 },
            { "A#2", 46 },
            { "B2", 47 },
            { "C3", 48 },
            { "C#3", 49 },
            { "D3", 50 },
            { "D#3", 51 },
            { "E3", 52 },
            { "F3", 53 },
            { "F#3", 54 },
            { "G3", 55 },
            { "G#3", 56 },
            { "A3", 57 },
            { "A#3", 58 },
            { "B3", 59 },
            { "C4", 60 },
            { "C#4", 61 },
            { "D4", 62 },
            { "D#4", 63 },
            { "E4", 64 },
            { "F4", 65 },
            { "F#4", 66 },
            { "G4", 67 },
            { "G#4", 68 },
            { "A4", 69 },
            { "A#4", 70 },
            { "B4", 71 },
            { "C5", 72 },
            { "C#5", 73 },
            { "D5", 74 },
            { "D#5", 75 },
            { "E5", 76 },
            { "F5", 77 },
            { "F#5", 78 },
            { "G5", 79 },
            { "G#5", 80 },
            { "A5", 81 },
            { "A#5", 82 },
            { "B5", 83 },
            { "C6", 84 },
            { "C#6", 85 },
            { "D6", 86 },
            { "D#6", 87 },
            { "E6", 88 },
            { "F6", 89 },
            { "F#6", 90 },
            { "G6", 91 },
            { "G#6", 92 },
            { "A6", 93 },
            { "A#6", 94 },
            { "B6", 95 },
        };

        public static IEnumerable<string> Pitches => NoteNumDict.Keys.OrderByDescending(key => key.Length);
    }
}
