using System;
using System.Collections.Generic;
using System.Reflection;

namespace Builder2
{
    public abstract class ModuleBase
    {
        public static readonly Dictionary<string, Func<ModuleBase>> ModuleTypes = new()
        {
            {"crossbow", () => new CrossbowTurret()},
            {"shield", () => new Shield()},
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
            // rotate Blocked clockwise 90 degrees
            var x = offsetX;
            var y = offsetY;

            switch (Rotation)
            {
                case 0:
                    break;
                case 1:
                    offsetX = -y;
                    offsetY = x;
                    break;
                case 2:
                    offsetX = -x;
                    offsetY = -y;
                    break;
                case 3:
                    offsetX = y;
                    offsetY = -x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (offsetX < 0) offsetX = Blocked.Length + offsetX;
            if (offsetY < 0) offsetY = Blocked[0].Length + offsetY;
            
            // not colliding with any part of the Blocked[][] grid
            // therefore, not blocked
            if (offsetX >= Blocked.Length || offsetX < 0) return false;
            if (offsetY >= Blocked[0].Length || offsetY < 0) return false;

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

    public class Shield : ModuleBase
    {
        public Shield() : base("shield")
        {
        }

        public override int OriginalWidth => 3;
        public override int OriginalHeight => 1;
        public override int Weight => 5;
        public override int Orcs => 2;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true}
        };
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