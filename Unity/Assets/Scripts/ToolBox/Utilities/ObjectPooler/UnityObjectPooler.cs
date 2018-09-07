using UnityEngine;
using System.Collections.Generic;

namespace SillyGames.SGBase.Utilities
{
    public static class UnityObjectPooler<T> where T : UnityEngine.Object
    {
        //private static List<T> s_lstObject = new List<T>();
        private static T s_key = null;

        /// <summary>
        /// gets or sets the base object, which is used to make clones when new objects are required.
        /// note: this object is never returned by RetrieveFromPool, its always preserved as a separate copy
        /// </summary>
        public static T BaseObject
        {
            get
            {
                if(s_key == null)
                {
                    return null;
                }

                return UnityObjectPooler<T, T>.GetBaseObject(s_key);
            }
            set
            {
                ///remove old data if setting new one
                if(s_key != null)
                {
                    UnityObjectPooler<T, T>.RemoveData(s_key);
                }
                if(value != null)
                {
                    s_key = Object.Instantiate<T>(value);
                    s_key.name = "UnityObjectPooler<" + typeof(T) + ">_key";
                    s_key.hideFlags |= HideFlags.HideInHierarchy;
                    UnityObjectPooler<T, T>.SetBaseObject(s_key,value);
                }
            }
        }

        /// <summary>
        /// tells the number of new objects created during the call <see cref="RetrieveFromPool"/>
        /// </summary>
        public static int TotalObjectsCreated
        {
            get
            {
                return UnityObjectPooler<T, T>.GetTotalObjectsCreated(s_key);
            }
        }
            
            

        /// <summary>
        /// tells how many objects are there in the pool currently
        /// </summary>
        public static int PoolObjectCount
        {
            get
            {
                return UnityObjectPooler<T, T>.GetPoolObjectCount(s_key);
            }
        }

        /// <summary>
        /// Allows number of object to be created beforehand
        /// </summary>
        /// <param name="a_iCount">number of objects that needs to be precreated</param>
        public static void PreCreate(int a_iCount)
        {
            //System.Diagnostics.Debug.Assert(BaseObject != null, "UnityObjectPooler: BaseObject not set for call 'PreCreate', type: " + typeof(T));

            UnityObjectPooler<T, T>.PreCreate(s_key,a_iCount);
        }

        public static T RetrieveFromPool()
        {
            //System.Diagnostics.Debug.Assert(BaseObject != null, "UnityObjectPooler: BaseObject not set for call 'RetrieveFromPool', type: " + typeof(T));
            return UnityObjectPooler<T, T>.RetrieveFromPool(s_key);
        }

        public static void ReturnToPool(T a_object)
        {
            //System.Diagnostics.Debug.Assert(a_object != null, "UnityObjectPooler: object was null, for the call 'ReturnToPool', object type: " + typeof(T));
            UnityObjectPooler<T, T>.ReturnToPool(s_key,a_object);
        }

        public static bool IsInPool(T a_object)
        {
            return UnityObjectPooler<T, T>.IsInPool(s_key,a_object);
        }
    }

    public static class UnityObjectPooler<T, K> where T : UnityEngine.Object
    {
        private class ObjectPoolEntry
        {
            public List<T> Objects = new List<T>();
            public T BaseObject { get; set; }
            public int TotalObjectsCreated { get; set; }
            public int PoolObjectCount { get { return Objects.Count; } }
            public void PreCreate(int a_iCount)
            {
                //System.Diagnostics.Debug.Assert(BaseObject != null, "UnityObjectPooler: BaseObject not set for call 'PreCreate', type: " + typeof(T));

                int iNumberofObjectsToCreate = a_iCount - Objects.Count;
                if (a_iCount > 0)
                {
                    for (int i = 0; i < iNumberofObjectsToCreate; i++)
                    {
                        Objects.Add(Create());
                    }
                }
            }

