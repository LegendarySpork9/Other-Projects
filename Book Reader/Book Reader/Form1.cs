using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Speech.Synthesis;
using Application = System.Windows.Forms.Application;
using Font = System.Drawing.Font;
using System.Linq;

namespace Book_Reader
{
    public partial class Book_Reader : Form
    {
        private int PageNumber = 0;
        private string LastLine;
        private int LastLineAddition;
        private int PictureNumber = 0;
        private string CurrentChapter;
        private bool TTSFlag = false;
        private string TTSChapter;
        readonly List<int> ItalicIndexes = new List<int>();
        readonly List<int> ItalicLengths = new List<int>();
        readonly SpeechSynthesizer Speech = new SpeechSynthesizer();

        public Book_Reader()
        {
            this.Icon = Icon.FromHandle(Properties.Resources.Book.GetHicon());
            InitializeComponent();
            LBIgnore.Select();
            PBTTS.Enabled = false;

            Speech.SetOutputToDefaultAudioDevice();
        }

        private void TruncateText(List<string> Text)
        {
            string[] Lines = Text.ToArray();
            TTSChapter = null;

            for (int i = 0; i < 29; i++)
            {
                if (i < Lines.Count())
                {
                    string line = Lines[i];
                    LastLine = Lines[i];
                    LastLineAddition = 0;

                    if (line.Length > RTBChapterText.Width)
                    {
                        int lastSpaceIndex = line.LastIndexOf(' ', RTBChapterText.Width);

                        if (lastSpaceIndex != -1)
                        {
                            line = line.Substring(0, lastSpaceIndex);
                        }
                    }

                    if (i == 28)
                    {
                        RTBChapterText.AppendText(line);
                        TTSChapter += $"{line.Replace("^", "")} ";
                    }

                    else
                    {
                        RTBChapterText.AppendText(line + Environment.NewLine);
                        TTSChapter += $"{line.Replace("^", "")} ";
                    }

                    if (string.IsNullOrWhiteSpace(LastLine) && i > 0)
                    {
                        LastLine = Lines[i - 1];
                        LastLineAddition = 1;
                    }
                }
            }
        }

        private new void Closing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Open(object sender, EventArgs e)
        {
            PBOpen.Enabled = false;
            Document_Data.LoadDocument();
            TruncateText(Text_Model.ChapterText);
            GetItalicWords();
            FormatText();

            TMCycleImage.Start();
            PBOpen.Enabled = true;
            PBTTS.Enabled = true;
            BTBack.Enabled = true;
            BTNext.Enabled = true;
        }

        private void Deselect(object sender, EventArgs e)
        {
            LBIgnore.Select();
        }

        private void GetItalicWords()
        {
            RTBChapterText.WordWrap = false;

            for (int x = 0; x < RTBChapterText.Lines.Length; x++)
            {
                if (RTBChapterText.Lines[x].Contains("^"))
                {
                    string[] Words = RTBChapterText.Lines[x].Split(' ');

                    foreach (string Word in Words)
                    {
                        if (Word.Contains("^"))
                        {
                            ItalicIndexes.Add(RTBChapterText.Text.IndexOf("^"));
                            ItalicLengths.Add(Word.Length - 1);

                            RTBChapterText.Text = RTBChapterText.Text.Remove(RTBChapterText.Text.IndexOf("^"), 1);
                        }
                    }
                }
            }

            RTBChapterText.WordWrap = true;
        }

        private void FormatText()
        {

            RTBChapterText.WordWrap = false;

            for (int x = 0; x < RTBChapterText.Lines.Length; x++)
            {
                if (x == 0)
                {
                    if (Text_Model.Chapters.Contains(RTBChapterText.Lines[x].ToString()) && RTBChapterText.Lines[x + 1].ToString() == "")
                    {
                        int LineStart = RTBChapterText.GetFirstCharIndexFromLine(x);

                        RTBChapterText.Select(LineStart, RTBChapterText.Lines[x].Length);
                        RTBChapterText.SelectionFont = new Font(RTBChapterText.Font, FontStyle.Bold | FontStyle.Underline);

                        CurrentChapter = RTBChapterText.Lines[x].ToString();
                    }
                }

                else if (RTBChapterText.Lines[x - 1].ToString() == "" && Text_Model.Chapters.Contains(RTBChapterText.Lines[x].ToString()) && RTBChapterText.Lines[x + 1].ToString() == "")
                {
                    int LineStart = RTBChapterText.GetFirstCharIndexFromLine(x);

                    RTBChapterText.Select(LineStart, RTBChapterText.Lines[x].Length);
                    RTBChapterText.SelectionFont = new Font(RTBChapterText.Font, FontStyle.Bold | FontStyle.Underline);

                    CurrentChapter = RTBChapterText.Lines[x].ToString();
                }

                else if (Text_Model.Disclaimer != null && Text_Model.Disclaimer.Contains(RTBChapterText.Lines[x].ToString()))
                {
                    int LineStart = RTBChapterText.GetFirstCharIndexFromLine(x);

                    RTBChapterText.Select(LineStart, RTBChapterText.Lines[x].Length);
                    RTBChapterText.SelectionColor = Color.Red;
                    RTBChapterText.SelectionFont = new Font(RTBChapterText.Font.FontFamily, 9, FontStyle.Bold);
                }
            }

            for (int x = 0; x != ItalicIndexes.Count; x++)
            {
                RTBChapterText.Select(ItalicIndexes[x], ItalicLengths[x]);
                RTBChapterText.SelectionFont = new Font(RTBChapterText.Font, FontStyle.Italic);
            }

            RTBChapterText.WordWrap = true;
        }

