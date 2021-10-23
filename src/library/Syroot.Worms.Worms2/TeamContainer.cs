using System.Collections.Generic;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms.Worms2
{
    /// <summary>
    /// Represents the list of teams stored in ST1 files.
    /// Used by W2. S. https://worms2d.info/Worms_2_team_file.
    /// </summary>
    public class TeamContainer : ILoadableFile, ISaveableFile
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamContainer"/> class.
        /// </summary>
        public TeamContainer() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamContainer"/> class, loading the data from the given
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public TeamContainer(Stream stream) => Load(stream);

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamContainer"/> class, loading the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public TeamContainer(string fileName) => Load(fileName);

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the list of <see cref="Team"/> instances stored.
        /// </summary>
        public IList<Team> Teams { get; set; } = new List<Team>();

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public void Load(Stream stream)
        {
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);
            Teams = new List<Team>();
            while (!reader.EndOfStream)
                Teams.Add(reader.Load<Team>());
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
            foreach (Team team in Teams)
                team.Save(writer.BaseStream);
        }

        /// <inheritdoc/>
        public void Save(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
        }
    }
}
