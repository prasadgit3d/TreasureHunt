using UnityEngine;
using SillyGames.SGBase.TaskSystem;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(ObjectEnableDisableTask), true)]
    public class ObjectEnableDisableTaskEditor : TaskBaseEditor
    {
        private ReorderableList list;

        protected new ObjectEnableDisableTask TargetElement 
        {
            get
            {
                return target as ObjectEnableDisableTask;
            }
        }

        void OnEnable()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Objects"), true, true, true, true);
            list.drawElementCallback = DrawElements;
            list.drawHeaderCallback = DrawHeader;
        }

        void DrawElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            GUI.BeginGroup(rect);
            
            EditorGUI.PropertyField(new Rect(0, 2, rect.width - 90, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_UnityObject"), GUIContent.none);
            TargetElement.m_Objects[index].IncludeInTask = GUI.Toggle(new Rect(rect.width - 75, 2, 15, EditorGUIUtility.singleLineHeight), TargetElement.m_Objects[index].IncludeInTask, "");
            TargetElement.m_Objects[index].Enable = GUI.Toggle(new Rect(rect.width - 30, 2, 15, EditorGUIUtility.singleLineHeight), TargetElement.m_Objects[index].Enable, "");

            GUI.EndGroup();
        }

        void DrawHeader(Rect a_rect)
        {
            EditorGUI.LabelField(new Rect(a_rect.x, a_rect.y, a_rect.width - 90, EditorGUIUtility.singleLineHeight), "Objects");
            EditorGUI.LabelField(new Rect(a_rect.x + a_rect.width - 90, a_rect.y, 45, EditorGUIUtility.singleLineHeight), "Include");
            EditorGUI.LabelField(new Rect(a_rect.x + a_rect.width - 45, a_rect.y, 45, EditorGUIUtility.singleLineHeight), "Enable");
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