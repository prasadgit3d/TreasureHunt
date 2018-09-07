using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(TriggerBasedTask))]
    public class TriggerBasedTaskEditor : TaskBaseEditor
    {
        protected new TriggerBasedTask TargetElement
        {
            get
            {
                return target as TriggerBasedTask;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.changed = false;

            TargetElement.TargetCollider = (Collider)EditorGUILayout.ObjectField("Target Collider", TargetElement.TargetCollider, typeof(Collider), true);
            TargetElement.HitCollider = (Collider)EditorGUILayout.ObjectField("Hit Collider", TargetElement.HitCollider, typeof(Collider), true);
        }
    }
}