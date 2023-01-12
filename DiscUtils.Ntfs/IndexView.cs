using System;
using System.Collections.Generic;
using DiscUtils.Streams;

namespace DiscUtils.Ntfs
{
    internal class IndexView<K, D>
        where K : IByteArraySerializable, new()
        where D : IByteArraySerializable, new()
    {
        private readonly Index _index;

        public IndexView(Index index)
        {
            _index = index;
        }

        public int Count => _index.Count;

        public IEnumerable<KeyValuePair<K, D>> Entries
        {
            get
            {
                foreach (KeyValuePair<byte[], byte[]> entry in _index.Entries)
                {
                    yield return new KeyValuePair<K, D>(Convert<K>(entry.Key), Convert<D>(entry.Value));
                }
            }
        }

        public D this[K key]
        {
            get => Convert<D>(_index[Unconvert(key)]);

            set => _index[Unconvert(key)] = Unconvert(value);
        }

        public IEnumerable<KeyValuePair<K, D>> FindAll(IComparable<byte[]> query)
        {
            foreach (KeyValuePair<byte[], byte[]> entry in _index.FindAll(query))
            {
                yield return new KeyValuePair<K, D>(Convert<K>(entry.Key), Convert<D>(entry.Value));
            }
        }

        public KeyValuePair<K, D> FindFirst(IComparable<byte[]> query)
        {
            foreach (KeyValuePair<K, D> entry in FindAll(query))
            {
                return entry;
            }

            return default(KeyValuePair<K, D>);
        }

        public IEnumerable<KeyValuePair<K, D>> FindAll(IComparable<K> query)
        {
            foreach (KeyValuePair<byte[], byte[]> entry in _index.FindAll(new ComparableConverter(query)))
            {
                yield return new KeyValuePair<K, D>(Convert<K>(entry.Key), Convert<D>(entry.Value));
            }
        }

        public KeyValuePair<K, D> FindFirst(IComparable<K> query)
        {
            foreach (KeyValuePair<K, D> entry in FindAll(query))
            {
                return entry;
            }

            return default(KeyValuePair<K, D>);
        }

        public bool TryGetValue(K key, out D data)
        {
            byte[] value;
            if (_index.TryGetValue(Unconvert(key), out value))
            {
                data = Convert<D>(value);
                return true;
            }
            data = default(D);
            return false;
        }

        public bool ContainsKey(K key)
        {
            return _index.ContainsKey(Unconvert(key));
        }

        public void Remove(K key)
        {
            _index.Remove(Unconvert(key));
        }

        private static T Convert<T>(byte[] data)
            where T : IByteArraySerializable, new()
        {
            T result = new T();
            result.ReadFrom(data, 0);
            return result;
        }

        private static byte[] Unconvert<T>(T value)
            where T : IByteArraySerializable, new()
        {
            byte[] buffer = new byte[value.Size];
            value.WriteTo(buffer, 0);
            return buffer;
        }

        private class ComparableConverter : IComparable<byte[]>
        {
            private readonly IComparable<K> _wrapped;

            public ComparableConverter(IComparable<K> toWrap)
            {
                _wrapped = toWrap;
            }

            public int CompareTo(byte[] other)
            {
                return _wrapped.CompareTo(Convert<K>(other));
            }
        }
    }
}