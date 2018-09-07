using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SillyGames.TreasureHunt.HuntEditor
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate | AttributeTargets.GenericParameter, Inherited = false)]
    public class SerializableAttribute : Attribute
    {
        public SerializableAttribute()
        {

        }

    }
}
