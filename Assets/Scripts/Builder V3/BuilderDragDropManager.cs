using Builder2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuilderDragDropManager : MonoBehaviour
{
    private VisualElement currentlyDraggedVisual;
    private ModuleBase currentDraggedModule;

    private Vector2Int originalModulePos;
    private ModuleBase originalModuleType; //different rotations are considered different module types, so this is needed to return to the original rotation

    private bool draggingOverCells;
    private Vector2Int currentFocusCellPos;

    private ModuleBase[] shieldsCache;
    private ModuleBase[] attackPartiesCache;

    private BuilderCellGrid cellGrid;

    private void Start()
    {
        cellGrid = GetComponent<BuilderCellGrid>();
        shieldsCache = new ModuleBase[] {
            ModuleBase.ModuleTypes["shield-left"](),
            ModuleBase.ModuleTypes["shield-right"](),
            ModuleBase.ModuleTypes["shield-up"](),
            ModuleBase.ModuleTypes["shield-down"]() };
        attackPartiesCache = new ModuleBase[] {
            ModuleBase.ModuleTypes["orc-attack-party-0"](),
            ModuleBase.ModuleTypes["orc-attack-party-90"](),
            ModuleBase.ModuleTypes["orc-attack-party-180"](),
            ModuleBase.ModuleTypes["orc-attack-party-270"]()
        };
    }

    void Update()
    {
        if(currentlyDraggedVisual != null)
        {
            if ((OrcAttackParty)currentDraggedModule != null && Input.GetKeyDown(KeyCode.R))
                UpdateAttackPartyRotation();
            Drag();
        }
    }

    public void Grab()
    {

    }

    public void Release()
    {
        if (currentlyDraggedVisual == null)
            return;
        if(!draggingOverCells) //means it's dragging over module library, aka trash
        {
            //probably will go to a BuilderModuleManager script
            //moduleManager.Delete([probably a class that combines VisualElement and ModuleBase]);
        }
        else
        {
            if (cellGrid.CellsAvailable(currentDraggedModule.GetCollisionInfo(currentFocusCellPos)))
            {
                //moduleManager.AddModuleToGrid([class that combines VisualElement and ModuleBase], currentFocusCellPos);
            }
            else
            {
                if (originalModulePos.x < 0) //means module came from library, not moving within the grid
                {
                    //moduleManager.Delete([probably a class that combines VisualElement and ModuleBase]);
                }
                else
                {
                    //revert VisualElement and ModuleBase to original values
                    //moduleManager.AddModuleToGrid([class that combines VisualElement and ModuleBase], originalModulePos);
                }
            }
        }
    }

    private void Drag()
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos.y = Screen.height - mousePos.y; //input system uses y = 0 at bottom of screen, ui uses y = 0 at top of screen
        currentlyDraggedVisual.transform.position = mousePos;

        //check if cursor is over module library. if not, continue
        draggingOverCells = true;
        DragOverGrid();

        void DragOverGrid()
        {
            Vector2Int newClosestCellPos = cellGrid.GetClosestCellFromMouse(mousePos, currentDraggedModule.PlacementType);

            if ((Shield)currentDraggedModule != null)
            {
                UpdateShieldRotation(newClosestCellPos);
            }

            EnforceModuleBounds(ref newClosestCellPos);

            if (newClosestCellPos == currentFocusCellPos)
                return;
            currentFocusCellPos = newClosestCellPos;


            cellGrid.HighlightCells(currentDraggedModule.GetCollisionInfo(newClosestCellPos));
        }
    }

    private void EnforceModuleBounds(ref Vector2Int targetCell)
    {
        RectInt bounds = currentDraggedModule.GridBounds;
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
            currentDraggedModule = shieldsCache[0];
        }
        else if(targetCell.x == cellGrid.gridWidth - 1) //right edge
        {
            currentDraggedModule = shieldsCache[1];
        }
        else if (targetCell.y == 0) //top edge
        {
            currentDraggedModule = shieldsCache[2];
        }
        else if (targetCell.y == cellGrid.gridHeight - 1) //bottom edge
        {
            currentDraggedModule = shieldsCache[3];
        }
    }

    private void UpdateAttackPartyRotation()
    {
        OrcAttackParty attackPartyModule = (OrcAttackParty)currentDraggedModule;
        int newRotation = attackPartyModule.rotation + 1;
        if (newRotation == 4)
            newRotation = 0;
        currentDraggedModule = attackPartiesCache[newRotation];
    }
}
