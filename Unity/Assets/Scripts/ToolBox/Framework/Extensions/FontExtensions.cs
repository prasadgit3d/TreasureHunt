using System;
using UnityEngine;

namespace SillyGames.SGBase
{
    public static class FontExtensions
    {
        // Returns string pixel width with specified font & size
        public static float GetPixelWidth(this Font a_font,string a_String,  int a_iSize)
        {
            float fWidth = 0.0f;
            a_font.RequestCharactersInTexture(a_String);
            for (int i = 0; i < a_String.Length; i++)
                fWidth += a_font.GetPixelWidth(a_String[i], a_iSize);
            return fWidth;
        }

        // Returns charactor pixel width with specified font
        public static float GetPixelWidth(this Font a_font, char a_Char, int a_iSize)
        {
            CharacterInfo charInfoOut;
            a_font.GetCharacterInfo(a_Char, out charInfoOut, a_iSize);
            return charInfoOut.advance;
        }

    }
}
