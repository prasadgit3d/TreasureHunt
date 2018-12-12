using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SillyGames.TreasureHunt.HuntEditor
{
    [Serializable]
    public class HuntAction 
    {
        public string m_actionName = string.Empty;
        public int m_huntID = 0;
    }

    public class HuntActionNode:Node<HuntAction>
    {
        public enum Type
        {
            None,
            SetBool,
            SetInt,
            SetFloat,
            SetString,
            StartTask,
            StopTask,
            RaiseEvent
        }
    }
}