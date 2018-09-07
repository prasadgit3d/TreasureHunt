using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(TransformationTask), true)]
    public class TransformationTaskEditor : LerpTaskEditor
    {
        protected new TransformationTask TargetElement
        {
            get
            {
                return target as TransformationTask;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.changed = false;

            GUILayout.BeginHorizontal();
            TargetElement.IsUsingDynamicTransforms = GUILayout.Toggle(TargetElement.IsUsingDynamicTransforms, "Dynamic", GUI.skin.button);
            TargetElement.SnapOnStop = GUILayout.Toggle(TargetElement.SnapOnStop, "Snap On Stop", GUI.skin.button);
                        if (GUILayout.Button("Swap"))
            {
                Vector3 tempPosition = TargetElement.StartPosition;
                TargetElement.StartPosition = TargetElement.EndPosition;
                TargetElement.EndPosition = tempPosition;

                Quaternion tempRotation = TargetElement.StartRotation;
                TargetElement.StartRotation = TargetElement.EndRotation;
                TargetElement.EndRotation = tempRotation;

                Vector3 tempScale = TargetElement.StartScale;
                TargetElement.StartScale = TargetElement.EndScale;
                TargetElement.EndScale = tempScale;

                Transform temp = TargetElement.StartTransform;
                TargetElement.StartTransform = TargetElement.EndTransform;
                TargetElement.EndTransform = temp;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(GUI.skin.button);
            
            Transform obj = (Transform)EditorGUILayout.ObjectField("Start", TargetElement.StartTransform, typeof(Transform), true);
            if (obj != null)
            {
                if (TargetElement.IsTranslationEnabled)
                {
                    TargetElement.StartPosition = obj.position;
                }
                if (TargetElement.IsRotationEnabled)
                {
                    TargetElement.StartRotation = obj.rotation;
                }

                if (TargetElement.IsScalingEnabled)
                {
                    TargetElement.StartScale = obj.localScale;
                }
            }
            TargetElement.StartTransform = obj;
            TargetElement.StartPosition = EditorGUILayout.Vector3Field("Position", TargetElement.StartPosition);
            Vector3 rotStart = EditorGUILayout.Vector3Field("Rotation", TargetElement.StartRotation.eulerAngles);
            TargetElement.StartRotation = Quaternion.Euler(rotStart);
            TargetElement.StartScale = EditorGUILayout.Vector3Field("Scale", TargetElement.StartScale);
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.button);
            Transform obj1 = (Transform)EditorGUILayout.ObjectField("End", TargetElement.EndTransform, typeof(Transform), true);
            if (obj1 != null)
            {
                if (TargetElement.IsTranslationEnabled)
                {
                    TargetElement.EndPosition = obj1.position;
                }
                if (TargetElement.IsRotationEnabled)
                {
                    TargetElement.EndRotation = obj1.rotation;
                }

                if (TargetElement.IsScalingEnabled)
                {
                    TargetElement.EndScale = obj1.localScale;
                }
            }
            TargetElement.EndTransform = obj1;

            TargetElement.EndPosition = EditorGUILayout.Vector3Field("Position", TargetElement.EndPosition);
            Vector3 rotEnd = EditorGUILayout.Vector3Field("Rotation", TargetElement.EndRotation.eulerAngles);
            TargetElement.EndRotation = Quaternion.Euler(rotEnd);
            TargetElement.EndScale = EditorGUILayout.Vector3Field("Scale", TargetElement.EndScale);
            EditorGUILayout.EndVertical();

            TargetElement.TargetTransform = (Transform)EditorGUILayout.ObjectField("Target", TargetElement.TargetTransform, typeof(Transform), true);

            GUILayout.BeginHorizontal();
            TargetElement.IsTranslationEnabled = GUILayout.Toggle(TargetElement.IsTranslationEnabled, "Translate", GUI.skin.button);

            TargetElement.IsRotationEnabled = GUILayout.Toggle(TargetElement.IsRotationEnabled, "Rotate", GUI.skin.button);

            TargetElement.IsScalingEnabled = GUILayout.Toggle(TargetElement.IsScalingEnabled, "Scale", GUI.skin.button);
            GUILayout.EndHorizontal();
        }
    }
}