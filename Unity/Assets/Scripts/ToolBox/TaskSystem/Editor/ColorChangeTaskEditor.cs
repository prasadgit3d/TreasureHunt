using UnityEngine;
using System.Collections;
using UnityEditor;
using SillyGames.SGBase.UI;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(ColorChangeTask), true)]
    public class ColorChangeTaskEditor : LerpTaskEditor
    {
        protected new ColorChangeTask TargetElement
        {
            get
            {
                return target as ColorChangeTask;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.changed = false;

            GUILayout.BeginHorizontal();
            TargetElement.SnapOnStop = GUILayout.Toggle(TargetElement.SnapOnStop, "Snap On Stop", GUI.skin.button);
            if (GUILayout.Button("Swap"))
            {
                Color temp = TargetElement.StartColor;
                TargetElement.StartColor = TargetElement.EndColor;
                TargetElement.EndColor = temp;
            }

            TargetElement.SetStartColorFromTarget = GUILayout.Toggle(TargetElement.SetStartColorFromTarget, "Set Start Color from Target", GUI.skin.button);

            if (TargetElement.SetStartColorFromTarget)
            {
                if (TargetElement.TargetTransform != null)
                {
                    TargetElement.StartColor = TargetElement.TargetTransform.CurrentColor;
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Start:", GUILayout.MinWidth(50.0f));
            TargetElement.StartColor = EditorGUILayout.ColorField(TargetElement.StartColor);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("End:", GUILayout.MinWidth(50.0f));
            TargetElement.EndColor = EditorGUILayout.ColorField(TargetElement.EndColor);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target", GUILayout.MinWidth(50.0f));
            TargetElement.TargetTransform = (UILabelBase)EditorGUILayout.ObjectField(TargetElement.TargetTransform, typeof(UILabelBase), true);
            GUILayout.EndHorizontal();
        }
    }
}
