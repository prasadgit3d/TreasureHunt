using UnityEngine;
using System.Collections;

namespace SillyGames.SGBase.Localization
{
    public enum eIndentType
    {
        eType_Horizontal,
        eType_Vertical
    }
    public delegate void RenderFunction();
    public static class EditorUtils
    {
        /// <summary>
        /// Envelops the RenderFunction with calls to GUILayout.BeginHorizontal and GUILayoutEndHorizontal
        /// </summary>
        /// <param name="func">function to be enveloped.</param>
        /// <param name="options">Options.</param>
        public static void HAutoLayout(RenderFunction func, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            func.Invoke();
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Envelops the RenderFunction with calls to GUILayout.BeginHorizontal and GUILayoutEndHorizontal
        /// </summary>
        /// <param name="func">function to be enveloped.</param>
        /// <param name="options">Options.</param>
        public static void VAutoLayout(RenderFunction func, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            func.Invoke();
            GUILayout.EndVertical();
        }


        public static Vector2 ScrollAutoLayout(Vector2 scrollPos, RenderFunction func, params GUILayoutOption[] options)
        {
            Vector2 pos = GUILayout.BeginScrollView(scrollPos);
            func.Invoke();
            GUILayout.EndScrollView();
            return pos;
        }
    }
}