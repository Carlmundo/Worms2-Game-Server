using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms.Worms2
{
    /// <summary>
    /// Represents scheme options stored in an OPT file which contains game settings.
    /// Used by W2. S. https://worms2d.info/Options_file.
    /// </summary>
    public class SchemeOptions : ILoadableFile, ISaveableFile
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "OPTFILE";

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializs a new instance of the <see cref="SchemeOptions"/> class.
        /// </summary>
        public SchemeOptions() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeOptions"/> class, loading the data from the given
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public SchemeOptions(Stream stream) => Load(stream);

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeOptions"/> class, loading the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public SchemeOptions(string fileName) => Load(fileName);

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the round time before sudden death is triggered in minutes.
        /// </summary>
        public int RoundTime { get; set; }

        /// <summary>
        /// Gets or sets the turn time in seconds available for the player to move.
        /// </summary>
        public int TurnTime { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds available for a worm to retreat after using a weapon which ends the turn
        /// while standing on land.
        /// </summary>
        public int RetreatTime { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds available for a worm to retreat after using a weapon which ends the turn
        /// while on a rope.
        /// </summary>
        public int RetreatTimeRope { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of objects (mines or oil drums) on the map.
        /// </summary>
        public int ObjectCount { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds a mine requires to explode. -1 is random between 0-3 seconds.
        /// </summary>
        public int MineDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mines can refuse to explode after their count down.
        /// </summary>
        public bool DudMines { get; set; }

        /// <summary>
        /// Gets or sets the influence power of the wind affecting weapons in percent.
        /// </summary>
        public int WindPower { get; set; }

        /// <summary>
        /// Gets or sets the friction deaccelerating objects touching solid ground between 0-5. 0 is default, 1 is low
        /// friction, 5 is high friction.
        /// </summary>
        public int Friction { get; set; }

        /// <summary>
        /// Gets or sets the number of kills which have to be done to replay the turn.
        /// </summary>
        public int ReplayRequiredKills { get; set; }

        /// <summary>
        /// Gets or sets the number of damage in health points which has to be done to replay the turn.
        /// </summary>
        public int ReplayRequiredDamage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether significant turns will be replayed in offline games.
        /// </summary>
        public bool AutomaticReplays { get; set; }

        /// <summary>
        /// Gets or sets the maximum fall damage applied in health points.
        /// </summary>
        public int FallDamage { get; set; }

        /// <summary>
        /// Gets or sets the number of rope swings allowed with one Ninja Rope.
        /// </summary>
        public int RopeSwings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the total round time until sudden death will be displayed in the
        /// turn timer.
        /// </summary>
        public bool ShowRoundTime { get; set; }

        /// <summary>
        /// Gets or sets the amount in pixels which the water will rise between turns after Sudden Death was triggered.
        /// </summary>
        public int WaterRiseRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the worms health is reduced to 1 when Sudden Death is triggered.
        /// </summary>
        public bool SuddenDeathHealthDrop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an indestructible border will be placed around the map.
        /// </summary>
        public bool IndestructibleBorder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the radius in which girders can be placed around the active worm is
        /// no longer unlimited.
        /// </summary>
        public bool RestrictGirders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the worm selection order determining the next worm to be played.
        /// </summary>
        public WormSelect WormSelectMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the chat box will be closed upon starting to move a worm or stays
        /// open.
        /// </summary>
        public bool ExtendedChatControls { get; set; }

        /// <summary>
        /// Gets or sets the delay in seconds between each team's turn to allow relaxed switching of seats.
        /// </summary>
        public int HotSeatDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether weapons collected in previous round will be carried over to the next
        /// round.
        /// </summary>
        public bool EnableStockpiling { get; set; }

        /// <summary>
        /// Gets or sets the percentual probability of a weapon or health crate to drop between turns.
        /// </summary>
        public int CrateProbability { get; set; }

        /// <summary>
        /// Gets or sets the percentual probability of a crate dropping closer to weak teams.
        /// </summary>
        public int CrateIntelligence { get; set; }

        /// <summary>
        /// Gets or sets the amount of health included in a health crate added to the collecting worm's energy.
        /// </summary>
        public int HealthCrateEnergy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether crates can explode upon trying to collect them.
        /// </summary>
        public bool BoobyTraps { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether super weapons can be collected from crates.
        /// </summary>
        public bool EnableSuperWeapons { get; set; }

        /// <summary>
        /// Gets or sets the initial worm energy at round start.
        /// </summary>
        public int WormEnergy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether worms cannot walk and are mostly stuck at their current position
        /// without using any utilities.
        /// </summary>
        public bool ArtilleryMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether worm selection is disabled after sudden death.
        /// </summary>
        public bool SuddenDeathDisableWormSelect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether oil drums will be placed instead of mines.
        /// </summary>
        public bool UseOilDrums { get; set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public void Load(Stream stream)
        {
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Read the header.
            if (reader.ReadString(_signature.Length) != _signature)
                throw new InvalidDataException("Invalid OPT file signature.");

            // Read the options.
            RoundTime = reader.ReadInt32();
            TurnTime = reader.ReadInt32();
            RetreatTime = reader.ReadInt32();
            RetreatTimeRope = reader.ReadInt32();
            ObjectCount = reader.ReadInt32();
            MineDelay = reader.ReadInt32();
            DudMines = reader.ReadBoolean(BooleanCoding.Dword);
            WindPower = reader.ReadInt32();
            Friction = reader.ReadInt32();
            ReplayRequiredKills = reader.ReadInt32();
            ReplayRequiredDamage = reader.ReadInt32();
            AutomaticReplays = reader.ReadBoolean(BooleanCoding.Dword);
            FallDamage = reader.ReadInt32();
            RopeSwings = reader.ReadInt32();
            ShowRoundTime = reader.ReadBoolean(BooleanCoding.Dword);
            WaterRiseRate = reader.ReadInt32();
            SuddenDeathHealthDrop = reader.ReadBoolean(BooleanCoding.Dword);
            IndestructibleBorder = reader.ReadBoolean(BooleanCoding.Dword);
            RestrictGirders = reader.ReadBoolean(BooleanCoding.Dword);
            WormSelectMode = reader.ReadEnum<WormSelect>(true);
            ExtendedChatControls = reader.ReadBoolean(BooleanCoding.Dword);
            HotSeatDelay = reader.ReadInt32();
            EnableStockpiling = reader.ReadBoolean(BooleanCoding.Dword);
            CrateProbability = reader.ReadInt32();
            CrateIntelligence = reader.ReadInt32();
            HealthCrateEnergy = reader.ReadInt32();
            BoobyTraps = reader.ReadBoolean(BooleanCoding.Dword);
            EnableSuperWeapons = reader.ReadBoolean(BooleanCoding.Dword);
            WormEnergy = reader.ReadInt32();
            ArtilleryMode = reader.ReadBoolean(BooleanCoding.Dword);
            SuddenDeathDisableWormSelect = reader.ReadBoolean(BooleanCoding.Dword);
            // The following option does not exist in all schemes.
            if (!reader.EndOfStream)
                UseOilDrums = reader.ReadBoolean(BooleanCoding.Dword);
        }

        /// <inheritdoc/>
        public void Load(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Load(stream);
        }

        /// <inheritdoc/>
        public void Save(Stream stream)
        {
            using BinaryStream writer = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Write the header.
            writer.Write(_signature, StringCoding.Raw);

            // Write the options.
            writer.Write(RoundTime);
            writer.Write(TurnTime);
            writer.Write(RetreatTime);
            writer.Write(RetreatTimeRope);
            writer.Write(ObjectCount);
            writer.Write(MineDelay);
            writer.Write(DudMines, BooleanCoding.Dword);
            writer.Write(WindPower);
            writer.Write(Friction);
            writer.Write(ReplayRequiredKills);
            writer.Write(ReplayRequiredDamage);
            writer.Write(AutomaticReplays, BooleanCoding.Dword);
            writer.Write(FallDamage);
            writer.Write(RopeSwings);
            writer.Write(ShowRoundTime, BooleanCoding.Dword);
            writer.Write(WaterRiseRate);
            writer.Write(SuddenDeathHealthDrop, BooleanCoding.Dword);
            writer.Write(IndestructibleBorder, BooleanCoding.Dword);
            writer.Write(RestrictGirders, BooleanCoding.Dword);
            writer.WriteEnum(WormSelectMode, true);
            writer.Write(ExtendedChatControls, BooleanCoding.Dword);
            writer.Write(HotSeatDelay);
            writer.Write(EnableStockpiling, BooleanCoding.Dword);
            writer.Write(CrateProbability);
            writer.Write(CrateIntelligence);
            writer.Write(HealthCrateEnergy);
            writer.Write(BoobyTraps, BooleanCoding.Dword);
            writer.Write(EnableSuperWeapons, BooleanCoding.Dword);
            writer.Write(WormEnergy);
            writer.Write(ArtilleryMode, BooleanCoding.Dword);
            writer.Write(SuddenDeathDisableWormSelect, BooleanCoding.Dword);
            writer.Write(UseOilDrums, BooleanCoding.Dword);
        }

        /// <inheritdoc/>
        public void Save(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
        }
    }
}
