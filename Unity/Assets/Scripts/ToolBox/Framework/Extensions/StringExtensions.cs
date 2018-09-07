using System.Collections;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;


namespace SillyGames.SGBase
{
    public static class StringExtensions
    {
        /// <summary>
        /// Matches a string against the specified regular expression.
        /// </summary>
        /// <param name="aThis">String to compare.</param>
        /// <param name="aExpr">Regular expression to use.</param>
        public static bool MatchesRegEx(this string aThis, string aExpr)
        {
            Regex r = new Regex(aExpr);
            return (r.IsMatch(aThis));
        }

        /// <summary>
        /// Check string present in specific sting.
        /// </summary>
        /// <param name="aThis">String to compare to.</param>
        /// <param name="aToCheck">String to compare with.</param>
        /// <param name="aComp">String comparison options</param>
        /// <returns>True if sting contains toCheck string as per comparison option</returns>
        public static bool Contains(this string aThis, string aToCheck, StringComparison aComp)
        {
            return aThis.IndexOf(aToCheck, aComp) >= 0;
        }

        /// <summary>
        /// Generates the MD5 hash for the specified input string.
        /// </summary>
        /// <returns>128-bit MD5 hash as a hex-encoded string.</returns>
        /// <param name="text">Text.</param>
        public static string GetHashMD5(this string aInput)
        {
            var md5 = MD5.Create();

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(aInput);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
        /// <summary>
        /// Shorten string for max length up to 'a_iLength' + 'a_filler'
        /// </summary>
        /// <param name="a_source"></param>
        /// <param name="a_iLength"></param>
        /// <param name="a_filler"></param>
        /// <returns></returns>
        public static string Shorten(this string a_source, int a_iLength, string a_filler = "...")
        {
            if (a_source.Length <= (a_iLength + a_filler.Length))
            {
                return a_source;
            }
            else
            {
                return a_source.Substring(0, a_iLength) + a_filler;
            }
        }

    }
}