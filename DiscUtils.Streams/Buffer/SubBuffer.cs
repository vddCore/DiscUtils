using System;
using System.Collections.Generic;

namespace DiscUtils.Streams.Buffer
{
    /// <summary>
    /// Class representing a portion of an existing buffer.
    /// </summary>
    public class SubBuffer : Buffer
    {
        private readonly long _first;
        private readonly long _length;

        private readonly IBuffer _parent;

        /// <summary>
        /// Initializes a new instance of the SubBuffer class.
        /// </summary>
        /// <param name="parent">The parent buffer.</param>
        /// <param name="first">The first byte in <paramref name="parent"/> represented by this sub-buffer.</param>
        /// <param name="length">The number of bytes of <paramref name="parent"/> represented by this sub-buffer.</param>
        public SubBuffer(IBuffer parent, long first, long length)
        {
            _parent = parent;
            _first = first;
            _length = length;

            if (_first + _length > _parent.Capacity)
            {
                throw new ArgumentException("Substream extends beyond end of parent stream");
            }
        }

        /// <summary>
        /// Can this buffer be read.
        /// </summary>
        public override bool CanRead => _parent.CanRead;

        /// <summary>
        /// Can this buffer be modified.
        /// </summary>
        public override bool CanWrite => _parent.CanWrite;

        /// <summary>
        /// Gets the current capacity of the buffer, in bytes.
        /// </summary>
        public override long Capacity => _length;

        /// <summary>
        /// Gets the parts of the buffer that are stored.
        /// </summary>
        /// <remarks>This may be an empty enumeration if all bytes are zero.</remarks>
        public override IEnumerable<StreamExtent> Extents => OffsetExtents(_parent.GetExtentsInRange(_first, _length));

        /// <summary>
        /// Flushes all data to the underlying storage.
        /// </summary>
        public override void Flush()
        {
            _parent.Flush();
        }

        /// <summary>
        /// Reads from the buffer into a byte array.
        /// </summary>
        /// <param name="pos">The offset within the buffer to start reading.</param>
        /// <param name="buffer">The destination byte array.</param>
        /// <param name="offset">The start offset within the destination buffer.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public override int Read(long pos, byte[] buffer, int offset, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Attempt to read negative bytes");
            }

            if (pos >= _length)
            {
                return 0;
            }

            return _parent.Read(pos + _first, buffer, offset,
                (int)Math.Min(count, Math.Min(_length - pos, int.MaxValue)));
        }

        /// <summary>
        /// Writes a byte array into the buffer.
        /// </summary>
        /// <param name="pos">The start offset within the buffer.</param>
        /// <param name="buffer">The source byte array.</param>
        /// <param name="offset">The start offset within the source byte array.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(long pos, byte[] buffer, int offset, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Attempt to write negative bytes");
            }

            if (pos + count > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Attempt to write beyond end of substream");
            }

            _parent.Write(pos + _first, buffer, offset, count);
        }

        /// <summary>
        /// Sets the capacity of the buffer, truncating if appropriate.
        /// </summary>
        /// <param name="value">The desired capacity of the buffer.</param>
        public override void SetCapacity(long value)
        {
            throw new NotSupportedException("Attempt to change length of a subbuffer");
        }

        /// <summary>
        /// Gets the parts of a buffer that are stored, within a specified range.
        /// </summary>
        /// <param name="start">The offset of the first byte of interest.</param>
        /// <param name="count">The number of bytes of interest.</param>
        /// <returns>An enumeration of stream extents, indicating stored bytes.</returns>
        public override IEnumerable<StreamExtent> GetExtentsInRange(long start, long count)
        {
            long absStart = _first + start;
            long absEnd = Math.Min(absStart + count, _first + _length);
            return OffsetExtents(_parent.GetExtentsInRange(absStart, absEnd - absStart));
        }

        private IEnumerable<StreamExtent> OffsetExtents(IEnumerable<StreamExtent> src)
        {
            foreach (StreamExtent e in src)
            {
                yield return new StreamExtent(e.Start - _first, e.Length);
            }
        }
    }
}