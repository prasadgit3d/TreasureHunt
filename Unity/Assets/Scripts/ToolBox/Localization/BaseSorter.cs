using System.Collections.Generic;
using SillyGames.SGBase.Localization;

namespace SillyGames.SGBase.Localization
{
    public interface ISorter<V>
    {
        List<KeyValuePair<string, List<V>>> GetGroups(List<V> toGetFrom);
        List<V> SortGroups(List<V> toSort);
        List<V> SortKeys(List<V> toSort);
        List<V> SortEntries(List<V> toSort);
    }

    public abstract class BaseSorter<T, V> : ISorter<V> where V : LocalizableData<T>
    {



        public abstract List<V> SortGroups(List<V> toSort);

        public abstract List<V> SortKeys(List<V> toSort);

        public abstract List<V> SortEntries(List<V> toSort);

        public List<KeyValuePair<string, List<V>>> GetGroups(List<V> toGetFrom)
        {
            List<KeyValuePair<string, List<V>>> groups = new List<KeyValuePair<string, List<V>>>();
            List<string> groupsNames = new List<string>();
            for (int i = 0; i < toGetFrom.Count; ++i)
            {
                if (!groupsNames.Contains(toGetFrom[i].group))
                {
                    string name = toGetFrom[i].group;
                    groupsNames.Add(name);
                    List<V> items = new List<V>();
                    for (int j = 0; j < toGetFrom.Count; ++j)
                    {
                        if (toGetFrom[j].group == toGetFrom[i].group)
                        {
                            items.Add(toGetFrom[j]);
                        }
                    }
                    KeyValuePair<string, List<V>> group = new KeyValuePair<string, List<V>>(name, items);
                    groups.Add(group);

                }
            }
            return groups;
        }
    }
}
