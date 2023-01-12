using System;

namespace DiscUtils.Core.Internal
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class VirtualDiskFactoryAttribute : Attribute
    {
        public VirtualDiskFactoryAttribute(string type, string fileExtensions)
        {
            Type = type;
            FileExtensions = fileExtensions.Replace(".", string.Empty).Split(',');
        }

        public string[] FileExtensions { get; }

        public string Type { get; }
    }
}