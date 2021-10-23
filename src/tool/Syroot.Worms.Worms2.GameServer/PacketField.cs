using System;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents a bitset determining which fields are available in a <see cref="Packet"/> instance.
    /// </summary>
    [Flags]
    internal enum PacketField : int
    {
        None,
        Value0 = 1 << 0,
        Value1 = 1 << 1,
        Value2 = 1 << 2,
        Value3 = 1 << 3,
        Value4 = 1 << 4,
        Value10 = 1 << 10,
        DataLength = 1 << 5,
        Data = 1 << 6,
        Error = 1 << 7,
        Name = 1 << 8,
        Session = 1 << 9,
        All = Int32.MaxValue
    }
}
