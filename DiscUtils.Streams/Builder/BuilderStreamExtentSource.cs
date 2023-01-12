using System.IO;

namespace DiscUtils.Streams.Builder
{
    public class BuilderStreamExtentSource : BuilderExtentSource
    {
        private readonly Stream _stream;

        public BuilderStreamExtentSource(Stream stream)
        {
            _stream = stream;
        }

        public override BuilderExtent Fix(long pos)
        {
            return new BuilderStreamExtent(pos, _stream);
        }
    }
}