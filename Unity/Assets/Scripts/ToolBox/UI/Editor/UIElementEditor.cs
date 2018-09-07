using SillyGames.SGBase.UI;
using UnityEditor;
using UnityEngine;

namespace SillyGames.SGBase.UI
{
    [CustomEditor(typeof(UIElement), true)]
    public class UIElementEditor : Editor
    {
        protected UIElement TargetElement
        {
            get
            {
                return target as UIElement;
            }
        }

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(TargetElement.IsShown ? "Hide" : "Show"))
            {
                if (TargetElement.IsShown)
                {
                    TargetElement.Hide();
                }
                else
                {
                    TargetElement.Show();
                }
            }

            GUILayout.EndHorizontal();
            TargetElement.OnCustomInspectorPrivate();
            DrawDefaultInspector();
        }
    }
}