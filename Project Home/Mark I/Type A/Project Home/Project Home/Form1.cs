using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml;

namespace Project_Home
{
    public partial class Form1 : Form
    {
        // Creates the variables and gives them the properties of the recognition engine and the synthesizer.
        SpeechRecognitionEngine SRE = null;
        SpeechSynthesizer Home = new SpeechSynthesizer();

        // Creates a variable for the Random properties.
        Random R = new Random();
        public Form1()
        {
            InitializeComponent();

            // Sets the text in the debug textbox to a size of 12.
            TBHome.Font = new Font("", 12);

            // Sets the language for the recognition engine to UK english.
            SRE = SetLanguage("en-GB");

            // Event handler for the recognised text.
            SRE.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(MainEvent_SpeechRecognized);

            // Event handler for the loading of the grammar.
            LoadGrammarAndCommands();

            // Sets the microphone for the input.
            SRE.SetInputToDefaultAudioDevice();

            // Sets the recognition engine to listen.
            SRE.RecognizeAsync(RecognizeMode.Multiple);
        }

        // Loads the grammar from the database.
        private void LoadGrammarAndCommands()
        {
            // Attempts to run the following code.
            try
            {
                // Generates a connection to the database.
                string ConString = ConfigurationManager.ConnectionStrings["MyGrammar"].ConnectionString;
                SqlConnection Con = new SqlConnection(ConString);

                // Opens the connection.
                Con.Open();

                // Generates a connection between the SqlConnection and the SqlCommand.
                SqlCommand SC = new SqlCommand();
                SC.Connection = Con;

                // Selects all the commands from the database table.
                SC.CommandText = "SELECT * FROM DefaultTable";
                // SC.CommandType = CommandType.TableDirect;

                // Creates the variable and gives it all the properties of the data reader.
                SqlDataReader SDR = SC.ExecuteReader();
                
                // Loops through the database until it has been read.
                while (SDR.Read())
                {
                    // Loads the default commands column into the variable LoadCMD.
                    var LoadCMD = SDR["DefaultCommands"].ToString();

                    // Creates the new grammar for the recognition engine from all the data int default commands column.
                    Grammar CommandGrammar = new Grammar(new GrammarBuilder(new Choices(LoadCMD)));

                    // Loads the grammar into the recognition engine.
                    SRE.LoadGrammarAsync(CommandGrammar);
                }

                // Closes the reader and the connection.
                SDR.Close();
                Con.Close();
            }

            // Prevents the program closing if an isssue is found by displaying the error message to the user. 
            catch (Exception ex)
            {
                Home.SpeakAsync("I have detected an invalid entry in your web commands, potentially a blank line. Web commands " +
                    "will cease to work until the issues has been resolved. " + ex.Message);
            }
        }

        // Converts the voice to text and executes the commands.
        private void MainEvent_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Recognises spoken words and and puts them in a variable.
            string Speech = e.Result.Text;

            // Variable to contain the responce of the system.
            string Response = null;

            // Prints what is said to the Dedug Box with a timestamp.
            TBHome.AppendText(DateTime.Now + ": " + Speech);
            TBHome.AppendText(Environment.NewLine);

            // Switches the speech variable to e.Result.Text.
            switch (Speech)
            {
                // Executable commands with their keywords (Subject to major overhaul).
                case "hello":
                    int RN = R.Next(1, 3);

                    if (RN == 1)
                    {
                        Home.SpeakAsync("Hello sir");
                        Response = "Hello sir";
                    }

                    if (RN == 2)
                    {
                        // This checks if it is morning or afternoon so that it uses the correct phrase (good morning/afternoon).
                        TimeSpan M = new TimeSpan(12, 0, 0);
                        TimeSpan A = new TimeSpan(11, 59, 59);
                        TimeSpan E = new TimeSpan(17, 0, 0);
                        TimeSpan N = DateTime.Now.TimeOfDay;

                        if (N < M)
                        {
                            Home.SpeakAsync("Good morning sir");
                            Response = "Good morning sir";
                        }
                        if (N > A && N < E)
                        {
                            Home.SpeakAsync("Good afternoon sir");
                            Response = "Good afternoon sir";
                        }
                        else
                        {
                            Home.SpeakAsync("Good evening sir");
                            Response = "Good evening sir";
                        }
                    }

                    break;

                // Tells the user time of day.
                case "what is the time":
                    DateTime DT = DateTime.Now;
                    string Time = DT.GetDateTimeFormats('t')[0];
                    Home.SpeakAsync(Time);
                    Response = Time;
                    break;

                // Tells the user what the day it.
                case "what is the day":
                    string Day = "Today is, " + DateTime.Now.ToString("dddd");
                    Home.SpeakAsync(Day);
                    Response = Day;
                    break;

                // Tells the user what the date is.
                case "what is the date":
                case "what is todays date":
                    string Date = "The date is, " + DateTime.Now.ToString("dd MMM yyyy");
                    Home.SpeakAsync(Date);
                    Response = Date;
                    break;

                // Tells the user they are gay as per Luke's request
                case "I like cum":
                    Home.SpeakAsync("Wow, that's very gay.");
                    Response = "Wow, that's very gay.";
                    break;
            }

            // Prints what is said by the system to the Dedug Box with a timestamp.
            TBHome.AppendText(DateTime.Now + ": " + Response);
            TBHome.AppendText(Environment.NewLine);
        }

        // Checks if the language that the recognition engine is set to is equal to the language we gave the parameter.
        private SpeechRecognitionEngine SetLanguage(string PreferredLanguage)
        {
            foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (config.Culture.ToString() == PreferredLanguage)
                {
                    SRE = new SpeechRecognitionEngine(config);
                    break;
                }
            }

            // If the desired language is not found, then it will load the default.
            if (SRE == null)
            {
                MessageBox.Show("The desired language is not installed on this device, the speech engine will continue to use "
                    + SpeechRecognitionEngine.InstalledRecognizers()[0].Culture.ToString() + " as the default language.",
                    "Language: " + PreferredLanguage + " could not be found!");

                SRE = new SpeechRecognitionEngine(SpeechRecognitionEngine.InstalledRecognizers()[0]);
            }

            // This returns the language that the recognition engine has been set too.
            return SRE;
        }

        // Gets an email from the email address web server and puts the emails data into variables.
        private void GetAllEmails()
        {
            //try
            //{
            //    // Creates the variables and gives the variable WC the properties of the WebClient api.
            //    System.Net.WebClient WC = new System.Net.WebClient();
            //    string Responce;
            //    string Ttitle;
            //    string Summary;
            //    string TagLine;
            //
            //    // Creates an xml document
            //    XmlDocument XD = new XmlDocument();
            //
            //    // Logging into the email server.
            //    //WC.Credentials = new System.Net.NetworkCredential(Usernametxt.Text, Passwordtxt.Text);
            //
            //    // Reading the data and converting it to a string.
            //    Responce = Encoding.UTF8.GetString(WC.DownloadData(@"https://mail.google.com/mail/feed/atom"));
            //    Responce = Responce.Replace(@"<feed version = "" 0.3 "" xmlns = "" http://purl.org/atom/ns# "" > ", @"<feed>");
            //}
        }

        private void TBHome_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
