namespace DiscUtils.Streams.Buffer
{
    public interface IMappedBuffer : IBuffer
    {
        long MapPosition(long position);
    }
}