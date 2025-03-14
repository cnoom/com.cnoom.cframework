using System;
using System.Collections.Generic;

namespace CnoomFrameWork.Modules.ActionModule
{
    // 优先队列（按时间排序）
    public class PriorityQueue<T>
    {
        private readonly Comparison<T> comparison;
        private readonly List<T> data;

        public PriorityQueue(Comparison<T> comparison)
        {
            data = new List<T>();
            this.comparison = comparison;
        }

        public int Count => data.Count;

        public void Enqueue(T item)
        {
            data.Add(item);
            int child = data.Count - 1;
            while (child > 0)
            {
                int parent = (child - 1) / 2;
                if(comparison(data[child], data[parent]) >= 0)
                    break;
                Swap(child, parent);
                child = parent;
            }
        }

        public T Dequeue()
        {
            int last = data.Count - 1;
            T front = data[0];
            data[0] = data[last];
            data.RemoveAt(last);

            last--;
            var parent = 0;
            while (true)
            {
                int left = parent * 2 + 1;
                if(left > last) break;
                int right = left + 1;
                int min = right <= last && comparison(data[right], data[left]) < 0 ? right : left;
                if(comparison(data[parent], data[min]) <= 0) break;
                Swap(parent, min);
                parent = min;
            }
            return front;
        }

        public T Peek()
        {
            return data[0];
        }

        private void Swap(int a, int b)
        {
            (data[a], data[b]) = (data[b], data[a]);
        }
    }
}