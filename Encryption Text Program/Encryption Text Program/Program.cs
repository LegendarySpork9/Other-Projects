using System;
using System.Diagnostics;

namespace Encryption_Text_Program
{
    class Program
    {
        // Main Code
        static void Main(string[] args)
        {
            // Variables containing other class properties.
            Encryption Encrypt = new Encryption();
            Decryption Decrypt = new Decryption();

            // Displays choice of Encrypt or Decrypt to user.
            Console.WriteLine("Welcome to the Text Encryption Program!");
            Console.WriteLine("Do you want to Encrypt or Decrypt?");

            // Stores the response in a new variable. 
            string response = Console.ReadLine();

            // Runs code if the user inputs Encrypt.
            if ((response == "encrypt") || (response == "Encrypt"))
            {
                // Displays choice of Text or File to user.
                Console.WriteLine("\n" + "Would you like to enter text to encrypt or encrypt from a text file?");
                Console.WriteLine("Answer with either text or text file.");

                // Stores the response in a variable.
                response = Console.ReadLine();

                // Runs code if the user inputs Text.
                if ((response == "text") || (response == "Text"))
                {
                    // Displays a request to enter the text to the user.
                    Console.WriteLine("\n" + "Please enter the text that you would like to encrypt.");

                    // Stores the response in a variable.
                    string TexttoEncrypt = Console.ReadLine();

                    // Passes the response the get set fuction in the encryption class.
                    Encrypt.Text = TexttoEncrypt;
                }

                // Runs code if the user inputs File.
                if ((response == "text file") || (response == "Text File"))
                {
                    // Displays a request to enter the file path to the user.
                    Console.WriteLine("\n" + "Please enter the file path for the text file that you would like to encrypt.");
                    Console.WriteLine("E.g. " + @"C:\Users\TobyH\Documents\Text Documents\Example.txt");

                    // Stores the response in a variable.
                    string FiletoEncrypt = Console.ReadLine();

                    // Passes the response the get set fuction in the encryption class.
                    Encrypt.File = FiletoEncrypt;
                }

                // Breaks out of the if statements.
                return;
            }

            // Runs code if the user inputs Decrypt.
            if ((response == "decrypt") || (response == "Decrypt"))
            {
                
                Console.WriteLine("\n" + "Please enter the file path for the text file that you would like to Decrypt.");
                Console.WriteLine("E.g. " + @"C:\Users\TobyH\Documents\Text Documents\Example.txt");
                
                string FiletoDecrypt = Console.ReadLine();

                Decrypt.File = FiletoDecrypt;
            }

            // Runs code if the user hasn't entered one of the two choices.
            else
            {
                // Displays to the user the error.
                Console.WriteLine("\n" + "That is not a valid response, program will now restart");

                // restarts the program.
                Process.Start("Encryption Text Program");
                Environment.Exit(0);
            }
        }
    }
}
