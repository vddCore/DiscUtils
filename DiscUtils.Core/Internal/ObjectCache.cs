using System;
using System.Collections.Generic;

namespace DiscUtils.Core.Internal
{
    /// <summary>
    /// Caches objects.
    /// </summary>
    /// <typeparam name="K">The type of the object key.</typeparam>
    /// <typeparam name="V">The type of the objects to cache.</typeparam>
    /// <remarks>
    /// Can be use for two purposes - to ensure there is only one instance of a given object,
    /// and to prevent the need to recreate objects that are expensive to create.
    /// </remarks>
    internal class ObjectCache<K, V>
    {
        private const int MostRecentListSize = 20;
        private const int PruneGap = 500;

        private readonly Dictionary<K, WeakReference> _entries;
        private int _nextPruneCount;
        private readonly List<KeyValuePair<K, V>> _recent;

        public ObjectCache()
        {
            _entries = new Dictionary<K, WeakReference>();
            _recent = new List<KeyValuePair<K, V>>();
        }

        public V this[K key]
        {
            get
            {
                for (int i = 0; i < _recent.Count; ++i)
                {
                    KeyValuePair<K, V> recentEntry = _recent[i];
                    if (recentEntry.Key.Equals(key))
                    {
                        MakeMostRecent(i);
                        return recentEntry.Value;
                    }
                }

                WeakReference wRef;
                if (_entries.TryGetValue(key, out wRef))
                {
                    V val = (V)wRef.Target;
                    if (val != null)
                    {
                        MakeMostRecent(key, val);
                    }

                    return val;
                }

                return default(V);
            }

            set
            {
                _entries[key] = new WeakReference(value);
                MakeMostRecent(key, value);
                PruneEntries();
            }
        }

        internal void Remove(K key)
        {
            for (int i = 0; i < _recent.Count; ++i)
            {
                if (_recent[i].Key.Equals(key))
                {
                    _recent.RemoveAt(i);
                    break;
                }
            }

            _entries.Remove(key);
        }

        private void PruneEntries()
        {
            _nextPruneCount++;

            if (_nextPruneCount > PruneGap)
            {
                List<K> toPrune = new List<K>();
                foreach (KeyValuePair<K, WeakReference> entry in _entries)
                {
                    if (!entry.Value.IsAlive)
                    {
                        toPrune.Add(entry.Key);
                    }
                }

                foreach (K key in toPrune)
                {
                    _entries.Remove(key);
                }

                _nextPruneCount = 0;
            }
        }

        private void MakeMostRecent(int i)
        {
            if (i == 0)
            {
                return;
            }

            KeyValuePair<K, V> entry = _recent[i];
            _recent.RemoveAt(i);
            _recent.Insert(0, entry);
        }

        private void MakeMostRecent(K key, V val)
        {
            while (_recent.Count >= MostRecentListSize)
            {
                _recent.RemoveAt(_recent.Count - 1);
            }

            _recent.Insert(0, new KeyValuePair<K, V>(key, val));
        }
    }
}