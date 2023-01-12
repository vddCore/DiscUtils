namespace DiscUtils.Core.Compression
{
    /// <summary>
    /// Possible results of attempting to compress data.
    /// </summary>
    /// <remarks>
    /// A compression routine <i>may</i> return <c>Compressed</c>, even if the data
    /// was 'all zeros' or increased in size.  The <c>AllZeros</c> and <c>Incompressible</c>
    /// values are for algorithms that include special detection for these cases.
    /// </remarks>
    public enum CompressionResult
    {
        /// <summary>
        /// The data compressed succesfully.
        /// </summary>
        Compressed,

        /// <summary>
        /// The data was all-zero's.
        /// </summary>
        AllZeros,

        /// <summary>
        /// The data was incompressible (could not fit into destination buffer).
        /// </summary>
        Incompressible
    }
}