using System;
using System.Collections.Generic;
using System.Linq;
using CnoomFrameWork.Core.CommonEnum;

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
        
        /// <summary>
        ///     插入排序
        /// </summary>
        /// <param name="self"></param>
        /// <param name="order"></param>
        /// <typeparam name="T"></typeparam>
        public static void SortByInsertionExtension<T>(this ICollection<T> self, SortOrder order = SortOrder.Ascending) where T : IComparable<T>
        {
            T[] array = self.ToArray();
            int n = array.Length;
            
            for (int i = 1; i < n; i++)
            {
                T current = array[i];
                int j = i - 1;
        
                bool shouldShift = order == SortOrder.Ascending 
                    ? array[j].CompareTo(current) > 0 
                    : array[j].CompareTo(current) < 0;
        
                while (j >= 0 && shouldShift)
                {
                    array[j + 1] = array[j];
                    j--;
                    if (j >= 0)
                    {
                        shouldShift = order == SortOrder.Ascending 
                            ? array[j].CompareTo(current) > 0 
                            : array[j].CompareTo(current) < 0;
                    }
                }
                array[j + 1] = current;
            }
        
            self.Clear();
            foreach (T item in array)
            {
                self.Add(item);
            }
        }
    }
}