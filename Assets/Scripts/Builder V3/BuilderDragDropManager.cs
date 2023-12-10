using Builder2;
using System;
using System.Collections;
using System.Collections.Generic;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

public class BuilderDragDropManager : MonoBehaviour
{
    public ModuleImage currentDraggedModule { get; private set; }

    private Vector2Int originalModulePos;
    private ModuleBase originalModuleType; //different rotations are considered different module types, so this is needed to return to the original rotation

    private bool draggingOverCells;
    private Vector2Int currentFocusCellPos;

    private ModuleBase[] shieldsCache;
    private ModuleBase[] attackPartiesCache;

    private BuilderCellGrid cellGrid;
    private BuilderModuleManager moduleManager;

    private void Start()
    {
        cellGrid = GetComponent<BuilderCellGrid>();
        moduleManager = GetComponent<BuilderModuleManager>();

        shieldsCache = new ModuleBase[] {
            ModuleBase.ModuleTypes["shield-left"],
            ModuleBase.ModuleTypes["shield-right"],
            ModuleBase.ModuleTypes["shield-up"],
            ModuleBase.ModuleTypes["shield-down"] };
        attackPartiesCache = new ModuleBase[] {
            ModuleBase.ModuleTypes["orc-attack-party-0"],
            ModuleBase.ModuleTypes["orc-attack-party-90"],
            ModuleBase.ModuleTypes["orc-attack-party-180"],
            ModuleBase.ModuleTypes["orc-attack-party-270"]
        };
    }

    public void Grab(ModuleImage module)
    {
        if (currentDraggedModule != null)
            Release();
        currentDraggedModule = module;

        originalModulePos = moduleManager.RemoveModule(module);
        originalModuleType = module.module;

        currentFocusCellPos = new Vector2Int(-1, -1);
    }

    public void Release()
    {
        if (currentDraggedModule == null)
            return;
        if(!draggingOverCells) //means it's dragging over module library, aka trash
        {
            currentDraggedModule.RemoveFromHierarchy();
            currentDraggedModule = null;
        }
        else
        {
            if (cellGrid.CellsAvailable(currentDraggedModule.module.GetCollisionInfo(currentFocusCellPos)))
            {
                moduleManager.AddModule(currentDraggedModule, currentFocusCellPos);
                currentDraggedModule = null;
            }
            else
            {
                Revert();
            }
        }
    }

    public void Revert()
    {
        if (originalModulePos.x < 0) //means module came from library, not moving within the grid
        {
            currentDraggedModule.RemoveFromHierarchy();
        }
        else
        {
            currentDraggedModule.module = originalModuleType;
            moduleManager.AddModule(currentDraggedModule, originalModulePos);
        }
        currentDraggedModule = null;
    }

    public void Drag(Vector2 mousePos)
    {
        currentDraggedModule.transform.position = mousePos;

        //check if cursor is over module library. if not, continue
        draggingOverCells = true;
        DragOverGrid();

        void DragOverGrid()
        {
            Vector2Int newClosestCellPos = cellGrid.GetClosestCellFromMouse(mousePos, currentDraggedModule.module.PlacementType);

            if ((Shield)currentDraggedModule.module != null)
            {
                UpdateShieldRotation(newClosestCellPos);
            }

            EnforceModuleBounds(ref newClosestCellPos);

            if (newClosestCellPos == currentFocusCellPos)
                return;
            currentFocusCellPos = newClosestCellPos;


            cellGrid.HighlightCells(currentDraggedModule.module.GetCollisionInfo(newClosestCellPos));
        }
    }

    public void Rotate()
    {
        if (currentDraggedModule != null && (OrcAttackParty)currentDraggedModule.module != null)
            UpdateAttackPartyRotation();
    }

    private void EnforceModuleBounds(ref Vector2Int targetCell)
    {
        RectInt bounds = currentDraggedModule.module.GridBounds;
        int top = targetCell.y - bounds.yMin;
        int bottom = top + bounds.height; //this is actually 1 row below the lowest point on the module, but this makes calculations easier
        if (top < 0)
            targetCell.y -= top; //top is negative so this will increase y, pushing the module down
        else if (bottom > cellGrid.gridHeight)
            targetCell.y -= (bottom - cellGrid.gridHeight);

        int left = targetCell.x - bounds.xMin;
        int right = left + bounds.width; //this is actually 1 column to the right of the rightmost point on the module, but this makes calculations easier
        if (left < 0)
            targetCell.x -= left; //top is negative so this will increase x, pushing the module to the right
        else if (right > cellGrid.gridWidth)
            targetCell.x -= (right - cellGrid.gridWidth);
    }

    private void UpdateShieldRotation(Vector2Int targetCell)
    {
        bool xEdge = targetCell.x == 0 || targetCell.x == cellGrid.gridWidth - 1;
        bool yEdge = targetCell.y == 0 || targetCell.y == cellGrid.gridHeight - 1;

        if (xEdge && yEdge) //when target cell is corner, ignore
            return;

        if(targetCell.x == 0) //left edge
        {
            currentDraggedModule.module = shieldsCache[0];
        }
        else if(targetCell.x == cellGrid.gridWidth - 1) //right edge
        {
            currentDraggedModule.module = shieldsCache[1];
        }
        else if (targetCell.y == 0) //top edge
        {
            currentDraggedModule.module = shieldsCache[2];
        }
        else if (targetCell.y == cellGrid.gridHeight - 1) //bottom edge
        {
            currentDraggedModule.module = shieldsCache[3];
        }
    }

    private void UpdateAttackPartyRotation()
    {
        OrcAttackParty attackPartyModule = (OrcAttackParty)currentDraggedModule.module;
        int newRotation = attackPartyModule.rotation + 1;
        if (newRotation == 4)
            newRotation = 0;
        currentDraggedModule.module = attackPartiesCache[newRotation];
    }
}