            public T RetrieveFromPool()
            {
                //System.Diagnostics.Debug.Assert(BaseObject != null, "UnityObjectPooler: BaseObject not set for call 'RetrieveFromPool', type: " + typeof(T));
                T objectToReturn = null;
                if (Objects.Count <= 0)
                {
                    //Debug.Log("UnityObjectPooler: Creating new object, type: " + typeof(T) + ", objects created so far during retrieval: " + TotalObjectsCreated);
                    TotalObjectsCreated++;
                    objectToReturn = Create();
                }
                else
                {
                    objectToReturn = Objects[Objects.Count - 1];
                    Objects.RemoveAt(Objects.Count - 1);
                }
                return objectToReturn;
            }
            public void ReturnToPool(T a_object)
            {
                //System.Diagnostics.Debug.Assert(a_object != null, "UnityObjectPooler: object was null, for the call 'ReturnToPool', object type: " + typeof(T));

                if (!Objects.Contains(a_object))
                {
                    Objects.Add(a_object);
                }
                else
                {
                    Debug.LogWarning("UnityObjectPooler: Object was already in the pool: " + a_object + ", ignoring it for now!!");
                }
            }
            public bool IsInPool(T a_object)
            {
                return Objects.Contains(a_object);
            }
            private T Create()
            {
                return UnityEngine.Object.Instantiate<T>(BaseObject);
            }
        }

        private static Dictionary<K, ObjectPoolEntry> s_dictPoolMap = new Dictionary<K, ObjectPoolEntry>();

        public static bool ContainsKey(K a_key)
        {
            return s_dictPoolMap.ContainsKey(a_key);
        }

        public static void SetBaseObject(K a_key, T a_data)
        {
            if (!s_dictPoolMap.ContainsKey(a_key))
            {
                s_dictPoolMap.Add(a_key, new ObjectPoolEntry());
            }
            s_dictPoolMap[a_key].BaseObject = a_data;
        }

        public static T GetBaseObject(K a_key)
        {
            T a_data = null;
            ObjectPoolEntry entry = null;
            s_dictPoolMap.TryGetValue(a_key, out entry);
            if(entry != null)
            {
                a_data = entry.BaseObject;
            }
            return a_data;
        }

        public static int GetTotalObjectsCreated(K a_key)
        {
            ObjectPoolEntry entry = null;
            if(s_dictPoolMap.TryGetValue(a_key,out entry))
            {
                return entry.TotalObjectsCreated;
            }
            return 0;            
        }

        public static int GetPoolObjectCount(K a_key)
        {
            ObjectPoolEntry entry = null;
            if (s_dictPoolMap.TryGetValue(a_key, out entry))
            {
                return entry.PoolObjectCount;
            }
            return 0; 
        }

        public static void PreCreate(K a_key, int a_iCount)
        {
            //System.Diagnostics.Debug.Assert(s_dictPoolMap.ContainsKey(a_key), "UnityObjectPooler: Did not find any data for the call 'PreCreate', Key: " + a_key);    
            s_dictPoolMap[a_key].PreCreate(a_iCount);
        }

        public static T RetrieveFromPool(K a_key)
        {
            //System.Diagnostics.Debug.Assert(s_dictPoolMap.ContainsKey(a_key), "UnityObjectPooler: Did not find any data for the call 'RetrieveFromPool', Key: " + a_key);
            //Debug.Log("Retrieving from Pool with a key: " + a_key);
            return s_dictPoolMap[a_key].RetrieveFromPool();
        }

        public static void ReturnToPool(K a_key, T a_data)
        {
            //System.Diagnostics.Debug.Assert(s_dictPoolMap.ContainsKey(a_key), "UnityObjectPooler: Did not find any data for the call 'RetrieveFromPool', Key: " + a_key);
            s_dictPoolMap[a_key].ReturnToPool(a_data);
        }
        
        public static bool IsInPool(K a_key, T a_data)
        {
            ObjectPoolEntry entry = null;
            if (s_dictPoolMap.TryGetValue(a_key, out entry))
            {
                return entry.IsInPool(a_data);
            }
            return false;
        }

        public static void RemoveData(K a_key)
        {
            if(s_dictPoolMap.ContainsKey(a_key))
            {
                s_dictPoolMap.Remove(a_key);
            }
        }
    }

}