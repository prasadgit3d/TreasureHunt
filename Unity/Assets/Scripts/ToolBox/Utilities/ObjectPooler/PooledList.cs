using System;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.SGBase.Utilities
{
    [Serializable]
    class PooledList<T> : List<T>
    {
        public Func<T> Creator { get; set; }
        private List<T> m_pooledObjects = new List<T>();

        public new void Clear()
        {
            foreach (var item in this)
            {
                AddToPoolIfNotContains(item);
            }
            base.Clear();
        }

        public T Add()
        {
            if(m_pooledObjects.Count> 0)
            {
                var item = m_pooledObjects[m_pooledObjects.Count-1];
                m_pooledObjects.RemoveAt(m_pooledObjects.Count - 1);
                Add(item);
                return item;
            }
            else
            {
                var item = Creator();
                base.Add(item);
                return item;
            }
        }

        public new bool Remove(T item)
        {
            if(base.Remove(item))
            {
                AddToPoolIfNotContains(item);
            }
            return false;
        }

        public new int RemoveAll(Predicate<T> match)
        {
            int iItemsRemoved = 0;
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                if (match(item))
                {
                    RemoveAt(i);
                    --i;
                    ++iItemsRemoved;
                }
            }
            return iItemsRemoved;
        }
        
        public new void RemoveAt(int index)
        {
            var item = this[index];
            this.RemoveAt(index);
            AddToPoolIfNotContains(item);
        }
        
        public new void RemoveRange(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                RemoveAt(i + index);
            }
        }

        void AddToPoolIfNotContains(T a_item)
        {
            if (!m_pooledObjects.Contains(a_item))
            {
                m_pooledObjects.Add(a_item);
            }
        }

    }
}
