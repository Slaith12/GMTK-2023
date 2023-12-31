using System;
using System.Collections.Generic;
using modules;
using UnityEngine;

namespace Builder2
{
    public abstract class ModuleBase
    {
        public static readonly IReadOnlyDictionary<string, ModuleBase> ModuleTypes = new Dictionary<string, ModuleBase>()
        {
            {"crossbow", new CrossbowTurret()},
            {"autobow", new AutobowTurret()},
            {"shield", new ShieldDown()},
            {"shield-up", new ShieldUp()},
            {"shield-down", new ShieldDown()},
            {"shield-left", new ShieldLeft()},
            {"shield-right", new ShieldRight()},
            {"cockpit", new Cockpit()},
            {"orc-attack-party", new OrcAttackParty0()},
            {"orc-attack-party-0", new OrcAttackParty0()},
            {"orc-attack-party-90", new OrcAttackParty90()},
            {"orc-attack-party-180", new OrcAttackParty180()},
            {"orc-attack-party-270", new OrcAttackParty270()},
            {"small-motor", new SmallMotor()}
        };

        protected ModuleBase(string id)
        {
            ModuleID = id;
        }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int Orcs { get; }
        public abstract Action<Sieger> ModuleEffect { get; }

        // y is vertical, x is horizontal, apparently
        protected abstract bool[][] Blocked { get; }

        protected virtual Vector2Int[] GridCollision => new Vector2Int[] { Vector2Int.zero };
        public virtual RectInt GridBounds => new RectInt(0, 0, 1, 1);
        public virtual CellCategory PlacementType => CellCategory.All;

        public string ModuleID { get; }
        public abstract string ModuleType { get; }

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

        public List<Vector2Int> GetCollisionInfo(Vector2Int origin)
        {
            List<Vector2Int> cellPositions = new List<Vector2Int>(GridCollision);
            string poses = "";
            for (int i = 0; i < cellPositions.Count; i++)
            {
                cellPositions[i] += origin;
                poses += cellPositions[i] + ", ";
            }
            return cellPositions;
        }
    }

    public class AutobowTurret : ModuleBase
    {
        public AutobowTurret() : base("autobow")
        {
        }

        public override int Width => 1;
        public override int Height => 1;
        public override int Orcs => 2;
        public override Action<Sieger> ModuleEffect => sieger => sieger.GetComponent<Autobow>().numBows++;
        public override string ModuleType => "Autobow";

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

        public override int Width => 1;
        public override int Height => 1;
        public override int Orcs => 1;
        public override Action<Sieger> ModuleEffect => sieger => sieger.GetComponent<Crossbow>().timers.Add(0);
        public override string ModuleType => "Crossbow";

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

        public override int Orcs => 3;
        public override CellCategory PlacementType => CellCategory.Edges;
        public override string ModuleType => "Shield";

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

        public override int Width => 3;
        public override int Height => 1;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(-1,0), new Vector2Int(1,0) };
        public override RectInt GridBounds => new RectInt(-1, 0, 3, 1);
    }

    public class ShieldDown : Shield
    {
        public ShieldDown() : base("shield-down")
        {
        }

        public override int Width => 3;
        public override int Height => 1;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0) };
        public override RectInt GridBounds => new RectInt(-1, 0, 3, 1);
    }

    public class ShieldLeft : Shield
    {
        public ShieldLeft() : base("shield-left")
        {
        }

        public override int Width => 1;
        public override int Height => 3;

        protected override bool[][] Blocked => new[]
        {
            new[] {true},
            new[] {true},
            new[] {true}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        public override RectInt GridBounds => new RectInt(0, -1, 1, 3);
    }


    public class ShieldRight : Shield
    {
        public ShieldRight() : base("shield-right")
        {
        }

        public override int Width => 1;
        public override int Height => 3;

        protected override bool[][] Blocked => new[]
        {
            new[] {true},
            new[] {true},
            new[] {true}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        public override RectInt GridBounds => new RectInt(0, -1, 1, 3);
    }

    public class Cockpit : ModuleBase
    {
        public Cockpit() : base("cockpit")
        {
        }

        public override int Width => 1;
        public override int Height => 1;
        public override int Orcs => 1;
        public override Action<Sieger> ModuleEffect => sieger => sieger.enabled = true;
        public override string ModuleType => "Cockpit";

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

        public override int Width => 3;
        public override int Height => 3;
        public override int Orcs => 3;
        public override Action<Sieger> ModuleEffect => sieger => sieger.attackOrcsAvailable += 3;
        public override string ModuleType => "OAP";

        public abstract int rotation { get; }
    }

    //middle piece = lower-left corner
    public class OrcAttackParty0 : OrcAttackParty
    {
        public OrcAttackParty0() : base("orc-attack-party-0")
        {
        }

        public override int rotation => 0;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, false, false},
            new[] {true, true, false},
            new[] {true, true, true}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[] 
        { Vector2Int.zero, -Vector2Int.up, -Vector2Int.up * 2, Vector2Int.right, Vector2Int.right * 2 };
        public override RectInt GridBounds => new RectInt(0, -2, 3, 3);
    }

    //middle piece = upper-left corner
    public class OrcAttackParty90 : OrcAttackParty
    {
        public OrcAttackParty90() : base("orc-attack-party-90")
        {
        }

        public override int rotation => 1;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true},
            new[] {true, true, false},
            new[] {true, false, false}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[]
        { Vector2Int.zero, -Vector2Int.down, -Vector2Int.down * 2, Vector2Int.right, Vector2Int.right * 2 };
        public override RectInt GridBounds => new RectInt(0, 0, 3, 3);
    }

    //middle piece = upper-right corner
    public class OrcAttackParty180 : OrcAttackParty
    {
        public OrcAttackParty180() : base("orc-attack-party-180")
        {
        }

        public override int rotation => 2;

        protected override bool[][] Blocked => new[]
        {
            new[] {true, true, true},
            new[] {false, true, true},
            new[] {false, false, true}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[]
        { Vector2Int.zero, -Vector2Int.down, -Vector2Int.down * 2, Vector2Int.left, Vector2Int.left * 2 };
        public override RectInt GridBounds => new RectInt(-2, 0, 3, 3);
    }

    //middle piece = lower-right corner
    public class OrcAttackParty270 : OrcAttackParty
    {
        public OrcAttackParty270() : base("orc-attack-party-270")
        {
        }

        public override int rotation => 3;

        protected override bool[][] Blocked => new[]
        {
            new[] {false, false, true},
            new[] {false, true, true},
            new[] {true, true, true}
        };
        protected override Vector2Int[] GridCollision => new Vector2Int[]
        { Vector2Int.zero, -Vector2Int.up, -Vector2Int.up * 2, Vector2Int.left, Vector2Int.left * 2 };
        public override RectInt GridBounds => new RectInt(-2, -2, 3, 3);
    }

    public class SmallMotor : ModuleBase
    {
        public SmallMotor() : base("small-motor")
        {
        }

        public override int Width => 1;
        public override int Height => 1;
        public override int Orcs => 3;
        public override string ModuleType => "Motor";

        public override Action<Sieger> ModuleEffect => (sieger => sieger.movementSpeed += 1f);
        protected override bool[][] Blocked => new[]
        {
            new[] {true}
        };
    }
}