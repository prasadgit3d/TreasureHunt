using System;
using System.Collections.Generic;

namespace SillyGames.SGBase
{
    public static class ArrayExtensions
    {
        public static int CountTrueFor<T>(this T[] a_array, Predicate<T> match)
        {
            int iCount = 0;
            foreach (var item in a_array)
                {
                    if (match(item))
                    {
                        ++iCount;
                    }
                }
            return iCount;
        }
    }
}
