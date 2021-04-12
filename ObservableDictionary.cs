using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace dcsdbeditor
{
    public class ObservableDictionary : IDictionary<string, string>, INotifyCollectionChanged
    {
        private Dictionary<string, string> _internalDictionary;

        public ObservableDictionary()
        {
            _internalDictionary = new Dictionary<string, string>();
        }

        public string this[string key] 
        { 
            get => _internalDictionary[key]; 
            set => _internalDictionary[key] = value; 
        }

        public ICollection<string> Keys => _internalDictionary.Keys;

        public ICollection<string> Values => _internalDictionary.Values;

        public int Count => _internalDictionary.Count;

        public bool IsReadOnly => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Refresh()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(string key, string value)
        {
            _internalDictionary.Add(key, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _internalDictionary.Add(item.Key, item.Value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Clear()
        {
            _internalDictionary.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _internalDictionary.ContainsKey(item.Key) && _internalDictionary.ContainsValue(item.Value);
        }

        public bool ContainsKey(string key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary)_internalDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            var res = _internalDictionary.Remove(key);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return res;
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            var res = _internalDictionary.Remove(item.Key);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return res;
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            value = "";
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }
    }
}
