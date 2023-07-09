﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Builder2
{
    public abstract class ModuleBase
    {
        public static readonly Dictionary<string, Func<ModuleBase>> ModuleTypes = new()
        {
            {"crossbow", () => new CrossbowTurret()},
            {"shield-up", () => new ShieldUp()},
            {"shield-down", () => new ShieldDown()},
            {"shield-left", () => new ShieldLeft()},
            {"shield-right", () => new ShieldRight()},
            {"cockpit", () => new Cockpit()},
        };

        protected ModuleBase(string displayType)
        {
            DisplayType = displayType;
        }

        public abstract int OriginalWidth { get; }
        public abstract int OriginalHeight { get; }
        public abstract int Weight { get; }
        public abstract int Orcs { get; }

        // y is vertical, x is horizontal, apparently
        protected abstract bool[][] Blocked { get; }

        public string DisplayType { get; }

        public int Rotation { get; set; }

        public int Width => Rotation % 2 == 0 ? OriginalWidth : OriginalHeight;

        public int Height => Rotation % 2 == 0 ? OriginalHeight : OriginalWidth;

        // ReSharper disable once InconsistentNaming
        public void RotateCW()
        {
            Rotation = (Rotation + 1) % 4;
        }

        // ReSharper disable once InconsistentNaming
        public void RotateCCW()
        {
            Rotation = (Rotation + 3) % 4;
        }

        public float GetRotationAngle()
        {
            return Rotation * 90f;
        }

        public bool IsBlocked(int offsetX, int offsetY)
        {
            // it is centered so move everything by half
            offsetX += (int)Math.Floor(Width / 2f);
            offsetY += (int)Math.Floor(Height / 2f);
            Debug.Log("query is " + offsetX + ", " + offsetY + " for " + DisplayType + " with rotation " + Rotation + " blocked? ");

            // not colliding with any part of the Blocked[][] grid
            // therefore, not blocked
            if (offsetY >= Blocked.Length || offsetX < 0) return false;
            if (offsetX >= Blocked[0].Length || offsetY < 0) return false;

            return Blocked[offsetY][offsetX];
        }
    }

    public class CrossbowTurret : ModuleBase
    {
        public CrossbowTurret() : base("crossbow")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 1;
        public override int Weight => 2;
        public override int Orcs => 1;

        protected override bool[][] Blocked => new[]
        {
            new[] {true}
        };
    }

    public abstract class Shield : ModuleBase
    {
        public override int Weight => 5;
        public override int Orcs => 2;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true}
        };

        protected Shield(string displayType) : base(displayType)
        {
        }
    }

    public class ShieldUp : Shield
    {
        public ShieldUp() : base("shield-up")
        {
        }

        public override int OriginalWidth => 3;
        public override int OriginalHeight => 1;
    }

    public class ShieldDown : Shield
    {
        public ShieldDown() : base("shield-down")
        {
        }

        public override int OriginalWidth => 3;
        public override int OriginalHeight => 1;
    }

    public class ShieldLeft : Shield
    {
        public ShieldLeft() : base("shield-left")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 3;
    }


    public class ShieldRight : Shield
    {
        public ShieldRight() : base("shield-right")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 3;
    }

    public class Cockpit : ModuleBase
    {
        public Cockpit() : base("cockpit")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 1;
        public override int Weight => 2;
        public override int Orcs => 1;

        protected override bool[][] Blocked => new[]
        {
            new[] {true}
        };
    }
}