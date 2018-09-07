using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(SerialTask), true)]
    public class SerialTaskEditor : TaskBaseEditor
    {
        private ReorderableList list;
        protected new SerialTask TargetElement 
        {
            get
            {
                return target as SerialTask;
            }
        }

        void OnEnable()
        {
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_TaskList"), true, true, true, true);

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => { var element = list.serializedProperty.GetArrayElementAtIndex(index); rect.y += 2; EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none); };

            list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Task List"); };
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