        private string StoreItalicWords()
        {
            RTBChapterText.WordWrap = false;
            string Page = RTBChapterText.Text;

            for (int x = (ItalicIndexes.Count - 1); x >= 0; x--)
            {
                Page = Page.Insert(ItalicIndexes[x], "^");
            }

            ItalicIndexes.Clear();
            ItalicLengths.Clear();

            RTBChapterText.WordWrap = true;
            return Page;
        }

        private void Back(object sender, EventArgs e)
        {
            if (PageNumber == 0)
            {
                MessageBox.Show("There are no previous pages, you are at the start of the book.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                PageNumber--;
                RTBChapterText.Clear();
                ItalicIndexes.Clear();
                ItalicLengths.Clear();
                RTBChapterText.Text = Pages.LoadPage(Text_Model.Pages[PageNumber]);

                GetItalicWords();
                FormatText();
            }
        }

        private void Next(object sender, EventArgs e)
        {
            if (Text_Model.ChapterText.Count() == 0 || Text_Model.ChapterText.Count() == Text_Model.ChapterText.IndexOf(LastLine) + 1)
            {
                MessageBox.Show("There are no more pages, you are at the end of the book.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                string Page = StoreItalicWords();

                if (Pages.CurrentPage(Page) == "run RemovePreviousPage")
                {
                    Pages.RemovePreviousPage(LastLine, LastLineAddition);

                    RTBChapterText.Clear();
                    TruncateText(Text_Model.ChapterText);
                    GetItalicWords();
                    FormatText();

                    PageNumber++;
                }

                else
                {
                    if (Text_Model.Pages.Count == (PageNumber + 1))
                    {
                        RTBChapterText.Clear();
                        TruncateText(Text_Model.ChapterText);
                        GetItalicWords();
                        FormatText();

                        PageNumber++;
                    }

                    else
                    {
                        PageNumber++;
                        RTBChapterText.Clear();
                        RTBChapterText.Text = Pages.LoadPage(Text_Model.Pages[PageNumber]);

                        GetItalicWords();
                        FormatText();
                    }
                }
            }
        }

        private void CycleImage(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Text_Model.ImageLocation))
            {
                TMCycleImage.Stop();
                TMCycleImage.Interval = 5000;
                PictureNumber++;

                if (PictureNumber == 4)
                {
                    if (PBImages?.Image != null)
                    {
                        PBImages.Image.Dispose();
                    }

                    PBImages.Image = Image.FromFile(Images.GetNextImage(CurrentChapter, PictureNumber));

                    PictureNumber = 0;
                    TMCycleImage.Start();
                }

                else
                {
                    if (PBImages?.Image != null)
                    {
                        PBImages.Image.Dispose();
                    }

                    PBImages.Image = Image.FromFile(Images.GetNextImage(CurrentChapter, PictureNumber));

                    TMCycleImage.Start();
                }
            }
        }

        private void TTS(object sender, EventArgs e)
        {
            switch (TTSFlag)
            {
                case true: TTSFlag = false; PBTTS.Image.Dispose(); PBTTS.Image = Properties.Resources.Mute; BTBack.Enabled = true; BTNext.Enabled = true; break;
                case false: TTSFlag = true; PBTTS.Image.Dispose(); PBTTS.Image = Properties.Resources.Unmute; BTBack.Enabled = false; BTNext.Enabled = false; break;
            }

            if (TTSFlag)
            {
                Speech.SpeakCompleted += SpeakNextPage;
                Speech.SpeakAsync(TTSChapter);
            }

            else
            {
                Speech.SpeakCompleted -= SpeakNextPage;
                Speech.SpeakAsyncCancelAll();
            }
        }

        private void SpeakNextPage(object sender, SpeakCompletedEventArgs e)
        {
            string Page = StoreItalicWords();

            if (Pages.CurrentPage(Page) == "run RemovePreviousPage")
            {
                Pages.RemovePreviousPage(LastLine, LastLineAddition);

                RTBChapterText.Clear();
                TruncateText(Text_Model.ChapterText);
                GetItalicWords();
                FormatText();

                PageNumber++;

                Speech.SpeakAsync(TTSChapter);
            }

            else
            {
                if (Text_Model.Pages.Count == (PageNumber + 1))
                {
                    RTBChapterText.Clear();
                    TruncateText(Text_Model.ChapterText);
                    GetItalicWords();
                    FormatText();

                    PageNumber++;

                    Speech.SpeakAsync(TTSChapter);
                }

                else
                {
                    PageNumber++;
                    RTBChapterText.Clear();
                    RTBChapterText.Text = Pages.LoadPage(Text_Model.Pages[PageNumber]);

                    GetItalicWords();
                    FormatText();

                    Speech.SpeakAsync(TTSChapter);
                }
            }
        }
    }
}
