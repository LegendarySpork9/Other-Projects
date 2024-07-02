// Namespaces used in the Class.
using System.Net;
using Dropbox.Api;
using System.IO.Compression;
using System.Diagnostics;
using Dropbox.Api.Files;
using System.Xml;

namespace PA_Updater
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Runs code even if error may occur.
            try
            {
                // Writes the given string to the console.
                Console.Write("Hunter Corp [Version 1.0.0]\n(c) Toby Hunter. All rights reserved.\n\n");

                // Makes the program wait for a second.
                Thread.Sleep(1000);

                // Sets the value of ConsoleOutput to the given string.
                string ConsoleOutput = "Beginning Update\n";

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Testing Dropbox Connection  ";

                // Assigns WC all the properties within the WebClient class and opens a stream to the URL.
                WebClient WC = new WebClient();
                WC.OpenRead("https://www.dropbox.com/");

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != 32; x++)
                {
                    // Sets the position of the cursor back 1 and creates a progress swivel. 
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    switch (x % 4)
                    {
                        case 0: Console.Write("/"); break;
                        case 1: Console.Write("-"); break;
                        case 2: Console.Write("\\"); break;
                        case 3: Console.Write("|"); break;
                    }

                    // Makes the program wait for the given time.
                    Thread.Sleep(100);
                }

                // Sets the position of the cursor back 1 and makes the program wait for the given time.
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write("*\n");

                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Locating Update Files  ";

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != 32; x++)
                {
                    // Sets the position of the cursor back 1 and creates a progress swivel. 
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    switch (x % 4)
                    {
                        case 0: Console.Write("/"); break;
                        case 1: Console.Write("-"); break;
                        case 2: Console.Write("\\"); break;
                        case 3: Console.Write("|"); break;
                    }

                    // Makes the program wait for the given time.
                    Thread.Sleep(100);
                }

                // Sets the position of the cursor back 1 and makes the program wait for the given time.
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write("*\n");

                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Starting Download of Update Files";

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Finalising Download of Update Files  ";

                // Assigns Drobox all the properties of the DropboxClient class, stores the dropbox folder path, stores the name of the update zip file,
                // Stores its local file path.
                DropboxClient Dropbox = new DropboxClient("");
                string Folder = "Project Assistance/Latest Release";
                string FileName = "Assistant.zip";
                string FilePath = ".\\Updater\\temp\\Assistant.zip";

                // Downloads the file.
                using (var response = await Dropbox.Files.DownloadAsync("/" + Folder + "/" + FileName))
                {
                    // Moves the file to the specified file path.
                    using (var fileStream = File.Create(FilePath))
                    {
                        // Waits for the download to finish.
                        (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
                    }
                }

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != 32; x++)
                {
                    // Sets the position of the cursor back 1 and creates a progress swivel. 
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    switch (x % 4)
                    {
                        case 0: Console.Write("/"); break;
                        case 1: Console.Write("-"); break;
                        case 2: Console.Write("\\"); break;
                        case 3: Console.Write("|"); break;
                    }

                    // Makes the program wait for the given time.
                    Thread.Sleep(100);
                }

                // Sets the position of the cursor back 1 and makes the program wait for the given time.
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write("*\n");

                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Extracting Update Files  ";

                // Extracts the ZIP file to the given path, deletes the zip file and moves the extracted files to the program's directory.
                ZipFile.ExtractToDirectory(FilePath, FilePath.Replace("Assistant.zip", ""));
                File.Delete(FilePath);
                FilePath = FilePath.Replace("Assistant.zip", "");

                // Searches all files in the file path.
                foreach (var file in Directory.GetFiles(FilePath))
                {
                    // Runs code if condition is met.
                    if (file.Contains("Release Note") == true)
                    {
                        // Moves the file from the temp folder to the Release Notes directory.
                        File.Copy(file, Path.Combine(FilePath.Replace("\\Updater\\temp", "\\Release Notes"), Path.GetFileName(file)), true);
                    }

                    // Runs code if condition is met.
                    else
                    {
                        // Moves the file from the temp folder to the main directory.
                        File.Copy(file, Path.Combine(FilePath.Replace("\\Updater\\temp", ""), Path.GetFileName(file)), true);
                    }
                }

                // Searches all directories in the file path.
                foreach (var directory in Directory.GetDirectories(FilePath))
                {
                    // Moves the directory from the temp folder to the main directory.
                    File.Copy(directory, Path.Combine(FilePath.Replace("\\Updater\\temp", ""), Path.GetFileName(directory)), true);
                }

                // Deletes all files and folders in the Updater's temp folder.
                Process.Start("cmd.exe", string.Format("/C RD /S /Q \"{0}", FilePath));

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != 32; x++)
                {
                    // Sets the position of the cursor back 1 and creates a progress swivel. 
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    switch (x % 4)
                    {
                        case 0: Console.Write("/"); break;
                        case 1: Console.Write("-"); break;
                        case 2: Console.Write("\\"); break;
                        case 3: Console.Write("|"); break;
                    }

                    // Makes the program wait for the given time.
                    Thread.Sleep(100);
                }

                // Creates the temp folder.
                Directory.CreateDirectory(FilePath);

                // Sets the position of the cursor back 1 and makes the program wait for the given time.
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write("*\n");

                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Updating Assistant Version  ";

                // Stores the dropbox folder path, stores the name of the update zip file and Stores its local file path.
                Folder = "Project Assistance/Global Configuration";
                FileName = "Global Configuration File.config";
                FilePath = ".\\Configuration\\Global Configuration File.config";

                // Downloads the file.
                using (var response = await Dropbox.Files.DownloadAsync("/" + Folder + "/" + FileName))
                {
                    // Moves the file to the specified file path.
                    using (var fileStream = File.Create(FilePath))
                    {
                        // Waits for the download to finish.
                        (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
                    }
                }

                // Assigns XML all the properties of the XmlDocument class, loads the xml of the given file, assigns the AssistantName vairable the declared value in the config XML, loads the xml of the given file, changes the version number of the assistant to match
                // the latest version and saves the change to the Global Configuration File.
                XmlDocument XML = new XmlDocument();
                XML.Load(@".\Configuration\Assistant.config");
                string AssistantName = XML.SelectSingleNode("Configuration/Assistant/Information").Attributes[0].Value;
                XML.Load(@".\Global Configuration File.config");
                XML.SelectSingleNode("BilboBaggins/" + AssistantName + "/Version").InnerText = XML.SelectSingleNode("BilboBaggins/LatestRelease").Attributes[0].Value;
                XML.Save(@".\Global Configuration File.config");

                // Reads the file into a stream.
                using (var memoryStream = new MemoryStream(File.ReadAllBytes(FilePath)))
                {
                    // Uploads the file to dropbox.
                    var Result = await Dropbox.Files.UploadAsync("/" + Folder + "/" + FileName, WriteMode.Overwrite.Instance, body: memoryStream);
                }

                // Deletes the GCF file.
                File.Delete(FilePath);

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != 32; x++)
                {
                    // Sets the position of the cursor back 1 and creates a progress swivel. 
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    switch (x % 4)
                    {
                        case 0: Console.Write("/"); break;
                        case 1: Console.Write("-"); break;
                        case 2: Console.Write("\\"); break;
                        case 3: Console.Write("|"); break;
                    }

                    // Makes the program wait for the given time.
                    Thread.Sleep(100);
                }

                // Sets the position of the cursor back 1 and makes the program wait for the given time.
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write("*\n");

                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Update Complete";

                // Runs code until the count reaches the length of the string.
                for (int x = 0; x != ConsoleOutput.Length; x++)
                {
                    // Writes the given string to the console, makes the program wait for the given time.
                    Console.Write(ConsoleOutput[x]);
                    Thread.Sleep(100);
                }

                // Stores the file path of the main program.
                FilePath = @".\Project Assistance.exe";

                // Makes the program wait for a second.
                Thread.Sleep(1000);

                // Runs the main program.
                Process.Start(FilePath);

                // Closes the Updater.
                Environment.Exit(0);
            }

            // Runs code if an error occurs.
            catch (Exception ex)
            {
                // Creates a file path and name for the error log, creates the error message, creates the file and writes the error message to the log file.
                string ErrorLog = @".\Logs\Error Logs\LocalConfigCheck_Error." + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";
                string ErrorMsg = "The following error occured while running the Updater:" + Environment.NewLine +
                    ex.ToString();
                File.Create(ErrorLog).Dispose();
                File.WriteAllText(ErrorLog, ErrorMsg);

                Thread.Sleep(5000);
            }
        }
    }
}