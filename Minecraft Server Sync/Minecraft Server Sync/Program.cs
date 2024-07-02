// Namespaces used in the Class.
using Dropbox.Api;
using System.IO.Compression;
using Dropbox.Api.Files;
using System.Diagnostics;
using System.Timers;

namespace Minecraft_Server_Sync
{
    internal class Program
    {
        // Assigns timer all the properties of the Timer class.
        static System.Timers.Timer timer = new System.Timers.Timer();

        static async Task Main(string[] args)
        {
            // Runs code even if error may occur.
            try
            {
                // Recovers the sync if it errored and forced a reload.
                File.Delete(".\\Data\\Title In Development.zip");
                File.Delete(".\\Configs\\DateLMServer.txt");

                // Sets the value of ConsoleOutput to the given string.
                string ConsoleOutput = "Performing Update Check  ";

                // Assigns Drobox all the properties of the DropboxClient class, stores the dropbox folder path, stores the name of the update zip file,
                // Stores its local file path.
                DropboxClient Dropbox = new DropboxClient("");
                string Folder = "Games/1.7.10";
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
                    // Writes a message to the console.
                    Console.WriteLine("Starting Download of Update Files");

                    // Sets the value of ConsoleOutput to the given string.
                    ConsoleOutput = "Finalising Download  ";

                    // Stores the dropbox folder path, stores the name of the update zip file, Stores its local file path.
                    Folder = "Games/1.7.10";
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

                    // Sets SL to the value in the text file.
                    string SL = File.ReadAllText(".\\Configs\\ServerLocation.txt");

                    // Deletes all files and folders in the save folder.
                    Process.Start("cmd.exe", string.Format("/C RD /S /Q \"{0}", SL));

                    // Pauses the process.
                    Thread.Sleep(15000);

                    // Creates the save folder.
                    Directory.CreateDirectory(SL);

                    // Extracts the ZIP file to the given path, deletes the zip file and moves the extracted files to the program's directory.
                    ZipFile.ExtractToDirectory(FilePath, SL);
                    File.Delete(FilePath);
                    FilePath = FilePath.Replace("Title In Development.zip", "");

                    // Sets ServerProp to text file values and moves the server properties file to the server folder.
                    string ServerProp = File.ReadAllText(".\\Configs\\ServerProp.txt");
                    File.Copy(ServerProp, SL + "server.properties", true);

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

                    // Creates and starts a new 10 minute timer.
                    timer.Interval = 300000;
                    timer.Elapsed += TimePassed;
                    timer.AutoReset = true;
                    timer.Enabled = true;
                    timer.Start();
                    Console.ReadLine();
                }

                // Runs code if condition met.
                else
                {
                    //Adds line breaks to the console.
                    Console.Write("\n");

                    // Deletes the DateLMServer file.
                    File.Delete(".\\Configs\\DateLMServer.txt");

                    // Creates and starts a new 10 minute timer.
                    timer.Interval = 300000;
                    timer.Elapsed += TimePassed;
                    timer.AutoReset = true;
                    timer.Enabled = true;
                    timer.Start();
                    Console.ReadLine();
                }
            }

            // Runs code if an error occurs.
            catch (Exception ex)
            {
                // Creates a file path and name for the error log, creates the error message, creates the file and writes the error message to the log file.
                string ErrorLog = @".\Logs\Update_Error." + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";
                string ErrorMsg = "The following error occured while performing the server update:" + Environment.NewLine + ex.ToString();
                File.Create(ErrorLog).Dispose();
                File.WriteAllText(ErrorLog, ErrorMsg);

                // Restarts the Program.
                Console.Clear();
                Process.Start(".\\Minecraft Server Sync.exe");
                Environment.Exit(0);
            }
        }

        private static void TimePassed(Object source, ElapsedEventArgs e)
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
            string M = null;

