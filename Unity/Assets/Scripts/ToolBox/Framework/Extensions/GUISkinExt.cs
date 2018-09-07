using UnityEngine;
using System.Collections;
using System;

namespace SillyGames.SGBase
{
    public static class GUISkinExt
    {
        /// <summary>
        /// calculate height of a guilayout UI element (button, label etc..) based on style, width and text content
        /// </summary>
        /// <param name="a_skin">skin from wich the style should be taken</param>
        /// <param name="a_guistyleName">name of a style</param>
        /// <param name="a_strTextContent">text content for the UI element</param>
        /// <param name="a_fStyleWidth">width of the UI element</param>
        /// <returns></returns>
        public static float CalcDynamicHeight(this GUISkin a_skin, string a_guistyleName, string a_strTextContent, float a_fStyleWidth)
        {
            GUIStyle bt = a_skin.GetStyle(a_guistyleName);
            if (bt == null)
            {
                bt = a_skin.GetStyle(a_guistyleName);
                if (bt == null)
                {
                    throw new Exception("Style '" + a_guistyleName + "' not found in GUISkin '" + a_skin.name + "'");
                }
            }
            GUIContent gc = new GUIContent(a_strTextContent);
            float length = bt.CalcHeight(gc, a_fStyleWidth);
            return length;
        }
    }
}