using UnityEngine;
using UnityEditor;
using System;
using SillyGames.SGBase;

namespace SillyGames.SGBaseEditor
{
    [CustomEditor(typeof(SoundManager))]
    class SoundManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
//#if UNITY_EDITOR
        var soundManager = target as SoundManager;
        soundManager.OnEditorGUI(base.OnInspectorGUI);
//#endif
        }
    }
     
    public static class SoundManagerExtension
    {
        //AudioSegmentEntry entry_ = new AudioSegmentEntry();
        //AudioFileEntry fileEntry = new AudioFileEntry();
        private enum EAction
        {
            None,
            Delete,
            Play,
            AddFileEntry,
            AddAudioSegmentEntry,
            Duplicate,
            PlayAdditive
        }
        public static void OnEditorGUI(this SoundManager soundManager, Action a_defaultInspectorRenderer)
        {
            soundManager.AudioSourcePoolCount = EditorGUILayout.IntField(new GUIContent("Initial AudioSource Count: ","Initial number of Audiosources it will created on Init"), soundManager.AudioSourcePoolCount);
            if (GUILayout.Button("Add audio file entry"))
            {
                soundManager.AddNewFileEntryPrivate(new SoundManager.AudioFileEntry());
            }

            if (soundManager.FileEntriesPrivate.Count > 0)
            {
                for (int i = 0; i < soundManager.FileEntriesPrivate.Count; i++)
                {
                    var fileEntry = soundManager.FileEntriesPrivate[i];
                    var action = soundManager.RenderAudioFileEntry(fileEntry);
                    switch (action)
                    {
                        case EAction.AddFileEntry:
                            {
                                fileEntry.SoundEntries.Add(new SoundManager.AudioSegmentEntry());
                                break;
                            }
                        case EAction.Delete:
                            {
                                Undo.RecordObject(soundManager, "Delete");
                                soundManager.FileEntriesPrivate.RemoveAt(i);

                                --i;
                                break;
                            }
                    }
                }
            }
            else
            {
                //if(GUILayout.Button("Add audio file entry"))
                //{
                //    m_lstFileEntries.Add(new AudioFileEntry());
                //}
            }

        }

        static EAction RenderAudioFileEntry(this SoundManager a_manager, SoundManager.AudioFileEntry a_fileEntry)
        {
            GUILayout.BeginVertical("", "box");
            EAction actionToReturn = EAction.None;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("✖"))
            {
                actionToReturn = EAction.Delete;
            }
            GUILayout.Label("Key: ", GUILayout.Width(30));
            a_fileEntry.FileEntryKey = EditorGUILayout.TextField(a_fileEntry.FileEntryKey);
            GUILayout.Label("Clip: ", GUILayout.Width(30));
            a_fileEntry.AudioClip = (AudioClip)EditorGUILayout.ObjectField("", a_fileEntry.AudioClip, typeof(AudioClip), false);

            GUILayout.EndHorizontal();
            if (a_fileEntry.SoundEntries.Count > 0)
            {
                for (int i = 0; i < a_fileEntry.SoundEntries.Count; i++)
                {
                    var audioSegmentEntry = a_fileEntry.SoundEntries[i];
                    var action = a_manager.RenderAudioSegmentEntry(audioSegmentEntry);
                    switch (action)
                    {
                        case EAction.Delete:
                            {
                                Undo.RecordObject(a_manager, "remove audio segment entry at " + i);
                                a_fileEntry.SoundEntries.RemoveAt(i);
                                --i;
                                break;
                            }
                        case EAction.Duplicate:
                            {
                                a_fileEntry.SoundEntries.Add(new SoundManager.AudioSegmentEntry(audioSegmentEntry));
                                break;
                            }
                        case EAction.Play:
                            {
                                a_manager.Play(audioSegmentEntry.Key);
                                break;
                            }
                        case EAction.PlayAdditive:
                            {
                                a_manager.Play(audioSegmentEntry.Key,true);
                                break;
                            }
                    }
                }
            }
            else
            {
                if (GUILayout.Button("+"))
                {
                    actionToReturn = EAction.AddFileEntry;
                }
            }
            GUILayout.EndVertical();
            return actionToReturn;
        }

        static EAction RenderAudioSegmentEntry(this SoundManager a_soundManager, SoundManager.AudioSegmentEntry a_entry)
        {
            EAction actionToReturn = EAction.None;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("✖", GUILayout.Width(20)))
            {
                actionToReturn = EAction.Delete;
            }
            bool bGUIEnabled = GUI.enabled;
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("♫", GUILayout.Width(20)))
            {
                actionToReturn = EAction.Play;
            }
            if (GUILayout.Button("♫♫", GUILayout.Width(20)))
            {
                actionToReturn = EAction.PlayAdditive;
            }
            GUI.enabled = bGUIEnabled;

            a_entry.Key = EditorGUILayout.TextField(a_entry.Key);

            GUILayout.Label("S: ", GUILayout.Width(20));
            a_entry.Start = EditorGUILayout.FloatField("", a_entry.Start, GUILayout.Width(50));
            GUILayout.Label("E: ", GUILayout.Width(20));
            a_entry.End = EditorGUILayout.FloatField("", a_entry.End, GUILayout.Width(50));
            GUILayout.Label("T: ", GUILayout.Width(20));
            a_entry.Duration = EditorGUILayout.FloatField("", a_entry.Duration, GUILayout.Width(50));
            if (GUILayout.Button("❏"))
            {
                actionToReturn = EAction.Duplicate;
            }
            GUILayout.EndHorizontal();
            return actionToReturn;
        }
    }
}