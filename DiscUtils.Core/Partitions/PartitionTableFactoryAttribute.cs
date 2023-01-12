using System;

namespace DiscUtils.Core.Partitions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class PartitionTableFactoryAttribute : Attribute {}
}