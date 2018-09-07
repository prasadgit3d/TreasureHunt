using UnityEngine;
using System.Collections;
using System;

namespace SillyGames.TreasureHunt.HuntEditor
{
    public class BooleanNode : PrimNode<bool> 
    {
        [SerializeField]
        private BooleanNode m_ref = null;

        public override PrimNode<bool> Ref
        {
            get
            {
                return m_ref;
            }

            set
            {
                m_ref = (BooleanNode)value;
            }
        }

        protected override void DrawPrimNode()
        {
            if(IsReference)
            {
                if(RefValue == null)
                {
                    GUILayout.Label("NULL");
                }
                else
                {
                    GUILayout.Label(RefValue.Value.ToString());
                }            
            }
            else
            {
                var value = Value;
                Value = GUILayout.Toggle(value, Value.ToString(), GUI.skin.button);
            }
            if (GUILayout.Button("print"))
            {
                Debug.Log(JsonUtility.ToJson(this));
            }
           
        }

    }

}