            // Searches all processes
            foreach (Process p in processes)
            {
                // Runs code if condition met.
                if (p.MainWindowHandle.ToInt32() > 0)
                {
                    // Runs code if condition met.
                    if (p.MainWindowTitle.ToString() == "Minecraft Server Console")
                    {
                        // Updates SE to true and writes string to text file.
                        M = "true";
                        File.WriteAllText(".\\Configs\\STM.txt", M);
                    }
                }
            }

            // Reads text from file.
            M = File.ReadAllText(".\\Configs\\STM.txt");

            // Sets the value of ConsoleOutput to the given string.
            ConsoleOutput = "Checking for Minecraft Server  ";

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
            if (M == "false")
            {
                // Runs code even if error may occur.
                try
                {
                    // Sets the value of ConsoleOutput to the given string.
                    ConsoleOutput = "Performing Update Check  ";

                    // Assigns Drobox all the properties of the DropboxClient class, stores the dropbox folder path, stores the name of the update zip file,
                    // Stores its local file path.
                    DropboxClient Dropbox = new DropboxClient("");
                    string Folder = "Games/1.7.10";
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
                        // Stops the timer.
                        timer.Stop();

                        // Writes a message to the console.
                        Console.WriteLine("Starting Download of Update Files");

                        // Sets the value of ConsoleOutput to the given string.
                        ConsoleOutput = "Finalising Download  ";

                        // Stores the dropbox folder path, stores the name of the update zip file, Stores its local file path.
                        Folder = "Games/1.7.10";
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

                        // Sets SL to the value in the text file.
                        string SL = File.ReadAllText(".\\Configs\\ServerLocation.txt");

                        // Deletes all files and folders in the save folder.
                        Process.Start("cmd.exe", string.Format("/C RD /S /Q \"{0}", SL));

                        // Pauses the process.
                        Thread.Sleep(15000);

                        // Creates the save folder.
                        Directory.CreateDirectory(SL);

                        // Extracts the ZIP file to the given path, deletes the zip file and moves the extracted files to the program's directory.
                        ZipFile.ExtractToDirectory(FilePath, SL);
                        File.Delete(FilePath);
                        FilePath = FilePath.Replace("Title In Development.zip", "");

                        // Sets ServerProp to text file values and moves the server properties file to the server folder.
                        string ServerProp = File.ReadAllText(".\\Configs\\ServerProp.txt");
                        File.Copy(ServerProp, SL + "server.properties", true);

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

                        // Starts the timer.
                        timer.Start();
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
                    string ErrorMsg = "The following error occured while performing the server update:" + Environment.NewLine + ex.ToString();
                    File.Create(ErrorLog).Dispose();
                    File.WriteAllText(ErrorLog, ErrorMsg);

                    // Restarts the Program.
                    Console.Clear();
                    Process.Start(".\\Minecraft Server Sync.exe");
                    Environment.Exit(0);
                }
            }

