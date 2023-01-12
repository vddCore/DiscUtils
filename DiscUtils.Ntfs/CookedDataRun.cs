using System;

namespace DiscUtils.Ntfs
{
    internal class CookedDataRun
    {
        public CookedDataRun(DataRun raw, long startVcn, long prevLcn, NonResidentAttributeRecord attributeExtent)
        {
            DataRun = raw;
            StartVcn = startVcn;
            StartLcn = prevLcn + raw.RunOffset;
            AttributeExtent = attributeExtent;

            if (startVcn < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startVcn), startVcn, "VCN must be >= 0");
            }

            if (StartLcn < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(prevLcn), prevLcn, "LCN must be >= 0");
            }
        }

        public NonResidentAttributeRecord AttributeExtent { get; }

        public DataRun DataRun { get; }

        public bool IsSparse => DataRun.IsSparse;

        public long Length
        {
            get => DataRun.RunLength;
            set => DataRun.RunLength = value;
        }

        public long StartLcn { get; set; }

        public long StartVcn { get; }
    }
}