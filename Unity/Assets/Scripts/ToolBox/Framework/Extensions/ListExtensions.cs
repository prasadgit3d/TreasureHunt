using System.Collections.Generic;
using System.Collections;
using System;

namespace SillyGames.SGBase
{
    public static class ListExtensions
    {
        /// <summary>
        /// swaps the content of a list based on provided two indices
        /// </summary>
        /// <param name="a_list"></param>
        /// <param name="a_iA"></param>
        /// <param name="a_iB"></param>
        public static void Swap(this IList a_list, int a_iA, int a_iB)
        {
            var temp = a_list[a_iA];
            a_list[a_iA] = a_list[a_iB];
            a_list[a_iB] = temp;
        }

        public static int CountTrueFor<T>(this List<T> a_array, Predicate<T> match)
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