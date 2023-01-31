using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleWallet.Common
{
    public static class Helpers
    {
        public static string GenerateAccountNumber()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }
        
        public static string PasswordHash(string password)
        {
            using MD5 md5 = MD5.Create();
            md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            if (result != null)
                foreach (var t in result)
                {
                    strBuilder.Append(t.ToString("x2"));
                }

            return strBuilder.ToString();
        }
        
        public static bool VerifyPassword(string password, string hash)
        {
            return StringComparer.OrdinalIgnoreCase.Compare(PasswordHash(password), hash) == 0;
        }
    }
}