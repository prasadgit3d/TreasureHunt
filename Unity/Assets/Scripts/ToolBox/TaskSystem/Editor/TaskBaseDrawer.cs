using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomPropertyDrawer(typeof(Task), true)]
    public class TaskBaseDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float labelWidth = GUI.skin.label.CalcSize(new GUIContent(property.displayName)).x + 20;
            EditorGUI.LabelField(new Rect(position.x, position.y, labelWidth, position.height), property.displayName);
            Task task = (Task)property.objectReferenceValue;
            string taskName = task != null ? task.TaskName : "Test";

            float taskNameWidth = position.width - 150 - labelWidth + 20;
            EditorGUI.TextField(new Rect(labelWidth, position.y, taskNameWidth, position.height), taskName);

            property.objectReferenceValue = EditorGUI.ObjectField(new Rect(taskNameWidth + labelWidth, position.y, 150, position.height), property.objectReferenceValue, typeof(Task), true);
        }
    }
}