using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Application = Microsoft.Office.Interop.Word.Application;

namespace Book_Reader
{
    class Document_Data
    {
        public static void LoadDocument()
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Word Documents|*.docx", ValidateNames = true, Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    ReadDocument(ofd.FileName);
                }

                else
                {
                    MessageBox.Show("You have not chosen a word document to load, please choose one to use this app.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static void ReadDocument(string FilePath)
        {
            string Password = Passwords.GetPassword(Path.GetFileNameWithoutExtension(FilePath));
            Images.GetImages(Path.GetFileNameWithoutExtension(FilePath));

            Application WordApp = new Application { Visible = false };
            Document Doc = WordApp.Documents.Open(FilePath, PasswordDocument: Password, ReadOnly: true);

            Range range = Doc.Range();
            range.Start = Doc.GoTo(What: WdGoToItem.wdGoToPage, Which: WdGoToDirection.wdGoToAbsolute, Count: 3).Start;

            GetParagraphs(range);
            GetHeadings(range);

            Doc.Close(SaveChanges: false);
            WordApp.Quit();
        }
        
        private static void GetParagraphs(Range range)
        {
            Paragraph p = range.Paragraphs[1];

            if (p.Range.Text.Contains("Disclaimer"))
            {
                p = range.Paragraphs[3];

                Text_Model.Disclaimer = p.Range.Text.Replace("\f", Environment.NewLine);
            }

            foreach (Paragraph paragraph in range.Paragraphs)
            {
                string ParagraphText;

                if (paragraph.Range.Font.Italic != 0)
                {
                    ParagraphText = GetFontStyle(paragraph);
                }

                else
                {
                    ParagraphText = paragraph.Range.Text;
                }

                string[] Lines = SplitText(ParagraphText, 81).ToArray();

                foreach (string Line in Lines)
                {
                    Text_Model.ChapterText.Add(Line.Replace("\f", Environment.NewLine));
                }
            }

        }

        private static List<string> SplitText(string Text, int CharacterLimit)
        {
            List<string> Lines = new List<string>();
            int StartIndex = 0;

            while (StartIndex < Text.Length)
            {
                int EndIndex = StartIndex + CharacterLimit;

                if (EndIndex >= Text.Length)
                {
                    Lines.Add(Text.Substring(StartIndex) + Environment.NewLine);
                    break;
                }

                int lastSpaceIndex = Text.LastIndexOf(' ', EndIndex);

                if (lastSpaceIndex != -1 && lastSpaceIndex >= StartIndex)
                {
                    Lines.Add(Text.Substring(StartIndex, lastSpaceIndex - StartIndex).Trim());
                    StartIndex = lastSpaceIndex + 1;
                }

                else
                {
                    Lines.Add(Text.Substring(StartIndex, CharacterLimit).Trim());
                    StartIndex += CharacterLimit;
                }
            }

            return Lines;
        }

        private static void GetHeadings(Range range)
        {
            foreach (Paragraph paragraph in range.Paragraphs)
            {
                if (IsHeading(paragraph))
                {
                    Text_Model.Chapters.Add(paragraph.Range.Text.Replace("\r", ""));
                }
            }
        }

        public static bool IsHeading(Paragraph paragraph)
        {
            string[] headingStyles = { "Heading 1", "Heading 2", "Heading 3" };

            foreach (string headingStyle in headingStyles)
            {
                if (paragraph.get_Style().NameLocal == headingStyle)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFontStyle(Paragraph paragraph)
        {
            if (!string.IsNullOrWhiteSpace(paragraph.Range.Text))
            {
                string[] Words = paragraph.Range.Text.Split(' ');
                string Paragraph = null;

                foreach (string Word in Words)
                {
                    int WordStart = paragraph.Range.Start + paragraph.Range.Text.IndexOf(Word);
                    int WordLength = Word.Length;

                    Range WordRange = paragraph.Range.Duplicate;
                    WordRange.SetRange(WordStart, WordStart + Word.Length);

                    if (WordRange.Font.Italic != 0)
                    {
                        Paragraph += $"^{WordRange.Text} ";
                    }

                    else
                    {
                        Paragraph += $"{WordRange.Text} ";
                    }

                    WordRange.Delete();
                }

                return Paragraph.TrimEnd();
            }

            else
            {
                return string.Empty;
            }
        }
    }
}
