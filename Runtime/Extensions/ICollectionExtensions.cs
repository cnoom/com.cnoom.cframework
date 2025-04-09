using System;
using System.Collections.Generic;
using System.Linq;

namespace CnoomFrameWork.Extensions
{
    public static class ICollectionExtensions
    {
        /// <summary>
        ///     Fisher-Yates 随机排序
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        public static void ShuffleExtension<T>(this ICollection<T> self)
        {
            Random rand = new Random();
            T[] array = self.ToArray();
            int n = array.Length;
            for(var i = 0; i < n; i++)
            {
                int r = i + rand.Next(n - i);
                (array[r], array[i]) = (array[i], array[r]);
            }
            self.Clear();
            foreach (T item in array)
            {
                self.Add(item);
            }
        }
    }
}