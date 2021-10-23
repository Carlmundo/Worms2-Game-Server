namespace Syroot.Worms.Worms2
{
    /// <summary>
    /// Represents the method to determine the next turn's worm.
    /// </summary>
    public enum WormSelect : int
    {
        /// <summary>Worms are selected in the order in which they appear in the team.</summary>
        Sequential,
        /// <summary>Worms are selected randomly.</summary>
        Random,
        /// <summary>Worms are selected by a computed rating system.</summary>
        Intelligent,
        /// <summary>Worms are selected by the player.</summary>
        Manual
    }

    /// <summary>
    /// Represents the weapons in the game.
    /// </summary>
    public enum Weapon
    {
        /// <summary>The Bazooka weapon.</summary>
        Bazooka,
        /// <summary>The Homing Missile weapon.</summary>
        HomingMissile,
        /// <summary>The Grenade weapon.</summary>
        Grenade,
        /// <summary>The Cluster Bomb weapon.</summary>
        ClusterBomb,
        /// <summary>The Banana Bomb weapon.</summary>
        BananaBomb,
        /// <summary>The Holy Hand Grenade weapon.</summary>
        HolyHandGrenade,
        /// <summary>The Homing Cluster Bomb weapon.</summary>
        HomingClusterBomb,
        /// <summary>The Petrol Bomb weapon.</summary>
        PetrolBomb,
        /// <summary>The Shotgun weapon.</summary>
        Shotgun,
        /// <summary>The Handgun weapon.</summary>
        Handgun,
        /// <summary>The Uzi weapon.</summary>
        Uzi,
        /// <summary>The Minigun weapon.</summary>
        Minigun,
        /// <summary>The Firepunch weapon.</summary>
        Firepunch,
        /// <summary>The Dragonball weapon.</summary>
        Dragonball,
        /// <summary>The Kamikaze weapon.</summary>
        Kamikaze,
        /// <summary>The Dynamite weapon.</summary>
        Dynamite,
        /// <summary>The Mine weapon.</summary>
        Mine,
        /// <summary>The Ming Vase weapon.</summary>
        MingVase,
        /// <summary>The Airstrike weapon.</summary>
        Airstrike,
        /// <summary>The Homing Airstrike weapon.</summary>
        HomingAirstrike,
        /// <summary>The Napalm Strike weapon.</summary>
        NapalmStrike,
        /// <summary>The Mail Strike weapon.</summary>
        MailStrike,
        /// <summary>The Girder weapon.</summary>
        Girder,
        /// <summary>The Pneumatic Drill weapon.</summary>
        PneumaticDrill,
        /// <summary>The Baseball Bat weapon.</summary>
        BaseballBat,
        /// <summary>The Prod weapon.</summary>
        Prod,
        /// <summary>The Teleport weapon.</summary>
        Teleport,
        /// <summary>The Ninja Rope weapon.</summary>
        NinjaRope,
        /// <summary>The Bungee weapon.</summary>
        Bungee,
        /// <summary>The Parachute weapon.</summary>
        Parachute,
        /// <summary>The Sheep weapon.</summary>
        Sheep,
        /// <summary>The Mad Cow weapon.</summary>
        MadCow,
        /// <summary>The Old Woman weapon.</summary>
        OldWoman,
        /// <summary>The Mortar weapon.</summary>
        Mortar,
        /// <summary>The Blowtorch weapon.</summary>
        Blowtorch,
        /// <summary>The Homing Pigeon weapon.</summary>
        HomingPigeon,
        /// <summary>The Super Sheep weapon.</summary>
        SuperSheep,
        /// <summary>The Super Banana Bomb weapon.</summary>
        SuperBananaBomb
    }
}
