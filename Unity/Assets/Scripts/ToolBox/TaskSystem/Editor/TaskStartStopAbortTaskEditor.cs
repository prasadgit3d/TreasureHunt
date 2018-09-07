using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(TaskStartStopAbortTask), true)]
    public class TaskStartStopAbortTaskEditor : TaskBaseEditor
    {
        private ReorderableList list;

        protected new TaskStartStopAbortTask TargetElement 
        {
            get
            {
                return target as TaskStartStopAbortTask;
            }
        }

        void OnEnable()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_TaskList"), true, true, true, true);

            list.drawElementCallback = DrawElements;
            list.drawHeaderCallback = DrawHeader;
        }

        void DrawElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            GUI.BeginGroup(rect);

            EditorGUI.PropertyField(new Rect(0, 2, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_task"), new GUIContent("Prasad","wallah"));
            TargetElement.m_TaskList[index].IncludeInTask = GUI.Toggle(new Rect(rect.width - 80, 2, 15, EditorGUIUtility.singleLineHeight), TargetElement.m_TaskList[index].IncludeInTask, "");
            EditorGUI.PropertyField(new Rect(rect.width - 60, 2, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_action"), GUIContent.none);

            GUI.EndGroup();
        }

        void DrawHeader(Rect a_rect)
        {
            EditorGUI.LabelField(new Rect(a_rect.x, a_rect.y, a_rect.width - 100, EditorGUIUtility.singleLineHeight), "Objects");
            EditorGUI.LabelField(new Rect(a_rect.x + a_rect.width - 95, a_rect.y, 45, EditorGUIUtility.singleLineHeight), "Include");
            EditorGUI.LabelField(new Rect(a_rect.x + a_rect.width - 45, a_rect.y, 45, EditorGUIUtility.singleLineHeight), "Action");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}