using System.Runtime.InteropServices;

namespace Syroot.Worms.Worms2.GameServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 50)]
    internal struct SessionInfo
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        internal uint Unknown0;
        internal uint Unknown4;
        internal Nation Nation;
        internal byte GameVersion;
        internal byte GameRelease;
        internal SessionType Type;
        internal SessionAccess Access;
        internal byte Unknown13;
        internal byte Unknown14;

        internal byte Unused15;
        internal ulong Unused16;
        internal ulong Unused24;
        internal ulong Unused32;
        internal ulong Unused40;
        internal ushort Unused48;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        internal SessionInfo(Nation nation, SessionType type, SessionAccess access = SessionAccess.Public)
        {
            Unknown0 = 0x17171717;
            Unknown4 = 0x02010101;
            Nation = nation;
            GameVersion = 49;
            GameRelease = 49;
            Type = type;
            Access = access;
            Unknown13 = 1;
            Unknown14 = 0;

            Unused15 = default;
            Unused16 = default;
            Unused24 = default;
            Unused32 = default;
            Unused40 = default;
            Unused48 = default;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public override string ToString() => $"{Unknown0:X8}-{Unknown4:X8} {Nation} {GameVersion}/{GameRelease} "
            + $"{Type}/{Access}/{Unknown13:X2}/{Unknown14:X2} "
            + $"({Unused15}-{Unused16}-{Unused24}-{Unused24}-{Unused40}-{Unused48})";
    }

    internal enum SessionType : byte
    {
        Room = 1,
        Game = 4,
        User = 5
    }

    internal enum SessionAccess : byte
    {
        Public = 1,
        Protected = 2
    }
}
