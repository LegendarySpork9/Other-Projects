using System.Collections.Generic;

namespace Book_Reader
{
    class Text_Model
    {
        public static string ImageLocation { get; set; }
        public static string Disclaimer { get; set; }
        public static List<string> Chapters = new List<string>();
        public static List<string> ChapterText = new List<string>();
        public static List<bool> BoldWord = new List<bool>();
        public static List<bool> ItalicWord = new List<bool>();
        public static List<string> Pages = new List<string>();
    }
}
