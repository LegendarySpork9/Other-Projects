using System.Security.Cryptography;
using System.Text;

namespace ServerStatusSite.Functions
{
    public class HashFunction
    {
        // Converts the given string to its hashed value.
        public string HashString(string value)
        {
            string hashString = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                StringBuilder hashedValue = new StringBuilder();
                SHA512 shaHash = SHA512.Create();

                byte[] hashBytes = shaHash.ComputeHash(Encoding.UTF8.GetBytes(value));

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashedValue.Append(hashBytes[i].ToString("x2"));
                }

                hashString = hashedValue.ToString();
            }

            return hashString;
        }
    }
}
