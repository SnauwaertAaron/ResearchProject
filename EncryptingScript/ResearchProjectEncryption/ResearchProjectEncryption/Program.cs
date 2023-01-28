using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ResearchProjectEncryption
{
    class Program
    {
        static void Main(string[] args)
        {
            string encriptionKey = GenerateEncryptionKey();
            Console.WriteLine(encriptionKey);
            File.WriteAllText("EK.txt", encriptionKey);

            byte[] encryptedConnectionstring= EncryptStringToBytes_Aes("<IoTHub_connectionstring>", encriptionKey);
            Console.WriteLine(encryptedConnectionstring);
            File.WriteAllBytes("IoTHub_connstr.bin", encryptedConnectionstring);
        }


        static string GenerateEncryptionKey()
        {
            return Encoding.UTF8.GetBytes("<MySuperSecretKey>").ToString();
        }

        static byte[] EncryptStringToBytes_Aes(string plainText, string password)
        {
            byte[] encrypted;
            byte[] salt = new byte[] { 9, 2, 0, 0, 8, 7, 3, 1 };

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }
            return encrypted;
        }
    }
}
