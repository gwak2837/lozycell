using UnityEngine;

public static class GameConfig
{
    public static class Player
    {
        public const float MoveSpeed = 5f;
        public const float MaxHealth = 100f;
        public const float BaseDamageMultiplier = 1f;
        public const float HitBoxSize = 0.5f;
        public const float SpeedPenaltyPerAminoAcid = 0.05f;
        public const float MinSpeedMultiplier = 0.5f;
    }

    public static class Enemy
    {
        public const float DefaultMoveSpeed = 1.5f;
        public const float DefaultDamage = 1f;
        public const float DefaultMaxHealth = 20f;
        public const float DefaultDefense = 2f;
        public const float DamageInterval = 1.0f;
        public const float StopDistance = 0.1f;
        public const float NucleobaseDropChance = 0.3f;
    }

    public static class Skills
    {
        // Projectile Skills
        public static class Ala // Multishot
        {
            public const float Damage = 10f;
            public const float Speed = 10f;
            public const int Count = 5;
            public const float Spread = 30f;
        }

        public static class Val // Power Shot
        {
            public const float Damage = 30f;
            public const float Speed = 15f;
            public const float Knockback = 5f;
        }

        public static class Tyr // Homing Missiles
        {
            public const float Damage = 15f;
            public const float Speed = 8f;
            public const int Count = 3;
            public const float Spread = 45f;
        }

        public static class Pro // Boomerang
        {
            public const float Damage = 15f;
            public const float Speed = 10f;
        }

        // Area Skills
        public static class Asp // Acid Pool
        {
            public const float EffectValue = 5f;
            public const float Duration = 5f;
            public const float Radius = 2f;
        }

        public static class Asn // Grass Knot
        {
            public const float EffectValue = 0f;
            public const float Duration = 5f;
            public const float Radius = 3f;
        }

        public static class Trp // Gravity Well
        {
            public const float EffectValue = 0f;
            public const float Duration = 5f;
            public const float Radius = 4f;
        }

        public static class Arg // Tesla Coil
        {
            public const float EffectValue = 10f;
            public const float Duration = 5f;
            public const float Radius = 3f;
        }

        public static class Met // Methyl Trail
        {
            public const float EffectValue = 10f;
            public const float Duration = 5f;
            public const float Radius = 1f;
        }

        // Buff Skills
        public static class Glu // Synaptic Boost
        {
            public const float Amount = 1.5f;
            public const float Duration = 10f;
        }

        public static class Leu // Muscle Up
        {
            public const float Amount = 1.5f;
            public const float Duration = 10f;
        }

        public static class Phe // Orbital Shield
        {
            public const float Amount = 1f;
            public const float Duration = 3f;
        }

        public static class Gln // Heal
        {
            public const float Amount = 33f;
        }

        // Global Skills
        public static class His // Anaphylaxis
        {
            public const float Value = 50f;
        }

        public static class Ser // Phospho Mark
        {
            public const float Duration = 10f;
        }

        public static class Thr // Alcohol Burn
        {
            public const float Value = 10f;
            public const float Duration = 5f;
        }

        public static class Gly // Synapse Shutdown
        {
            public const float Value = 0.5f;
            public const float Duration = 10f;
        }

        public static class Stop // Unlimited Void
        {
            public const float Duration = 5f;
        }

        // Chain Skills
        public static class Lys // Chain Lightning
        {
            public const float Damage = 20f;
            public const int MaxTargets = 4;
        }

        public static class Cys // S-S Death Bond
        {
            public const float Damage = 15f;
            public const int MaxTargets = 2;
        }

        // Summon Skills
        public static class Ile // Mirror Image
        {
            public const float Duration = 5f;
        }
    }

    public static class Spawner
    {
        // Nucleobase Spawner
        public const float NucleobaseSpawnRadius = 10f;
        public const float NucleobaseMinSpawnRadius = 4f;
        public const float NucleobaseSpawnInterval = 1f;
        public const int MaxNucleobaseCount = 50;

        // Enemy Spawner
        public const float EnemySpawnRadius = 12f;
        public const float EnemySpawnInterval = 2f;
    }

    public static class Pet
    {
        public const float ShootInterval = 1f;
        public const float Damage = 10f;
    }

    public static class Camera
    {
        public const float SmoothTime = 0.125f;
    }

    public static class Arcade
    {
        public const int TargetAminoAcids = 50;
    }

    public static class PeptideChain
    {
        public const float FollowSpeed = 10f;
        public const float NodeSpacing = 0.6f;
        public const float HeadOffset = 1.2f;
    }

    public static class Settings
    {
        public const string ShowVirtualControlsKey = "ShowVirtualControls";
    }

    public static class UI
    {
        public static class Joystick
        {
            public const float SizeCm = 2.0f;
            public const float HandleSizeCm = 0.8f;
        }

        public static class FloatingText
        {
            public const float Duration = 0.8f;
            public const float MoveSpeed = 1f;
            public const float RandomOffset = 0.3f;
            public const float NormalFontSize = 4f;
            public const float CriticalFontSize = 6f;
        }
    }
}
