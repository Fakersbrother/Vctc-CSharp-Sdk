using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Cobra.Utils
{
    public static class HashUtils
    {

        public static string HMAC_SHA256(string message, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                secret = "";
            }

            byte[] keyByte = Encoding.ASCII.GetBytes(secret);
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                return ToHex(hmacsha256.ComputeHash(messageBytes));
            }
        }
        public static string ToHex(byte[] input)
        {
            var builder = new StringBuilder(input.Length / 4);
            foreach (var c in input)
            {
                builder.Append(c.ToString("x2"));
            }
            return builder.ToString();
        }
        
    }
}