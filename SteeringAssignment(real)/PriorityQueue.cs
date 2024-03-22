namespace SteeringAssignment_real
{
    using System;
    using System.Collections.Generic;
    
    public class PriorityQueue<T> // position of cell, priority
    {
        private SortedList<double, Queue<T>> _queue;

        public PriorityQueue()
        {
            _queue = new SortedList<double, Queue<T>>();
        }

        public void Enqueue(T item, double priority)
        {
            if (!_queue.ContainsKey(priority))
            {
                _queue[priority] = new Queue<T>();
            }
            _queue[priority].Enqueue(item);
        }

        public T Dequeue()
        {
            if (_queue.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            var first = _queue.Keys[0];
            var item = _queue[first].Dequeue();
            if (_queue[first].Count == 0)
            {
                _queue.RemoveAt(0);
            }
            return item;
        }

        public bool Contains(T item)
        {
            foreach (var queue in _queue.Values)
            {
                if (queue.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEmpty => _queue.Count == 0;
    }

}
