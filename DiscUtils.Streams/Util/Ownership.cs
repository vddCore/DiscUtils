namespace DiscUtils.Streams.Util
{
    /// <summary>
    /// Enumeration used to indicate transfer of disposable objects.
    /// </summary>
    public enum Ownership
    {
        /// <summary>
        /// Indicates there is no transfer of ownership.
        /// </summary>
        None,

        /// <summary>
        /// Indicates ownership of the stream is transfered, the owner should dispose of the stream when appropriate.
        /// </summary>
        Dispose
    }
}