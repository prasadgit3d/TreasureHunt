using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(Task), true)]
    public class TaskBaseEditor : Editor
    {
        protected Task TargetElement
        {
            get
            {
                return target as Task;
            }
        }

        private bool drawDefaultInspector = false;

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, "Draw Default Inspector", GUI.skin.button);
            TargetElement.TaskName = EditorGUILayout.TextField("Task Name: ", TargetElement.TaskName);

            GUILayout.BeginHorizontal();
            SerializedProperty onStartEvent = serializedObject.FindProperty("OnStartEvent");
            EditorGUILayout.PropertyField(onStartEvent, GUILayout.Width((Screen.width / 3) - 10));
            SerializedProperty onUpdateEvent = serializedObject.FindProperty("OnUpdateEvent");
            EditorGUILayout.PropertyField(onUpdateEvent, GUILayout.Width((Screen.width / 3) - 10));
            SerializedProperty onEndEvent = serializedObject.FindProperty("OnEndEvent");
            EditorGUILayout.PropertyField(onEndEvent, GUILayout.Width((Screen.width / 3) - 10));
            GUILayout.EndHorizontal();

            TargetElement.ExecuteOnStart = GUILayout.Toggle(TargetElement.ExecuteOnStart, "Execute on Start", GUI.skin.button);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Play" + (TargetElement.IsPlaying ? "(Playing)" : "")))
            {
                TargetElement.Execute();
            }

            TargetElement.IsPaused = GUILayout.Toggle(TargetElement.IsPaused, "Pause" + (TargetElement.IsPaused ? "d" : ""), GUI.skin.button);

            if (GUILayout.Button("Stop"))
            {
                TargetElement.Stop();
            }

            if(GUILayout.Button("Abort"))
            {
                TargetElement.Abort();
            }
            
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            OnCustomUI();
        }

        protected virtual void OnCustomUI()
        {
            if(drawDefaultInspector)
            {
                DrawDefaultInspector();
            }
            
        }
    }
}
