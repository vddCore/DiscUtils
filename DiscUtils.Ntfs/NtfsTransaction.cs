using System;

namespace DiscUtils.Ntfs
{
    internal sealed class NtfsTransaction : IDisposable
    {
        [ThreadStatic]
        private static NtfsTransaction _instance;

        private readonly bool _ownRecord;

        public NtfsTransaction()
        {
            if (_instance == null)
            {
                _instance = this;
                Timestamp = DateTime.UtcNow;
                _ownRecord = true;
            }
        }

        public static NtfsTransaction Current => _instance;

        public DateTime Timestamp { get; }

        public void Dispose()
        {
            if (_ownRecord)
            {
                _instance = null;
            }
        }
    }
}