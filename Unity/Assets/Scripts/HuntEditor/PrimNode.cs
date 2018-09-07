using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

namespace SillyGames.TreasureHunt.HuntEditor
{
    /// <summary>
    /// since the primitive nodes must have a value and should not be null,hence the struct constrain
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PrimNode<T> : Node where T : struct, IComparable
    {
        [SerializeField]
        private bool m_bIsReference = false;
        public bool IsReference
        {
            get
            {
                return m_bIsReference;
            }
            set
            {
                m_bIsReference = value;
            }
        }

        [SerializeField]
        private T m_value = default(T);

        //[SerializeField]
        //private PrimNode<T> m_ref = null;

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
            set
            {
                m_value = value;
            }
        }

        public T? RefValue
        {
            get
            {
                return (Ref!= null)? (T?)Ref.Value:null;
            }
        }

        public abstract PrimNode<T> Ref
        {
            get; set;
        }

        [SerializeField]
        private int m_refInstanceID = 0;


        private Vector2 m_vecScrollPos = Vector2.zero;
        protected override sealed void OnDrawNode()
        {
            
            DrawHook();
            if(IsReference)
            {
                DrawRing();
            }
            if(DrawSelect())
            {
                Selection.activeObject = gameObject;
            }
            if(DrawDelete())
            {
                DestroyImmediate(gameObject);
                return;
            }
            IsReference = GUILayout.Toggle(IsReference, IsReference? "Reference Type": "Value Type",GUI.skin.button);
            //if(!IsReference)
            {
                m_vecScrollPos = GUILayout.BeginScrollView(m_vecScrollPos);
                GUILayout.BeginVertical();                
                DrawPrimNode();
               
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
        }

        protected virtual void DrawPrimNode() { }
        protected override void OnNodeDropped(Node a_node)
        {
            if(a_node.GetType() == GetType())
            {
                Ref = a_node as PrimNode<T>;
            }
        }



        public override sealed void DrawConnections()
        {
            base.DrawConnections();
            if (IsReference)
            {
                if (Ref != null)
                {
                    DrawLine(RingPoint,Ref.HookPoint);
                }
            }
        }

        protected override void OnUnlicked(Node a_linkedNode)
        {
            base.OnUnlicked(a_linkedNode);
            Ref = null;
        }

    }

}
