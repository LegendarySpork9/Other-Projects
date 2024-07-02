// Namespaces used in the Class.
using Dropbox.Api;
using System.IO.Compression;
using Dropbox.Api.Files;
using System.Diagnostics;

namespace Interplanetario
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Runs code even if error may occur.
            try
            {
                // Sets the value of ConsoleOutput to the given string.
                string ConsoleOutput = "Performing Update Check  ";

                // Assigns Drobox all the properties of the DropboxClient class, stores the dropbox folder path, stores the name of the update zip file,
                // Stores its local file path.
                DropboxClient Dropbox = new DropboxClient("");
                string Folder = "Games/Interplanetario";
                string FileName = "DateLMServer.txt";
                string FilePath = ".\\Configs\\DateLMServer.txt";

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
                ConsoleOutput = "Checking DateTimes  ";

                // Sets DateLM and DateLMServer to text file values.
                string DateLM = File.ReadAllText(".\\Configs\\DateLM.txt");
                string DateLMServer = File.ReadAllText(".\\Configs\\DateLMServer.txt");

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

                // Runs code if condition met.
                if (DateLM != DateLMServer)
                {
                    // Sets the value of ConsoleOutput to the given string.
                    ConsoleOutput = "Downloading Update  ";

                    // Stores the dropbox folder path, stores the name of the update zip file, Stores its local file path.
                    Folder = "Games/Interplanetario";
                    FileName = "Title In Development.zip";
                    FilePath = ".\\Data\\Title In Development.zip";

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
                    ConsoleOutput = "Extracting Update  ";

                    // Sets SESaves to the value in the text file.
                    string SESaves = File.ReadAllText(".\\Configs\\SESaves.txt");

                    // Deletes all files and folders in the save folder.
                    Process.Start("cmd.exe", string.Format("/C RD /S /Q \"{0}", SESaves));

                    // Pauses the process.
                    Thread.Sleep(5000);

                    // Creates the save folder.
                    Directory.CreateDirectory(SESaves);

                    // Extracts the ZIP file to the given path, deletes the zip file and moves the extracted files to the program's directory.
                    ZipFile.ExtractToDirectory(FilePath, SESaves);
                    File.Delete(FilePath);
                    FilePath = FilePath.Replace("Title In Development.zip", "");

                    // Updates the DateLM to match the DateLMServer and writes string to text file.
                    DateLM = DateLMServer;
                    File.WriteAllText(".\\Configs\\DateLM.txt", DateLM);

                    // Deletes the DateLMServer file.
                    File.Delete(".\\Configs\\DateLMServer.txt");

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
                    ConsoleOutput = "Performing Update  ";

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
                    Console.Write("*\n\n");

                    // Creates a new 10 minute timer.
                    Timer timer = new Timer(TimePassed, null, 0, 300000);
                    Console.ReadLine();
                }

                // Runs code if condition met.
                else
                {
                    //Adds line breaks to the console.
                    Console.Write("\n");

                    // Deletes the DateLMServer file.
                    File.Delete(".\\Configs\\DateLMServer.txt");

                    // Creates a new 10 minute timer.
                    Timer timer = new Timer(TimePassed, null, 0, 300000);
                    Console.ReadLine();
                }
            }

            // Runs code if an error occurs.
            catch (Exception ex)
            {
                // Creates a file path and name for the error log, creates the error message, creates the file and writes the error message to the log file.
                string ErrorLog = @".\Logs\Update_Error." + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";
                string ErrorMsg = "The following error occured while performing the world update:" + Environment.NewLine + ex.ToString();
                File.Create(ErrorLog).Dispose();
                File.WriteAllText(ErrorLog, ErrorMsg);
            }
        }

        private static void TimePassed(Object o)
        {
            Console.WriteLine("In TimePassed: " + DateTime.Now);
            ServerCheck();
        }

        private static async Task ServerCheck()
        {
            // Sets the value of ConsoleOutput to the given string.
            string ConsoleOutput = "\nObtaining Task Manager Processes  ";

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

            // Obtains all apps from TaskManager and declares SE.
            Process[] processes = Process.GetProcesses();
            string SE = null;

            // Searches all processes
            foreach (Process p in processes)
            {
                // Runs code if condition met.
                if (p.MainWindowHandle.ToInt32() > 0)
                {
                    // Runs code if condition met.
                    if (p.ToString() == "System.Diagnostics.Process (SpaceEngineers)")
                    {
                        // Updates SE to true and writes string to text file.
                        SE = "true";
                        File.WriteAllText(".\\Configs\\SETM.txt", SE);
                    }
                }
            }

            // Reads text from file.
            SE = File.ReadAllText(".\\Configs\\SETM.txt");

            // Sets the value of ConsoleOutput to the given string.
            ConsoleOutput = "Checking for Space Engineers  ";

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

            // Runs code if condition met.
            if (SE == "false")
            {
                // Runs code even if error may occur.
                try
                {
                    // Sets the value of ConsoleOutput to the given string.
                    ConsoleOutput = "Performing Update Check  ";

                    // Assigns Drobox all the properties of the DropboxClient class, stores the dropbox folder path, stores the name of the update zip file,
                    // Stores its local file path.
                    DropboxClient Dropbox = new DropboxClient("");
                    string Folder = "Games/Interplanetario";
                    string FileName = "DateLMServer.txt";
                    string FilePath = ".\\Configs\\DateLMServer.txt";

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
                    ConsoleOutput = "Checking DateTimes  ";

                    // Sets DateLM and DateLMServer to text file values.
                    string DateLM = File.ReadAllText(".\\Configs\\DateLM.txt");
                    string DateLMServer = File.ReadAllText(".\\Configs\\DateLMServer.txt");

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

                    // Runs code if condition met.
                    if (DateLM != DateLMServer)
                    {
                        // Sets the value of ConsoleOutput to the given string.
                        ConsoleOutput = "Downloading Update  ";

                        // Stores the dropbox folder path, stores the name of the update zip file, Stores its local file path.
                        Folder = "Games/Interplanetario";
                        FileName = "Title In Development.zip";
                        FilePath = ".\\Data\\Title In Development.zip";

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
                        ConsoleOutput = "Extracting Update  ";

                        // Sets SESaves to the value in the text file.
                        string SESaves = File.ReadAllText(".\\Configs\\SESaves.txt");

                        // Deletes all files and folders in the save folder.
                        Process.Start("cmd.exe", string.Format("/C RD /S /Q \"{0}", SESaves));

                        // Pauses the process.
                        Thread.Sleep(5000);

                        // Creates the save folder.
                        Directory.CreateDirectory(SESaves);

                        // Extracts the ZIP file to the given path, deletes the zip file and moves the extracted files to the program's directory.
                        ZipFile.ExtractToDirectory(FilePath, SESaves);
                        File.Delete(FilePath);
                        FilePath = FilePath.Replace("Title In Development.zip", "");

                        // Updates the DateLM to match the DateLMServer and writes string to text file.
                        DateLM = DateLMServer;
                        File.WriteAllText(".\\Configs\\DateLM.txt", DateLM);

                        // Deletes the DateLMServer file.
                        File.Delete(".\\Configs\\DateLMServer.txt");

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
                        ConsoleOutput = "Performing Update  ";

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
                        Console.Write("*\n\n");
                    }

                    // Runs code if condition met.
                    else
                    {
                        //Adds line breaks to the console.
                        Console.Write("\n");

                        // Deletes the DateLMServer file.
                        File.Delete(".\\Configs\\DateLMServer.txt");
                    }
                }

                // Runs code if an error occurs.
                catch (Exception ex)
                {
                    // Creates a file path and name for the error log, creates the error message, creates the file and writes the error message to the log file.
                    string ErrorLog = @".\Logs\Update_Error." + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";
                    string ErrorMsg = "The following error occured while performing the world update:" + Environment.NewLine + ex.ToString();
                    File.Create(ErrorLog).Dispose();
                    File.WriteAllText(ErrorLog, ErrorMsg);
                }
            }

            // Runs code if condition met.
            else
            {
                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Checking for Space Engineers - Part II  ";

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

                // Obtains all apps from TaskManager, creates a new list and declares SE.
                processes = Process.GetProcesses();
                List<string> listProc = new List<string>();
                SE = null;

                // Searches all processes.
                foreach (Process p in processes)
                {
                    // Runs code if condition met.
                    if (p.MainWindowHandle.ToInt32() > 0)
                    {
                        // Adds the value to the list.
                        listProc.Add(p.MainWindowTitle);
                    }
                }

                if (listProc.Contains("Space Engineers") == false)
                {
                    // Sets SESaves to the value in the text file, sets DateLM to text file value and obatins last modified date of selected file.
                    string SESaves = File.ReadAllText(".\\Configs\\SESaves.txt");
                    string DateLM = File.ReadAllText(".\\Configs\\DateLM.txt");
                    string SELM = File.GetLastWriteTime(Path.Combine(SESaves, "Sandbox.sbc")).ToString();

                    // Runs code if condition met.
                    if (DateLM != SELM)
                    {
                        // Runs code even if error may occur.
                        try
                        {
                            // Sets the value of ConsoleOutput to the given string.
                            ConsoleOutput = "Zipping Save Files  ";

                            // Zips save files into data folder.
                            ZipFile.CreateFromDirectory(SESaves, ".\\Data\\Title In Development.zip");

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
                            ConsoleOutput = "Uploading to Dropbox  ";

                            // Assigns Drobox all the properties of the DropboxClient class, stores the dropbox folder path, stores the name of the update zip file.
                            DropboxClient Dropbox = new DropboxClient("");
                            string Folder = "Games/Interplanetario";
                            string FileName = "Title In Development.zip";

                            // Reads the file into a stream.
                            using (var memoryStream = new MemoryStream(File.ReadAllBytes(".\\Data\\Title In Development.zip")))
                            {
                                // Uploads the file to dropbox.
                                var Result = await Dropbox.Files.UploadAsync("/" + Folder + "/" + FileName, WriteMode.Overwrite.Instance, body: memoryStream);
                            }

                            // Deletes the GCF file.
                            File.Delete(".\\Data\\Title In Development.zip");

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
                            ConsoleOutput = "Updating DateTimes  ";

                            // Updates the DateLM to match the SELM and writes string to text file.
                            DateLM = SELM;
                            File.WriteAllText(".\\Configs\\DateLM.txt", DateLM);

                            // Updates the DateLMServer to match the SELM and writes string to text file.
                            string DateLMServer = SELM;
                            File.WriteAllText(".\\Configs\\DateLMServer.txt", DateLMServer);

                            // Stores the dropbox folder path, stores the name of the update text file.
                            Folder = "Games/Interplanetario";
                            FileName = "DateLMServer.txt";

                            // Reads the file into a stream.
                            using (var memoryStream = new MemoryStream(File.ReadAllBytes(".\\Configs\\DateLMServer.txt")))
                            {
                                // Uploads the file to dropbox.
                                var Result = await Dropbox.Files.UploadAsync("/" + Folder + "/" + FileName, WriteMode.Overwrite.Instance, body: memoryStream);
                            }

                            // Deletes the GCF file.
                            File.Delete(".\\Configs\\DateLMServer.txt");

                            // Updates SE to true and writes string to text file.
                            SE = "false";
                            File.WriteAllText(".\\Configs\\SETM.txt", SE);

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
                            Console.Write("*\n\n");
                        }

                        // Runs code if an error occurs.
                        catch (Exception ex)
                        {
                            // Creates a file path and name for the error log, creates the error message, creates the file and writes the error message to the log file.
                            string ErrorLog = @".\Logs\Update_Error." + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";
                            string ErrorMsg = "The following error occured while performing the world update:" + Environment.NewLine + ex.ToString();
                            File.Create(ErrorLog).Dispose();
                            File.WriteAllText(ErrorLog, ErrorMsg);
                        }
                    }

                    //Runs code if condition met.
                    else
                    {
                        // Adds line breaks to the console.
                        Console.Write("\n");

                        // Updates SE to true and writes string to text file.
                        SE = "false";
                        File.WriteAllText(".\\Configs\\SETM.txt", SE);
                    }
                }

                //Runs code if condition met.
                else
                {
                    // Adds line breaks to the console.
                    Console.Write("\n");
                }
            }
        }
    }
}