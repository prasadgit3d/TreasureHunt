using UnityEngine;
namespace SillyGames.SGBase
{
    public static class StringExtensionsUnity
    {
        public static Color32 ToColor(this string a_hex)
        {
            a_hex = a_hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            a_hex = a_hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(a_hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(a_hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(a_hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (a_hex.Length == 8)
            {
                a = byte.Parse(a_hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }
    }
}