using UnityEditor;
using UnityEngine;

namespace SillyGames.TreasureHunt.HuntEditor
{
    public abstract class Node<T> : Node
    {
        [SerializeField]
        private T m_value = default(T);

        public T Value
        {
            get
            {
                if (!IsReference)
                {
                    return m_value;
                }
                else
                {
                    return Ref != null ? Ref.Value : default(T);
                }
            }
        }

        public T RefValue
        {
            get
            {
                return (Ref != null) ? (T)Ref.Value : default(T);
            }
        }

        public T LocalValue
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = value;
            }
        }
        protected new Node<T> Ref
        {
            get
            {
                return (Node<T>)base.Ref;
            }
            private set
            {
                base.Ref = value;
            }
        }

        private Vector2 m_vecScrollPos = Vector2.zero;
        SerializedObject serObj = null;

        protected override sealed void OnDrawNode()
        {

            base.OnDrawNode();
            //if(!IsReference)
            {
                m_vecScrollPos = GUILayout.BeginScrollView(m_vecScrollPos);
                GUILayout.BeginVertical();
                if (serObj == null)
                {
                    serObj = CustomEditor.serializedObject;
                }

                if(IsReference && Ref != null)
                {
                    var refNode = Ref;
                    while(refNode.IsReference && refNode.Ref != null)
                    {
                        refNode = refNode.Ref;
                    }
                    var m_valueProperty = refNode.CustomEditor.serializedObject.FindProperty("m_value");
                    if (m_valueProperty != null)
                    {
                        EditorGUILayout.PropertyField(m_valueProperty, true);
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
        }
    }
}