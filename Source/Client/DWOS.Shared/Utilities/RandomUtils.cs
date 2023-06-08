using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.Shared.Utilities
{
    /// <summary>
    /// Defines randomization-related utility methods.
    /// </summary>
    public static class RandomUtils
    {
        /// <summary>
        /// Generates a random string of the given length.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetRandomString(int size)
        {
            var r = new Random();
            var builder = new StringBuilder();
            char ch;
            
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * r.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates a numerical string of the given length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomDigits(int length)
        {
            return string.Concat(RandomDigits().Take(length));
        }

        private static IEnumerable<int> RandomDigits()
        {
            var rng = new Random(System.Environment.TickCount);
            while (true) yield return rng.Next(10);
        }

        /// <summary>
        /// Creates a random password string of a given length.
        /// </summary>
        /// <param name="passwordLength"></param>
        /// <returns></returns>
        public static string CreateRandomPassword(int passwordLength)
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            var chars = new char[passwordLength];
            var rd = new Random();

            for(int i = 0; i < passwordLength; i++)
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];

            return new string(chars);
        }


    }
}
