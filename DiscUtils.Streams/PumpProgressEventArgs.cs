using System;

namespace DiscUtils.Streams
{
    /// <summary>
    /// Event arguments indicating progress on pumping a stream.
    /// </summary>
    public class PumpProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the number of bytes read from <c>InputStream</c>.
        /// </summary>
        public long BytesRead { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes written to <c>OutputStream</c>.
        /// </summary>
        public long BytesWritten { get; set; }

        /// <summary>
        /// Gets or sets the absolute position in <c>OutputStream</c>.
        /// </summary>
        public long DestinationPosition { get; set; }

        /// <summary>
        /// Gets or sets the absolute position in <c>InputStream</c>.
        /// </summary>
        public long SourcePosition { get; set; }
    }
}