using Builder2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BuilderCellGrid))]
public class BuilderModuleManager : MonoBehaviour
{
    private BuilderCellGrid cellGrid;

    private ModuleImage[,] moduleGrid;
    private Dictionary<ModuleImage, Vector2Int> moduleList;
    private int numOrcs;
    private bool hasCockpit;

    public VisualElement placementGrid { get; private set; }
    public Label orcDisplay { get; private set; }
    public Button siegeButton { get; private set; }

    private void Awake()
    {
        cellGrid = GetComponent<BuilderCellGrid>();
        numOrcs = 21;

        UIDocument document = GetComponent<UIDocument>();
        VisualElement rootElement = document.rootVisualElement;
        placementGrid = rootElement.Q(name: "module-table");
        VisualElement statusPanel = rootElement.Q(name: "status-header");
        orcDisplay = statusPanel.Q<Label>(name: "orcs-test-info");
        siegeButton = statusPanel.Q<Button>(name: "siege-button");
    }

    private void Start()
    {
        moduleGrid = new ModuleImage[cellGrid.gridWidth, cellGrid.gridHeight];
        moduleList = new Dictionary<ModuleImage, Vector2Int>();
        //for some reason, if this isn't delayed by 1 frame after start, the builder freezes.
        StartCoroutine(InitMachine());
    }

    private IEnumerator InitMachine()
    {
        yield return null;
        List<ModuleData> siegeData = GameManager.gameManager.siegeMachineData;
        if (siegeData != null)
        {
            foreach (ModuleData data in siegeData)
            {
                ModuleImage module = new ModuleImage(data.type);
                module.SetAsGridItem();
                AddModule(module, data.position);
                RegisterModule(module);
            }
        }
        UpdateDisplays();
    }

    public bool ValidSetup()
    {
        return numOrcs >= 0 && hasCockpit;
    }    

    public void RegisterModule(ModuleBase module)
    {
        numOrcs -= module.Orcs;
        if (module.ModuleID == "cockpit")
            hasCockpit = true;
        UpdateDisplays();
    }

    public void UnregisterModule(ModuleBase module)
    {
        numOrcs += module.Orcs;
        if (module.ModuleID == "cockpit" && !moduleList.Keys.Any(module => module.module.ModuleID == "cockpit"))
            hasCockpit = false;
        UpdateDisplays();
    }

    private void UpdateDisplays()
    {
        orcDisplay.text = numOrcs.ToString();
        if (numOrcs < 0)
        {
            siegeButton.text = "Too many modules!";
            siegeButton.parent.RemoveFromClassList("test-pass");
            siegeButton.parent.AddToClassList("test-fail");
        }
        else
        {
            if (!hasCockpit)
            {
                siegeButton.text = "Cockpit needed!";
                siegeButton.parent.RemoveFromClassList("test-pass");
                siegeButton.parent.AddToClassList("test-fail");
                return;
            }
            siegeButton.text = "Siege!";
            siegeButton.parent.AddToClassList("test-pass");
            siegeButton.parent.RemoveFromClassList("test-fail");
        }
    }

    public bool AddModule(ModuleImage module, Vector2Int pos)
    {
        List<Vector2Int> moduleCollision = module.module.GetCollisionInfo(pos);
        if (module == null || !cellGrid.CellsAvailable(moduleCollision))
            return false;
        if(moduleList.ContainsKey(module))
        {
            Debug.LogWarning("Added module that was already present in grid. Removing module from previous location.");
            RemoveModule(module);
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

    public Vector2Int RemoveModule(ModuleImage module)
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
        return originPos;
    }

    public ModuleImage RemoveModuleAt(Vector2Int pos, out Vector2Int originPos)
    {
        ModuleImage module = GetModuleAt(pos);
        originPos = RemoveModule(module);
        return module;
    }
}
