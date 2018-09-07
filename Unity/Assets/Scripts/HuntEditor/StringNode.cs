using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SillyGames.TreasureHunt.HuntEditor;
using System;

namespace SillyGames.TreasureHunt.HuntEditor
{
    public struct StringStruct : IComparable
    {
        string m_strData;

        public int CompareTo(object obj)
        {
            return ((IComparable)m_strData).CompareTo(obj);
        }
    }

    public class StringNode : PrimNode<StringStruct>
    {
        [SerializeField]
        private StringNode m_ref = null;

        public override PrimNode<StringStruct> Ref
        {
            get
            {
                return m_ref;
            }

            set
            {
                m_ref = (StringNode)value;
            }
        }
    }
}
