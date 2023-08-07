using System;
using System.Collections.Generic;
using modules;

namespace Builder2
{
    public abstract class ModuleBase
    {
        public static readonly Dictionary<string, Func<ModuleBase>> ModuleTypes = new()
        {
            {"crossbow", () => new CrossbowTurret()},
            {"autobow", () => new AutobowTurret()},
            {"shield-up", () => new ShieldUp()},
            {"shield-down", () => new ShieldDown()},
            {"shield-left", () => new ShieldLeft()},
            {"shield-right", () => new ShieldRight()},
            {"cockpit", () => new Cockpit()},
            {"orc-attack-party-0", () => new OrcAttackParty0()},
            {"orc-attack-party-90", () => new OrcAttackParty90()},
            {"orc-attack-party-180", () => new OrcAttackParty180()},
            {"orc-attack-party-270", () => new OrcAttackParty270()},
            {"small-motor", () => new SmallMotor()}
        };

        protected ModuleBase(string displayType)
        {
            DisplayType = displayType;
        }

        public abstract int OriginalWidth { get; }
        public abstract int OriginalHeight { get; }
        public abstract int Weight { get; }
        public abstract int Orcs { get; }
        public abstract Action<Sieger> ModuleEffect { get; }

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
            offsetX += (int) Math.Floor(Width / 2f);
            offsetY += (int) Math.Floor(Height / 2f);

            // not colliding with any part of the Blocked[][] grid
            // therefore, not blocked
            if (offsetY >= Blocked.Length || offsetX < 0) return false;
            if (offsetX >= Blocked[0].Length || offsetY < 0) return false;

            return Blocked[offsetY][offsetX];
        }
    }

    public class AutobowTurret : ModuleBase
    {
        public AutobowTurret() : base("autobow")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 1;
        public override int Weight => 4;
        public override int Orcs => 3;
        public override Action<Sieger> ModuleEffect => sieger => sieger.GetComponent<Autobow>().numBows++;

        protected override bool[][] Blocked => new[]
        {
            new[] {true}
        };
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
        public override Action<Sieger> ModuleEffect => sieger => sieger.GetComponent<Crossbow>().timers.Add(0);

        protected override bool[][] Blocked => new[]
        {
            new[] {true}
        };
    }

    public abstract class Shield : ModuleBase
    {
        protected Shield(string displayType) : base(displayType)
        {
        }

        public override int Weight => 5;
        public override int Orcs => 3;
        public override Action<Sieger> ModuleEffect => (sieger => { 
            HealthObject health = sieger.GetComponent<HealthObject>();
            health.maxHealth += 12;
        });
    }

    public class ShieldUp : Shield
    {
        public ShieldUp() : base("shield-up")
        {
        }

        public override int OriginalWidth => 3;
        public override int OriginalHeight => 1;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true}
        };
    }

    public class ShieldDown : Shield
    {
        public ShieldDown() : base("shield-down")
        {
        }

        public override int OriginalWidth => 3;
        public override int OriginalHeight => 1;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true}
        };
    }

    public class ShieldLeft : Shield
    {
        public ShieldLeft() : base("shield-left")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 3;

        protected override bool[][] Blocked => new[]
        {
            new[] {true},
            new[] {true},
            new[] {true}
        };
    }


    public class ShieldRight : Shield
    {
        public ShieldRight() : base("shield-right")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 3;

        protected override bool[][] Blocked => new[]
        {
            new[] {true},
            new[] {true},
            new[] {true}
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
        public override Action<Sieger> ModuleEffect => sieger => sieger.enabled = true;

        protected override bool[][] Blocked => new[]
        {
            new[] {true}
        };
    }

    public abstract class OrcAttackParty : ModuleBase
    {
        protected OrcAttackParty(string displayType) : base(displayType)
        {
        }

        public override int OriginalWidth => 3;
        public override int OriginalHeight => 3;
        public override int Weight => 7;
        public override int Orcs => 4;
        public override Action<Sieger> ModuleEffect => sieger => sieger.attackOrcsAvailable += 4;
    }

    public class OrcAttackParty0 : OrcAttackParty
    {
        public OrcAttackParty0() : base("orc-attack-party-0")
        {
        }

        protected override bool[][] Blocked => new[]
        {
            new[] {true, false, false},
            new[] {true, true, false},
            new[] {true, true, true}
        };
    }

    public class OrcAttackParty90 : OrcAttackParty
    {
        public OrcAttackParty90() : base("orc-attack-party-90")
        {
        }

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true},
            new[] {true, true, false},
            new[] {true, false, false}
        };
    }

    public class OrcAttackParty180 : OrcAttackParty
    {
        public OrcAttackParty180() : base("orc-attack-party-180")
        {
        }

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true},
            new[] {false, true, true},
            new[] {false, false, true}
        };
    }

    public class OrcAttackParty270 : OrcAttackParty
    {
        public OrcAttackParty270() : base("orc-attack-party-270")
        {
        }

        protected override bool[][] Blocked => new[]
        {
            new[] {false, false, true},
            new[] {false, true, true},
            new[] {true, true, true}
        };
    }

    public class SmallMotor : ModuleBase
    {
        public SmallMotor() : base("small-motor")
        {
        }

        public override int OriginalWidth => 1;
        public override int OriginalHeight => 1;
        public override int Weight => 7;
        public override int Orcs => 2;

        public override Action<Sieger> ModuleEffect => (sieger => sieger.movementSpeed += 0.5f);
        protected override bool[][] Blocked => new[]
        {
            new[] {true}
        };
    }
}