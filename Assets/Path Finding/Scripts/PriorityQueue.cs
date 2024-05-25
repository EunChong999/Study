using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly SortedDictionary<float, Queue<T>> _sortedDictionary = new SortedDictionary<float, Queue<T>>();

    public int Count
    {
        get
        {
            int count = 0;
            foreach (var queue in _sortedDictionary.Values)
            {
                count += queue.Count;
            }
            return count;
        }
    }

    public void Enqueue(T item, float priority)
    {
        if (!_sortedDictionary.ContainsKey(priority))
        {
            _sortedDictionary[priority] = new Queue<T>();
        }
        _sortedDictionary[priority].Enqueue(item);
    }

    public T Dequeue()
    {
        if (_sortedDictionary.Count == 0)
        {
            throw new InvalidOperationException("Priority queue is empty");
        }

        using (var enumerator = _sortedDictionary.GetEnumerator())
        {
            enumerator.MoveNext();
            var queue = enumerator.Current.Value;
            var item = queue.Dequeue();
            if (queue.Count == 0)
            {
                _sortedDictionary.Remove(enumerator.Current.Key);
            }
            return item;
        }
    }

    public bool Contains(T item)
    {
        foreach (var queue in _sortedDictionary.Values)
        {
            if (queue.Contains(item))
            {
                return true;
            }
        }
        return false;
    }
}
