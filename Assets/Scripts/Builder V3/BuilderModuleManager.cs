using Builder2;
using System;
using System.Collections;
using System.Collections.Generic;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

public class BuilderModuleManager : MonoBehaviour
{
    private BuilderCellGrid cellGrid;

    private ModuleImage[,] moduleGrid;
    private Dictionary<ModuleImage, Vector2Int> moduleList;

    private VisualElement rootElement;
    private VisualElement placementGrid;

    private void Start()
    {
        cellGrid = GetComponent<BuilderCellGrid>();

        moduleGrid = new ModuleImage[cellGrid.gridWidth, cellGrid.gridHeight];
        moduleList = new Dictionary<ModuleImage, Vector2Int>();

        UIDocument document = GetComponent<UIDocument>();
        rootElement = document.rootVisualElement;
        placementGrid = rootElement.Q(name: "placements");
    }

    public bool AddModule(ModuleImage module, Vector2Int pos)
    {
        List<Vector2Int> moduleCollision = module.module.GetCollisionInfo(pos);
        if (module == null || !cellGrid.CellsAvailable(moduleCollision))
            return false;
        if(moduleList.ContainsKey(module))
        {
            Debug.LogWarning("Added module that was already present in grid. Removing module from previous location.");
            RemoveModule(module, false);
        }

        module.RemoveFromHierarchy();
        placementGrid.Add(module);
        Vector2 originOffset = (module.module.GridBounds.min+pos)*cellGrid.cellSize;
        module.transform.position = originOffset;

        foreach(Vector2Int cellPos in moduleCollision)
        {
            if (GetModuleAt(cellPos) != null)
            {
                Debug.LogWarning("Module collision error detected");
            }
            cellGrid.MarkOccupied(cellPos);
            moduleGrid[cellPos.x, cellPos.y] = module;
        }

        moduleList.Add(module, pos);

        return true;
    }

    public Vector2Int GetModuleLocation(ModuleImage module)
    {
        if (!moduleList.ContainsKey(module))
            return new Vector2Int(-1, -1);
        return moduleList[module];
    }

    public ModuleImage GetModuleAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= cellGrid.gridWidth)
            return null;
        if (pos.y < 0 || pos.y >= cellGrid.gridHeight)
            return null;
        return moduleGrid[pos.x, pos.y];
    }

    public IReadOnlyDictionary<ModuleImage, Vector2Int> GetModules()
    {
        return new Dictionary<ModuleImage, Vector2Int>(moduleList);
    }

    public Vector2Int RemoveModule(ModuleImage module, bool restoreToRootElement = true)
    {
        if (module == null || !moduleList.ContainsKey(module))
            return new Vector2Int(-1, -1);
        Vector2Int originPos = moduleList[module];
        List<Vector2Int> moduleCollision = module.module.GetCollisionInfo(originPos);
        foreach (Vector2Int cellPos in moduleCollision)
        {
            if(GetModuleAt(cellPos) != module)
            {
                Debug.LogWarning("Module collision error detected");
            }
            else
            {
                moduleGrid[cellPos.x, cellPos.y] = null;
                cellGrid.MarkUnOccupied(cellPos);
            }
        }
        moduleList.Remove(module);
        module.RemoveFromHierarchy();
        if (restoreToRootElement)
            rootElement.Add(module);
        return originPos;
    }

    public ModuleImage RemoveModuleAt(Vector2Int pos, out Vector2Int originPos, bool restoreToRootElement = true)
    {
        ModuleImage module = GetModuleAt(pos);
        originPos = RemoveModule(module, restoreToRootElement);
        return module;
    }
}
