using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SillyGames.TreasureHunt.HuntEditor
{
    [Serializable]
    public class HuntEvent 
    {
        public string m_actionName = string.Empty;
        public int ID = 0;
    }

    public class HuntEventNode:Node<HuntEvent>
    {

    }

    
        
}