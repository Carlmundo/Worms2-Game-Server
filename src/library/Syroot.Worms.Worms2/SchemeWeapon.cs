using System;
using System.Runtime.InteropServices;

namespace Syroot.Worms.Worms2
{
    /// <summary>
    /// Represents the configuration of a weapon. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SchemeWeapon : IEquatable<SchemeWeapon>
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        /// <summary>Amount of this weapon with which a team is equipped at game start. 10 and negative values represent
        /// infinity.</summary>
        public int Ammo;
        /// <summary>Number of turns required to be taken by each team before this weapon becomes available.</summary>
        public int Delay;
        /// <summary>Retreat time after using this weapon. 0 uses the setting from the game options.</summary>
        public int RetreatTime;
        /// <summary><see langword="true"/> to preselect this weapon in the next turn; otherwise <see langword="false"/>.</summary>
        public bool Remember;
        public int Unused1;
        /// <summary>Amount of this weapon added to the team armory when collected from a crate.</summary>
        public int CrateAmmo;
        /// <summary>Amount of bullets shot at once.</summary>
        public int BulletCount;
        /// <summary>Percentual chance of this weapon to appear in crates.</summary>
        public int Probability;
        /// <summary>Damage measured in health points which also determines the blast radius.</summary>
        public int Damage;
        /// <summary>Pushing power measured in percent.</summary>
        public int ExplosionPower;
        /// <summary>Offset to the bottom of an explosion, measured in percent.</summary>
        public int ExplosionBias;
        /// <summary>Milliseconds required before this weapon starts flying towards its target.</summary>
        public int HomingDelay;
        /// <summary>Length in milliseconds this weapon flies towards its target before giving up.</summary>
        public int HomingTime;
        /// <summary>Percentual amount this weaopn is affected by wind.</summary>
        public int WindResponse;
        public int Unused2;
        /// <summary>Number of clusters into which this weapon explodes.</summary>
        public int ClusterCount;
        /// <summary>Speed in which clusters are dispersed in percent.</summary>
        public int ClusterLaunchPower;
        /// <summary>Angle in which clusters are dispersed in degrees.</summary>
        public int ClusterLaunchAngle;
        /// <summary>Damage of clusters measured in health points which also determines the blast radius.</summary>
        public int ClusterDamage;
        /// <summary>Overrides the fuse of this weapon, 0 for default.</summary>
        public int Fuse;
        /// <summary>Amount of fire created.</summary>
        public int FireAmount;
        /// <summary>Speed in which fire spreads, measured in percent.</summary>
        public int FireSpreadSpeed;
        /// <summary>Period in which fire burns, measured in percent.</summary>
        public int FireTime;
        /// <summary>Melee impact force in percent.</summary>
        public int MeleeForce;
        /// <summary>Melee impact angle in degrees.</summary>
        public int MeleeAngle;
        /// <summary>Melee damage in health points.</summary>
        public int MeleeDamage;
        /// <summary>Height of the fire punch jump, measured in percent.</summary>
        public int FirepunchHeight;
        /// <summary>Damage a dragon ball causes, measured in health points.</summary>
        public int DragonballDamage;
        /// <summary>Power in which a dragon ball launches hit worms, measured in percent.</summary>
        public int DragonballPower;
        /// <summary>Angle in which a dragon ball launches hit worms, measured in degrees.</summary>
        public int DragonballAngle;
        /// <summary>Lifetime of a launched dragon ball measured in milliseconds.</summary>
        public int DragonballTime;
        /// <summary>Length of digging measured in milliseconds. Applies to Kamikaze and digging tools.</summary>
        public int DiggingTime;
        /// <summary>Amount of airstrike clusters thrown.</summary>
        public int StrikeClusterCount;
        /// <summary>Angle in which bullets are dispersed, measured in degrees.</summary>
        public int BulletSpreadAngle;

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns whether two <see cref="SchemeWeapon"/> instances are equal by value.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>Whether the instances are equal.</returns>
        public static bool operator ==(SchemeWeapon left, SchemeWeapon right) => left.Equals(right);

        /// <summary>
        /// Returns whether two <see cref="SchemeWeapon"/> instances are inequal by value.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>Whether the instances are inequal.</returns>
        public static bool operator !=(SchemeWeapon left, SchemeWeapon right) => !(left == right);

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is SchemeWeapon weapon && Equals(weapon);

        /// <inheritdoc/>
        public bool Equals(SchemeWeapon other)
            => Ammo == other.Ammo
            && Delay == other.Delay
            && RetreatTime == other.RetreatTime
            && Remember == other.Remember
            && Unused1 == other.Unused1
            && CrateAmmo == other.CrateAmmo
            && BulletCount == other.BulletCount
            && Probability == other.Probability
            && Damage == other.Damage
            && ExplosionPower == other.ExplosionPower
            && ExplosionBias == other.ExplosionBias
            && HomingDelay == other.HomingDelay
            && HomingTime == other.HomingTime
            && WindResponse == other.WindResponse
            && Unused2 == other.Unused2
            && ClusterCount == other.ClusterCount
            && ClusterLaunchPower == other.ClusterLaunchPower
            && ClusterLaunchAngle == other.ClusterLaunchAngle
            && ClusterDamage == other.ClusterDamage
            && Fuse == other.Fuse
            && FireAmount == other.FireAmount
            && FireSpreadSpeed == other.FireSpreadSpeed
            && FireTime == other.FireTime
            && MeleeForce == other.MeleeForce
            && MeleeAngle == other.MeleeAngle
            && MeleeDamage == other.MeleeDamage
            && FirepunchHeight == other.FirepunchHeight
            && DragonballDamage == other.DragonballDamage
            && DragonballPower == other.DragonballPower
            && DragonballAngle == other.DragonballAngle
            && DragonballTime == other.DragonballTime
            && DiggingTime == other.DiggingTime
            && StrikeClusterCount == other.StrikeClusterCount
            && BulletSpreadAngle == other.BulletSpreadAngle;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Ammo);
            hash.Add(Delay);
            hash.Add(RetreatTime);
            hash.Add(Remember);
            hash.Add(Unused1);
            hash.Add(CrateAmmo);
            hash.Add(BulletCount);
            hash.Add(Probability);
            hash.Add(Damage);
            hash.Add(ExplosionPower);
            hash.Add(ExplosionBias);
            hash.Add(HomingDelay);
            hash.Add(HomingTime);
            hash.Add(WindResponse);
            hash.Add(Unused2);
            hash.Add(ClusterCount);
            hash.Add(ClusterLaunchPower);
            hash.Add(ClusterLaunchAngle);
            hash.Add(ClusterDamage);
            hash.Add(Fuse);
            hash.Add(FireAmount);
            hash.Add(FireSpreadSpeed);
            hash.Add(FireTime);
            hash.Add(MeleeForce);
            hash.Add(MeleeAngle);
            hash.Add(MeleeDamage);
            hash.Add(FirepunchHeight);
            hash.Add(DragonballDamage);
            hash.Add(DragonballPower);
            hash.Add(DragonballAngle);
            hash.Add(DragonballTime);
            hash.Add(DiggingTime);
            hash.Add(StrikeClusterCount);
            hash.Add(BulletSpreadAngle);
            return hash.ToHashCode();
        }
    }
}
