using Builder2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleData
{
    public ModuleBase type;
    public Vector2Int position;

    public ModuleData(ModuleBase type, Vector2Int pos)
    {
        this.type = type;
        position = pos;
    }

    public override string ToString()
    {
        return "Module type " + type.DisplayType + " at postion " + position;
    }
}
