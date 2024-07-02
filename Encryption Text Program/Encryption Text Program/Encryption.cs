using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Encryption_Text_Program
{
    class Encryption
    {
        // Variables containing Random properties.
        Random Shift = new Random();
        Random LorR = new Random();

        // Stores user's responses so they can be encrypted.
        private string TexttoEncrypt;
        private string FiletoEncrypt;

        // Stores alphabet used in caesar cipher.
        private string CCShift = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // Stores the result of the encryption.
        private string EncryptedText;
        private string EncryptedFile;

        // Stores the current character of the encryption process.
        private string CurrentCharacter;

        // Stores the random numbers;
        private int RShift;
        private int RLorR;

        // Stores the random numbers generated for the RShift and RLorR variables.
        private string CSKey;

        // Temporarily stores the ceasar cipher shift key.
        private int CSKeyTemp;

        // Stores the character position in the CCShift variable.
        private int CharacterPos;

        // Get set functrion to store user's response in a private variable.
        public string Text
        {
            // Gets the variables current value.
            get
            {
                // Retrieves variables current value.
                return TexttoEncrypt;
            }

            // Sets the variables value.
            set
            {
                // Runs code if value is not null.
                if (value != null)
                {
                    // Sets the variables value to the user's input.
                    TexttoEncrypt = value;

                    // Calls the text encryption function.
                    TextEncryption();
                }

                // Runs code if value is anything else.
                else
                {
                    // Sets the variables value to null.
                    TexttoEncrypt = null;
                }
            }
        }

        // Get set function to store user's response in a private variable.
        public string File
        {
            // Gets the variables current value.
            get
            {
                // Retrieves variables current value.
                return FiletoEncrypt;
            }

            // Sets the variables value.
            set
            {
                // Runs code if value is not null.
                if (value != null)
                {
                    // Sets the variables value to the user's input.
                    FiletoEncrypt = value;

                    // Calls the file encryption function.
                    FileEncryption();
                }

                // Runs code if value is anything else.
                else
                {
                    // Sets the variables value to null.
                    FiletoEncrypt = null;
                }
            }
        }

        // Text Encryption Function.
        private void TextEncryption()
        {
            // Runs the section of code three times;
            for (int w = 0; w != 3; w++)
            {
                // Stores the random numbers in variables.
                RShift = Shift.Next(1, 6);
                RLorR = LorR.Next(1, 3);

                // Stores the generated numbers in the CSKey variable to be used in decryption.
                CSKey += RShift;
                CSKey += RLorR;
            }

            // Loops the length of the string.
            for (int pos = 0; pos != TexttoEncrypt.Length; pos++)
            {
                // Stores the current character of the string.
                CurrentCharacter = TexttoEncrypt[pos].ToString();

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
                    int currentRLorR = Convert.ToInt32(CSKey[y]);
                    int CurrentRShift = Convert.ToInt32(CSKey[y + 1]);

                    // Runs code if the variable equals 1.
                    if (currentRLorR == 1)
                    {
                        // Performs the letter shift.
                        CharacterPos -= CurrentRShift;

                        // Runs code if the shift goes negative.
                        if (CharacterPos < 0)
                        {
                            // Puts the shift to run from the end.
                            CharacterPos = 52 + CharacterPos;
                        }
                    }

                    // Runs code if the variable equals 2.
                    if (currentRLorR == 2)
                    {
                        // Performs the letter shift.
                        CharacterPos += CurrentRShift;

                        // Runs code if the shift goes over the string length.
                        if (CharacterPos > 51)
                        {
                            // Puts the shift to run from the start.
                            CharacterPos -= 51;
                        }
                    }
                }

                // Stores the encrypted character in the variable.
                EncryptedText += CCShift[CharacterPos].ToString();
            }

            CSKeyTemp = Convert.ToInt32(CSKey);

            // Adds a new line to seperate the encrypted text from the encrypted key.
            EncryptedText += Environment.NewLine + "%&%:";

            // Converts the key to Hex and adds it to the string to encrypt.
            EncryptedText += CSKeyTemp.ToString("X");

            // Variable containing the FileStream properties with settings.
            using (FileStream fs = new FileStream("Encrypted Output.txt", FileMode.OpenOrCreate))
            {
                // Variable containing Asymmetric Encryption Properties.
                using (Aes aes = Aes.Create())
                {
                    // Data for the key generation.
                    byte[] key =
                    {
                            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                    };

                    // Sets the key to the above byte array.
                    aes.Key = key;

                    // Sets the IV to what's contained in the asymmetric encryption property.
                    byte[] iv = aes.IV;

                    // Adds the IV tothe file stream.
                    fs.Write(iv, 0, iv.Length);

                    // Variable containing the CryptoStream properties with settings.
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // Variable containing the StreamWriter properties.
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            // Adds the Text to encrypt to the streamwriter.
                            sw.WriteLine(EncryptedText);
                        }
                    }
                }
            }

            Console.WriteLine("\nThe text was encrypted.");
        }

        // File Encryption Function
        private void FileEncryption()
        {

        }
    }
}