            // Runs code if condition met.
            else
            {
                // Sets the value of ConsoleOutput to the given string.
                ConsoleOutput = "Checking for Minecraft Server - Part II  ";

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
                M = null;

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

                if (listProc.Contains("Minecraft Server Console") == false)
                {
                    // Sets SL to the value in the text file, sets DateLM to text file value and obatins last modified date of selected file.
                    string SL = File.ReadAllText(".\\Configs\\ServerLocation.txt");
                    string DateLM = File.ReadAllText(".\\Configs\\DateLM.txt");
                    string SLM = File.GetLastWriteTime(Path.Combine(SL, "world\\level.dat")).ToString();

                    // Runs code if condition met.
                    if (DateLM != SLM)
                    {
                        // Runs code even if error may occur.
                        try
                        {
                            // Stops the timer.
                            timer.Stop();

                            // Sets the value of ConsoleOutput to the given string.
                            ConsoleOutput = "Zipping Server Files  ";

                            // Zips save files into data folder.
                            ZipFile.CreateFromDirectory(SL, ".\\Data\\Title In Development.zip");

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
                            ConsoleOutput = "Finalising Upload to Dropbox  ";

                            // Assigns Drobox all the properties of the DropboxClient class, stores the dropbox folder path, stores the name of the update zip file.
                            DropboxClient Dropbox = new DropboxClient("");
                            string Folder = "Games/1.7.10";
                            string FileName = "Title In Development.zip";

                            // Chunk size is 10MB.
                            const int chunkSize = 10 * 1024 * 1024;

                            // Loads the ZIP file.
                            var fileContent = File.ReadAllBytes(".\\Data\\Title In Development.zip");

                            // Reads the file into a stream.
                            using (var stream = new MemoryStream(fileContent))
                            {
                                // Calculates the number of required chunks, estimated upload time and writes the result to the console.
                                int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);
                                int UploadTime = 6 * numChunks;
                                Console.WriteLine("File has been split into " + numChunks + " chunks, this will take approximately " + UploadTime + " seconds.");

                                // Creates the chunks.
                                byte[] buffer = new byte[chunkSize];
                                string sessionId = null;

                                // Loops through all the chunks.
                                for (var idx = 0; idx < numChunks; idx++)
                                {
                                    // Writes the chunk number to the console and reads the current chunk.
                                    Console.WriteLine("Uploading chunk {0}", idx);
                                    var byteRead = stream.Read(buffer, 0, chunkSize);

                                    // Reads the chunk into a stream.
                                    using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                                    {
                                        // Runs code if condition met.
                                        if (idx == 0)
                                        {
                                            // Starts the upload of chunks.
                                            var result = await Dropbox.Files.UploadSessionStartAsync(body: memStream);
                                            sessionId = result.SessionId;
                                        }

                                        // Runs code if condition met.
                                        else
                                        {
                                            // Creates an upload session for the current chunk.
                                            UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                                            // Runs code if condition met.
                                            if (idx == numChunks - 1)
                                            {
                                                // Uploads the chunk to dropbox.
                                                await Dropbox.Files.UploadSessionFinishAsync(cursor, new CommitInfo("/" + Folder + "/" + FileName, mode: WriteMode.Overwrite.Instance), body: memStream);
                                            }

                                            // Runs code if condition met.
                                            else
                                            {
                                                // Uploads the chunk to dropbox.
                                                await Dropbox.Files.UploadSessionAppendV2Async(cursor, body: memStream);
                                            }
                                        }
                                    }
                                }
                            }

                            // Deletes the zip file.
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

                            // Updates the DateLM to match the SLM and writes string to text file.
                            DateLM = SLM;
                            File.WriteAllText(".\\Configs\\DateLM.txt", DateLM);

                            // Updates the DateLMServer to match the SLM and writes string to text file.
                            string DateLMServer = SLM;
                            File.WriteAllText(".\\Configs\\DateLMServer.txt", DateLMServer);

                            // Stores the dropbox folder path, stores the name of the update text file.
                            Folder = "Games/1.7.10";
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
                            M = "false";
                            File.WriteAllText(".\\Configs\\STM.txt", M);

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

                            // Starts the timer.
                            timer.Start();
                        }

                        // Runs code if an error occurs.
                        catch (Exception ex)
                        {
                            // Creates a file path and name for the error log, creates the error message, creates the file and writes the error message to the log file.
                            string ErrorLog = @".\Logs\Update_Error." + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";
                            string ErrorMsg = "The following error occured while performing the server update:" + Environment.NewLine + ex.ToString();
                            File.Create(ErrorLog).Dispose();
                            File.WriteAllText(ErrorLog, ErrorMsg);

                            // Restarts the Program.
                            Console.Clear();
                            Process.Start(".\\Minecraft Server Sync.exe");
                            Environment.Exit(0);
                        }
                    }

                    //Runs code if condition met.
                    else
                    {
                        // Adds line breaks to the console.
                        Console.Write("\n");

                        // Updates SE to true and writes string to text file.
                        M = "false";
                        File.WriteAllText(".\\Configs\\STM.txt", M);
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