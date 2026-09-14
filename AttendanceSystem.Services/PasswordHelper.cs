using AttendanceSystem.Models;

namespace AttendanceSystem.Services
{
    public static class PasswordHelper
    {
        // Ab hash ki jagah reversible encryption use ho raha hai taake admin
        // zarurat par employee ka password dobara dekh sake.
        public static string Encrypt(string plainPassword) => Encryption.EncryptString(plainPassword);
        public static string Decrypt(string encryptedPassword) => Encryption.DecryptString(encryptedPassword);
    }
}