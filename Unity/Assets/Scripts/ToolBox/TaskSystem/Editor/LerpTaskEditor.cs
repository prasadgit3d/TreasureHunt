using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(LerpTask), true)]
    public class LerpTaskEditor : TaskBaseEditor
    {
        protected new LerpTask TargetElement
        {
            get
            {
                return target as LerpTask;
            }
        }

        [SerializeField]
        private bool previewEnabled = false;

        [SerializeField]
        private float fraction = 0.0f;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.changed = false;

            TargetElement.IsExecutingReverse = GUILayout.Toggle(TargetElement.IsExecutingReverse, "Execute in Reverse", GUI.skin.button);
            
            TargetElement.UseTimeToUpdate = GUILayout.Toggle(TargetElement.UseTimeToUpdate, "Use Time to Update", GUI.skin.button);

            if (TargetElement.UseTimeToUpdate)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Duration", GUILayout.MinWidth(65.0f));
                TargetElement.TimeToComplete = EditorGUILayout.FloatField(TargetElement.TimeToComplete);
                //GUILayout.Label("Repetitions", GUILayout.MinWidth(65.0f));
                //TargetElement.Iterations = EditorGUILayout.IntField(TargetElement.Iterations);
                GUILayout.EndHorizontal();
            }


            GUILayout.Label("CurrentValue: " + TargetElement.CurrentValue);

            TargetElement.Mode = (LerpTask.WrapMode)EditorGUILayout.EnumPopup("Wrap Mode: ", TargetElement.Mode);

            GUILayout.BeginHorizontal();
            TargetElement.IsUsingAnimationCurve = GUILayout.Toggle(TargetElement.IsUsingAnimationCurve, "Use Animation Curve");

            if (TargetElement.IsUsingAnimationCurve)
            {
                if (TargetElement.Curve != null)
                {
                    TargetElement.Curve = EditorGUILayout.CurveField(TargetElement.Curve);
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            previewEnabled = GUILayout.Toggle(previewEnabled, "Preview", GUI.skin.button);
            var guiEnabled = GUI.enabled;
            GUI.enabled = previewEnabled;
            fraction = EditorGUILayout.Slider(fraction, 0.0f, 1.0f);
            if (previewEnabled)
            {
                TargetElement.OnInspectorSliderUpdateInternal(TargetElement.IsUsingAnimationCurve ? TargetElement.Curve.Evaluate(fraction) : fraction);
            }
            GUI.enabled = guiEnabled;
            GUILayout.EndHorizontal();
        }
    }
}
