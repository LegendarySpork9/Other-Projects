using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Encryption_Text_Program
{
    class Decryption
    {
        // Stores user's response so it can be decrypted.
        private string FiletoDecrypt;

        // Stores alphabet used in caesar cipher.
        private string CCShift = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Stores the result of the decryption.
        private string DecryptedFile;

        // Stores the current character of the decryption process.
        private string CurrentCharacter;

        // Stores the decryption.
        private string TexttoDecrypt;

        // Stores the ceasar cipher shift key.
        private string CSKey;

        // Stores the Shift Key current characters.
        private int RShift;
        private int RorR;

        // Temporarily stores the ceasar cipher shift key.
        private int CSKeyTemp;

        // Stores the character position in the CCShift variable.
        private int CharacterPos;

        // Stores the current position in the CSKey variable.
        private int CurrentKeyPosition = 0;

        // Get set function to store user's response in a private variable.
        public string File
        {
            // Gets the variables current value.
            get
            {
                // Retrieves variables current value.
                return FiletoDecrypt;
            }

            // Sets the variables value.
            set
            {
                // Runs code if value is not null.
                if (value != null)
                {
                    // Sets the variables value to the user's input.
                    FiletoDecrypt = value;

                    // Calls the file encryption function.
                    FileDecryption();
                }

                // Runs code if value is anything else.
                else
                {
                    // Sets the variables value to null.
                    FiletoDecrypt = null;
                }
            }
        }

        // File Decryption Function
        private async void FileDecryption()
        {
            using (FileStream fileStream = new(FiletoDecrypt, FileMode.Open))
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] iv = new byte[aes.IV.Length];

                    int numBytesToRead = aes.IV.Length;
                    int numBytesRead = 0;

                    while (numBytesToRead > 0)
                    {
                        int n = fileStream.Read(iv, numBytesRead, numBytesToRead);

                        if (n == 0) break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    byte[] key =
                    {
                        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                    };

                    using (CryptoStream cryptoStream = new(fileStream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    {
                        using (StreamReader decryptReader = new(cryptoStream))
                        {
                            DecryptedFile = await decryptReader.ReadToEndAsync();                            
                        }
                    }
                }
            }

            string[] LastLine = DecryptedFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in LastLine)
            {
                if (line.Contains("%&%:") == true)
                {
                    CSKey = line.Replace("%&%:", "");
                }

                else if (line.Contains(Environment.NewLine) == true)
                {
                    TexttoDecrypt += Environment.NewLine;
                }

                else
                {
                    TexttoDecrypt += line;
                }
            }

            Console.WriteLine(CSKey);

            CSKeyTemp = Int32.Parse(CSKey, System.Globalization.NumberStyles.HexNumber);
            CSKey += CSKeyTemp.ToString();

            Console.WriteLine(CSKey);
            
            // Loops the length of the string.
            for (int pos = 0; pos != TexttoDecrypt.Length; pos++)
            {
                // Stores the current character of the string.
                CurrentCharacter = TexttoDecrypt[pos].ToString();
            
                // Loops through the alphabet variable to find the character position.
                for (int x = 0; x != CCShift.Length; x++)
                {
                    // Runs code if it charcters match.
                    if (CCShift[x].ToString() == CurrentCharacter)
                    {
                        // Stores the current characters position.
                        CharacterPos = x;
                    }
                }
            
                // Runs the section of code three times.
                for (int y = 0; y != 3; y++)
                {
                    // Stores the current shift values in variables.
                    RShift = Convert.ToInt32(CSKey[CurrentKeyPosition]);
                    RorR = Convert.ToInt32(CSKey[CurrentKeyPosition + 1]);
            
                    Console.WriteLine(RShift.ToString());
                    Console.WriteLine(RorR.ToString());
            
                    // Runs code if the variable equals 1.
                    if (RorR == 1)
                    {
                        // Performs the letter shift.
                        CharacterPos += RShift;
            
                        // Runs code if the shift goes over the string length.
                        if (CharacterPos > 51)
                        {
                            // Puts the shift to run from the start.
                            CharacterPos -= 51;
                        }
            
                        //Console.WriteLine(CharacterPos.ToString());
                    }
            
                    // Runs code if the variable equals 2.
                    if (RorR == 2)
                    {
                        // Performs the letter shift.
                        CharacterPos -= RShift;
            
                        // Runs code if the shift goes negative.
                        if (CharacterPos < 0)
                        {
                            // Puts the shift to run from the end.
                            CharacterPos = 52 + CharacterPos;
                        }
            
                        //Console.WriteLine(CharacterPos.ToString());
                    }
            
                    CurrentKeyPosition += 2;
                }
            
                // Stores the encrypted character in the variable.
                DecryptedFile += CCShift[CharacterPos].ToString();
            
                Console.WriteLine(DecryptedFile);
            }
        }
    }
}
