using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AttendanceSystem.Models
{
    public static class Encryption
    {
        private static readonly string Key = "AttendanceSystem2026Key123456789012";

        public static string EncryptString(string plainText)
        {
            using Aes aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(Key.Substring(0, 32));
            aes.GenerateIV();

            using MemoryStream ms = new MemoryStream();

            ms.Write(aes.IV, 0, aes.IV.Length);

            using (CryptoStream cs = new CryptoStream(
                ms,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write))
            {
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string DecryptString(string encryptedText)
        {
            byte[] fullData = Convert.FromBase64String(encryptedText);

            using Aes aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(Key.Substring(0, 32));

            byte[] iv = new byte[16];
            Array.Copy(fullData, 0, iv, 0, 16);
            aes.IV = iv;

            using MemoryStream ms = new MemoryStream();

            using (CryptoStream cs = new CryptoStream(
                ms,
                aes.CreateDecryptor(),
                CryptoStreamMode.Write))
            {
                cs.Write(fullData, 16, fullData.Length - 16);
                cs.FlushFinalBlock();
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}