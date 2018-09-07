using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SillyGames.SGBase.Localization;

namespace SillyGames.SGBase.Localization
{
    public class AscendingAlphabeticalSort<T, V> : BaseSorter<T, V> where V : LocalizableData<T>
    {
        public override List<V> SortGroups(List<V> toSort)
        {
            List<KeyValuePair<string, List<V>>> groups = GetGroups(toSort);
            groups.Sort(((KeyValuePair<string, List<V>> x, KeyValuePair<string, List<V>> y) => x.Key.CompareTo(y.Key)));
            List<V> toReturn = new List<V>();
            for (int i = 0; i < groups.Count; ++i)
            {
                toReturn.AddRange(groups[i].Value);
            }
            return toReturn;
        }

        public override List<V> SortKeys(List<V> toSort)
        {
            List<KeyValuePair<string, List<V>>> groups = GetGroups(toSort);
            List<V> toReturn = new List<V>();
            for (int i = 0; i < groups.Count; ++i)
            {
                groups[i].Value.Sort((V x, V y) => x.key.CompareTo(y.key));
                toReturn.AddRange(groups[i].Value);
            }
            return toReturn;
        }

        public override List<V> SortEntries(List<V> toSort)
        {
            List<KeyValuePair<string, List<V>>> groups = GetGroups(toSort);
            List<V> toReturn = new List<V>();
            for (int i = 0; i < groups.Count; ++i)
            {
                List<V> sorted = groups[i].Value;
                if (typeof(T).IsSubclassOf(typeof(Object)))
                {
                    sorted.Sort(((V x, V y) => ((Object)System.Convert.ChangeType(x.data, typeof(Object))).name.CompareTo(((Object)System.Convert.ChangeType(y.data, typeof(Object))).name)));
                }
                else
                {
                    sorted.Sort(((V x, V y) => (x.data.ToString().CompareTo(y.data.ToString()))));
                }
                toReturn.AddRange(sorted);
            }
            return toReturn;
        }
    }
}
