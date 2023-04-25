using System;
using System.Collections.Generic;

namespace MobX.Utilities.Collections
{
    public class DictionaryQueue<TKey, TValue>
    {
        private readonly Queue<TKey> _queue;
        private readonly Dictionary<TKey, TValue> _dictionary;

        public DictionaryQueue()
        {
            _queue = new Queue<TKey>();
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public void Clear()
        {
            _queue.Clear();
            _dictionary.Clear();
        }

        public void Enqueue(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the DictionaryQueue.");
            }

            _queue.Enqueue(key);
            _dictionary.Add(key, value);
        }

        public bool TryPeek(out TValue value)
        {
            if (_queue.Count == 0)
            {
                value = default(TValue);
                return false;
            }

            var key = _queue.Peek();
            value = _dictionary[key];
            return true;
        }

        public bool TryDequeue(out TValue value)
        {
            if (_queue.Count == 0)
            {
                value = default(TValue);
                return false;
            }

            var key = _queue.Dequeue();
            value = _dictionary[key];
            _dictionary.Remove(key);
            return true;
        }

        public TValue Dequeue()
        {
            if (_queue.Count == 0)
            {
                throw new InvalidOperationException("The DictionaryQueue is empty.");
            }

            var key = _queue.Dequeue();
            var value = _dictionary[key];
            _dictionary.Remove(key);
            return value;
        }

        public void UpdateElement(TKey key, TValue item)
        {
            if (!_dictionary.ContainsKey(key))
            {
                throw new KeyNotFoundException("The specified key does not exist in the DictionaryQueue.");
            }

            _dictionary[key] = item;
        }

        public int Count => _queue.Count;

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public void Update(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;
                return;
            }

            _queue.Enqueue(key);
            _dictionary.Add(key, value);
        }
    }
}