using System.Security.Cryptography;
using System.Text;

namespace HonestPlates.Data
{
    // Encrpyting & Decrypting using AES-256 (using a 256-bit key)
    public static class Encryption
    {
        // 256-bit AES key, sourced from Connection.cs (pp1, pp2)
        // Make a 256-bit key from both pp1 and pp2 every time 100,000 times.
        private static readonly byte[] key = Rfc2898DeriveBytes.Pbkdf2( // turns pass into key
            Encoding.UTF8.GetBytes(EncryptionSecret.pp1),
            Encoding.UTF8.GetBytes(EncryptionSecret.pp2),
            100000, // scrambles both pp1 and pp2 
            HashAlgorithmName.SHA256,
            32); // length of the key: 32 bytes = 256 bits.

        // Plain text input; Base64 text output (randomIV stored infront of encypted byte)
        // random IV + lock w/ key, apply IV on front and turn into text.
        public static string Encrypt (string plainText) 
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();

                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = aes.EncryptCbc(plainBytes, aes.IV);

                // Result = IV (16 bytes) + encrypted bytes
                byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

                return Convert.ToBase64String(result);

            }
        }

        // Input Base64 text fron Encrypt and plain text outputs.
        // Exception if text DID not from Encryt(), or secret changed.
        // Convert text back into bytes, take off the IV, the unlock w/ key.
        public static string Decrypt (string encryptedText)
        {
            byte[] allBytes = Convert.FromBase64String(encryptedText);

            // Split the converted text back to IV (first 16 bytes) and encrypted bytes
            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[allBytes.Length - 16];
            Buffer.BlockCopy(allBytes, 0, iv, 0, 16);
            Buffer.BlockCopy(allBytes, 16, encryptedBytes, 0, encryptedBytes.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                byte[] plainBytes = aes.DecryptCbc(encryptedBytes, iv);
                return Encoding.UTF8.GetString(plainBytes);
            }
         }
    }
}