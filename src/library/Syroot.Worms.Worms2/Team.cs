using System;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms.Worms2
{
    /// <summary>
    /// Represents a team stored in a <see cref="TeamContainer"/> file.
    /// </summary>
    public class Team : ILoadable, ISaveable
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        public short Unknown1 { get; set; }

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the name of soundbank for the voice of team worms.
        /// </summary>
        public string SoundBankName { get; set; } = String.Empty;

        /// <summary>
        /// Gets the 8 worm names.
        /// </summary>
        public string[] WormNames { get; } = new string[8];

        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        public int Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public int Unknown9 { get; set; }
        public int Unknown10 { get; set; }
        public int Unknown11 { get; set; }
        public int Unknown12 { get; set; }
        public int Unknown13 { get; set; }
        public int Unknown14 { get; set; }
        public int Unknown15 { get; set; }
        public int Unknown16 { get; set; }
        public int Unknown17 { get; set; }
        public int Unknown18 { get; set; }
        public int Unknown19 { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown21 { get; set; }
        public int Unknown22 { get; set; }
        public int Unknown23 { get; set; }
        public int Unknown24 { get; set; }
        public int Unknown25 { get; set; }

        /// <summary>
        /// Gets or sets the number of games lost.
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        /// Gets or sets the number of games won.
        /// </summary>
        public int GamesWon { get; set; }

        public int Unknown26 { get; set; }
        public int Unknown27 { get; set; }

        /// <summary>
        /// Gets or sets the number of opponent worms killed by this team.
        /// </summary>
        public int Kills { get; set; }

        /// <summary>
        /// Gets or sets the number of worms which got killed in this team.
        /// </summary>
        public int Deaths { get; set; }

        /// <summary>
        /// Gets or sets the AI intelligence difficulty level, from 0-100, where 0 is human-controlled.
        /// </summary>
        public int CpuLevel { get; set; }

        public int Unknown28 { get; set; }
        public int Unknown29 { get; set; }
        public int Unknown30 { get; set; }

        /// <summary>
        /// Gets or sets the "difference" statistics value.
        /// </summary>
        public int Difference { get; set; }

        /// <summary>
        /// Gets or sets the number of games played, always being 0 for AI controlled teams.
        /// </summary>
        public int GamesPlayed { get; set; }

        /// <summary>
        /// Gets or sets the points gained by this team.
        /// </summary>
        public int Points { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public void Load(Stream stream)
        {
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);
            Unknown1 = reader.ReadInt16();
            Name = reader.ReadString(66);
            SoundBankName = reader.ReadString(36);
            for (int i = 0; i < WormNames.Length; i++)
                WormNames[i] = reader.ReadString(20);
            Unknown2 = reader.ReadInt32();
            Unknown3 = reader.ReadInt32();
            Unknown4 = reader.ReadInt32();
            Unknown5 = reader.ReadInt32();
            Unknown6 = reader.ReadInt32();
            Unknown7 = reader.ReadInt32();
            Unknown8 = reader.ReadInt32();
            Unknown9 = reader.ReadInt32();
            Unknown10 = reader.ReadInt32();
            Unknown11 = reader.ReadInt32();
            Unknown12 = reader.ReadInt32();
            Unknown13 = reader.ReadInt32();
            Unknown14 = reader.ReadInt32();
            Unknown15 = reader.ReadInt32();
            Unknown16 = reader.ReadInt32();
            Unknown17 = reader.ReadInt32();
            Unknown18 = reader.ReadInt32();
            Unknown19 = reader.ReadInt32();
            Unknown20 = reader.ReadInt32();
            Unknown21 = reader.ReadInt32();
            Unknown22 = reader.ReadInt32();
            Unknown23 = reader.ReadInt32();
            Unknown24 = reader.ReadInt32();
            Unknown25 = reader.ReadInt32();
            GamesLost = reader.ReadInt32();
            GamesWon = reader.ReadInt32();
            Unknown26 = reader.ReadInt32();
            Unknown27 = reader.ReadInt32();
            Kills = reader.ReadInt32();
            Deaths = reader.ReadInt32();
            CpuLevel = reader.ReadInt32();
            Unknown28 = reader.ReadInt32();
            Unknown29 = reader.ReadInt32();
            Unknown30 = reader.ReadInt32();
            Difference = reader.ReadInt32();
            GamesPlayed = reader.ReadInt32();
            Points = reader.ReadInt32();
        }

        /// <inheritdoc/>
        public void Save(Stream stream)
        {
            using BinaryStream writer = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);
            writer.Write(Unknown1);
            writer.WriteFixedString(Name, 66);
            writer.WriteFixedString(SoundBankName, 36);
            for (int i = 0; i < 8; i++)
                writer.WriteFixedString(WormNames[i], 20);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(Unknown4);
            writer.Write(Unknown5);
            writer.Write(Unknown6);
            writer.Write(Unknown7);
            writer.Write(Unknown8);
            writer.Write(Unknown9);
            writer.Write(Unknown10);
            writer.Write(Unknown11);
            writer.Write(Unknown12);
            writer.Write(Unknown13);
            writer.Write(Unknown14);
            writer.Write(Unknown15);
            writer.Write(Unknown16);
            writer.Write(Unknown17);
            writer.Write(Unknown18);
            writer.Write(Unknown19);
            writer.Write(Unknown20);
            writer.Write(Unknown21);
            writer.Write(Unknown22);
            writer.Write(Unknown23);
            writer.Write(Unknown24);
            writer.Write(Unknown25);
            writer.Write(GamesLost);
            writer.Write(GamesWon);
            writer.Write(Unknown26);
            writer.Write(Unknown27);
            writer.Write(Kills);
            writer.Write(Deaths);
            writer.Write(CpuLevel);
            writer.Write(Unknown28);
            writer.Write(Unknown29);
            writer.Write(Unknown30);
            writer.Write(Kills);
            writer.Write(Deaths);
            writer.Write(Difference);
            writer.Write(GamesPlayed);
            writer.Write(Points);
        }
    }
}
