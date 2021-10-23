namespace Syroot.Worms.Core
{
    /// <summary>
    /// Represents mathematical helper utilities.
    /// </summary>
    public static class Algebra
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the nearest, bigger <paramref name="multiple"/> of the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value whose nearest, bigger multiple will be returned of.</param>
        /// <param name="multiple">The multiple to return.</param>
        /// <returns>The nearest, bigger multiple.</returns>
        public static int NextMultiple(int value, int multiple) => (value + multiple - 1) / multiple * multiple;
    }
}
