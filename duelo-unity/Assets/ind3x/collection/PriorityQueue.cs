namespace Ind3x.Collection
{
    using System.Collections.Generic;
    using System.Linq;

    /// <typeparam name="P">Priority value, i.e. a number</typeparam>
    /// <typeparam name="V">Value stored at priority</typeparam>
    public class PriorityQueue<P, V>
    {
        #region Private Fields
        /// <summary>
        /// For each priority queued in, the first added will be ready for dequeue.
        /// The dictionary does all the priority sorting
        /// </summary>
        private SortedDictionary<P, Queue<V>> _items = new SortedDictionary<P, Queue<V>>();
        #endregion

        #region Public Properties
        public bool IsEmpty
        {
            get { return !_items.Any(); }
        }
        #endregion

        #region Public Methods
        public void Enqueue(P priority, V value)
        {
            // If there isn't already a queue at this priority, create it first
            if (!_items.TryGetValue(priority, out Queue<V> q))
            {
                q = new Queue<V>();
                _items.Add(priority, q);
            }

            q.Enqueue(value);
        }

        public V Dequeue()
        {
            var pair = _items.First();
            var value = pair.Value.Dequeue();

            // remove the item if there are no more entries queued at that priority key
            if (pair.Value.Count == 0)
            {
                _items.Remove(pair.Key);
            }

            return value;
        }
        #endregion
    }
}