using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;
using UnityEditor;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(AreaBasedTask), true)]
    public class AreaBasedTaskEditor : TaskBaseEditor
    {
        protected new AreaBasedTask TargetElement
        {
            get
            {
                return target as AreaBasedTask;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.changed = false;
            TargetElement.TargetTransform = (Transform)EditorGUILayout.ObjectField("Target", TargetElement.TargetTransform, typeof(Transform), true);
            TargetElement.HitPoint = (Transform)EditorGUILayout.ObjectField("Hit Point", TargetElement.HitPoint, typeof(Transform), true);
            TargetElement.HitPointRadius = Mathf.Abs(EditorGUILayout.FloatField("Hit Point Radius", TargetElement.HitPointRadius));
        }
    }
}
