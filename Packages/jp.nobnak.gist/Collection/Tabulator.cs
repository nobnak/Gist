using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Collection {

    public class Tabulator<Key, Value> : IDictionary<Key, Value> {

        protected Dictionary<Key, int> keyToIndex = new Dictionary<Key, int>();
        protected List<Key> keys = new List<Key>();
        protected List<Value> values = new List<Value>();

        public Value this[Key key] {
            get {
                int i;
                return keyToIndex.TryGetValue(key, out i) ? values[i] : default(Value);
            }
            set {
                int i;
                if (!keyToIndex.TryGetValue(key, out i)) {
                    Add(key, value);
                    return;
                }
                values[i] = value;
            }
        }

        #region interface
        #region IDictionary
        public ICollection<Key> Keys {
            get { return keys; }
        }
        public ICollection<Value> Values {
            get { return values; }
        }

        public int Count {
            get { return keyToIndex.Count; }
        }
        public bool IsReadOnly {
            get { return false; }
        }
        public void Clear() {
            keyToIndex.Clear();
            keys.Clear();
            values.Clear();
        }
        public IEnumerator GetEnumerator() {
            return GetEnumerator();
        }
        public void Add(Key key, Value value) {
            keyToIndex[key] = keys.Count;
            keys.Add(key);
            values.Add(value);
        }
        public bool ContainsKey(Key key) {
            return keyToIndex.ContainsKey(key);
        }
        public bool Remove(Key key) {
            int i;
            if (!keyToIndex.TryGetValue(key, out i))
                return false;
            keyToIndex.Remove(key);
            keys.RemoveAt(i);
            values.RemoveAt(i);
            return true;
        }
        public bool TryGetValue(Key key, out Value value) {
            int i;
            if (!keyToIndex.TryGetValue(key, out i)) {
                value = default(Value);
                return false;
            }
            value = values[i];
            return true;
        }
        public void Add(KeyValuePair<Key, Value> item) {
            Add(item.Key, item.Value);
        }
        public bool Contains(KeyValuePair<Key, Value> item) {
            return keyToIndex.ContainsKey(item.Key);
        }
        public bool Remove(KeyValuePair<Key, Value> item) {
            return Remove(item.Key);
        }
        IEnumerator<KeyValuePair<Key, Value>> IEnumerable<KeyValuePair<Key, Value>>.GetEnumerator() {
            for (var i = 0; i < keys.Count; i++)
                yield return new KeyValuePair<Key, Value>(keys[i], values[i]);
        }
        public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex) {
            throw new System.NotImplementedException();
        }
        #endregion

        public IList<Key> KeysAsList {
            get { return keys; }
        }
        public IList<Value> ValuesAsList {
            get { return values; }
        }
        #endregion
    }
}
