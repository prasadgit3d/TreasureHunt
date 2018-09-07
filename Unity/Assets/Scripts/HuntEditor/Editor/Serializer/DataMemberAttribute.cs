using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SillyGames.TreasureHunt.HuntEditor
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public class DataMemberAttribute : Attribute
    {
        public bool IsDictionary
        {
            get;
            set;

        }

        public Type KeyType { get; set; }
        public Type ValueType { get; set; }

        public ObjectType SerializationType { get; set; }

        public DataMemberAttribute()
        {
            Console.WriteLine("DataMemberAttribute");
        }

        public enum ObjectType
        {
            Object,
            Dictionary,
            List,
            Array
        }

        
    }
}