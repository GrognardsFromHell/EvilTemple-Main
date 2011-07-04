using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
    public sealed class TwoWayMap<TKey,TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _keyToValue;
        private readonly IDictionary<TValue, TKey> _valueToKey;

        public TwoWayMap()
        {
            _keyToValue = new Dictionary<TKey, TValue>();
            _valueToKey = new Dictionary<TValue, TKey>();
        }

        public void Insert(TKey key, TValue value)
        {
            _keyToValue.Add(key, value);
            _valueToKey.Add(value, key);
        }

        public TValue GetValue(TKey key)
        {
            return _keyToValue[key];
        }

        public TKey GetKey(TValue value)
        {
            return _valueToKey[value];
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value);
        }

        public void Clear()
        {
            _keyToValue.Clear();
            _valueToKey.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _keyToValue.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _keyToValue.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _keyToValue.Remove(item) && _valueToKey.Remove(item.Value);
        }

        public int Count
        {
            get { return _keyToValue.Count; }
        }

        public bool IsReadOnly
        {
            get { return _keyToValue.IsReadOnly; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _keyToValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(TKey key)
        {
            return _keyToValue.ContainsKey(key);
        }
        
        public void Add(TKey key, TValue value)
        {
            Insert(key, value);
        }

        public bool Remove(TKey key)
        {
            TValue value;
            if (!_keyToValue.TryGetValue(key, out value))
                return false;
            _keyToValue.Remove(key);
            _valueToKey.Remove(value);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _keyToValue.TryGetValue(key, out value);
        }

        public bool TryGetKey(TValue value, out TKey key)
        {
            return _valueToKey.TryGetValue(value, out key);
        }

        public TValue this[TKey key]
        {
            get { return _keyToValue[key]; }
            set { _keyToValue[key] = value; }
        }

        public ICollection<TKey> Keys
        {
            get { return _keyToValue.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _keyToValue.Values; }
        }
    }
}
