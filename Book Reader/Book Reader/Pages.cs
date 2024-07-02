namespace Book_Reader
{
    internal class Pages
    {
        public static string CurrentPage(string Text)
        {
            if (!Text_Model.Pages.Contains(Text.Replace("\n", "~")))
            {
                Text_Model.Pages.Add(Text.Replace("\n", "~"));
                return "run RemovePreviousPage";
            }

            else
            {
                return "run LoadPage";
            }
        }

        public static void RemovePreviousPage(string LastLine, int LastLineAddition)
        {
            int LineCount = Text_Model.ChapterText.IndexOf(LastLine) + LastLineAddition + 1;

            Text_Model.ChapterText.RemoveRange(0, LineCount);
        }

        public static string LoadPage(string Text)
        {
            return Text.Replace("~", "\n");
        }
    }
}
