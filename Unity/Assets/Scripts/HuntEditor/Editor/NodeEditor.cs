using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace SillyGames.TreasureHunt.HuntEditor
{
    public class MyWindow : EditorWindow
    {
        public Texture m_texture = null;
        [SerializeField]
        private Rect m_nodeCreationBntRect;
        public int index = 0;
        private Rect m_rect;
        //string myString = "Hello World";
        //bool groupEnabled;
        //bool myBool = true;
        //float myFloat = 1.23f;
        private List<String> m_nodeOptions = new List<String>();
        private List<Type> m_nodeTypes = new List<Type>();
        private static Node[] nodes = null;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/NodeEditor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            MyWindow window = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow));
            RefreshNodes();
            window.wantsMouseMove = true;
            window.FindAllDerivedTypes<Node>();
            window.Show();
            //Testserialization.Start();
        }

        private static void RefreshNodes()
        {
            nodes = GameObject.FindObjectsOfType<Node>();
        }

        void OnGUI()
        {
            if(Event.current.isMouse)
            {
                Node.MousePosition = Event.current.mousePosition;
            }

            if (Event.current.isKey)
            {
                Node.PressedKey = KeyCode.None;
                if (Event.current.type == EventType.KeyDown)
                {

                    // Node.PressedKey =Event.current.keyCode;

                    if (Event.current.keyCode == KeyCode.LeftControl ||
                        Event.current.keyCode == KeyCode.LeftAlt)
                    {
                        Node.PressedKey = Event.current.keyCode;
                    }
                }

            }

            if (GUI.Button(new Rect(Screen.width - 200, 0,200, 25), "Refresh: " + Node.MousePosition))
            {
                RefreshNodes();
            }
            if (nodes != null)
            {
                var bGotAnyNull = false;
                foreach (var item in nodes)
                {
                    if (item != null)
                    {
                        item.DrawNode();
                    }
                    else
                    {
                        bGotAnyNull = true;
                    }
                }

                foreach (var item in nodes)
                {
                    if (item != null)
                    {
                        item.DrawConnections();
                    }
                }

                if(bGotAnyNull)
                {
                    nodes = System.Array.FindAll<Node>(nodes, (Node a_node) => { return a_node != null; });
                }

                m_rect = EditorGUILayout.BeginHorizontal("Box", GUILayout.Width(300));
                index = EditorGUILayout.Popup(index, m_nodeOptions.ToArray());
                m_nodeCreationBntRect.x = m_rect.x;
                m_nodeCreationBntRect.y = m_rect.height + 10;
                m_nodeCreationBntRect.height = 40;
                m_nodeCreationBntRect.width = 150;
                if (GUI.Button(m_nodeCreationBntRect, "Create Node"))
                {
                    CreateNode();
                }
                EditorGUILayout.EndHorizontal();

            }

            if (Event.current.type == EventType.MouseUp)
            {
                Node.ResetDragInfo();
            }

        }

        void Update()
        {
            if(nodes != null)
            {
                foreach (var item in nodes)
                {
                    if (item != null)
                    {
                        item.EditorUpdate();
                    }
                }
            }
            Node.WindowSize = new Vector2(position.width,position.height);
            Repaint();
        }


        public void FindAllDerivedTypes<T>()
        {
            FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }

        private void FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                if (t.IsClass && t.IsSubclassOf(derivedType) && !t.IsAbstract)
                {
                    m_nodeOptions.Add(t.Name);
                    m_nodeTypes.Add(t);
                }
            }
            //return m_objects;

        }
        private void CreateNode()
        {
            if(m_nodeTypes.Count == 0)
            {
                FindAllDerivedTypes<Node>();
            }
            // throw new NotImplementedException();
            Debug.Log("index: " + index + ", count: " + m_nodeTypes.Count);
            Type typeDefine = m_nodeTypes[index];
            GameObject node = new GameObject(typeDefine.Name, typeDefine);
            RefreshNodes();
        }
    }
}