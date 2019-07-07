using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SillyGames.TreasureHunt.HuntEditor
{

    [Serializable]
    public class HuntEvent
    {
        [SerializeField]
        private string m_eventName = string.Empty;
        [SerializeField]
        private bool m_locked = false;

        public bool IsLocked { get { return m_locked; }set { m_locked = value; } }

        public HuntEventNode ParentNode { get; set; }
    }
    public class HuntEventNode:Node<HuntEvent>
    {       

        private void Start()
        {
            Value.ParentNode = this;
        }

        public bool IsLocked
        {
            get
            {
                return Value.IsLocked;
            }
            set
            {
                Value.IsLocked = value;
            }
        }


    }

    
        
}