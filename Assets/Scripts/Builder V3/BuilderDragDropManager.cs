using Builder2;
using System;
using System.Collections;
using System.Collections.Generic;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

//TODO: move audio related stuff to a separate script
[RequireComponent(typeof(BuilderModuleManager))]
public class BuilderDragDropManager : MonoBehaviour
{
    public ModuleImage currentDraggedModule { get; private set; }

    private Vector2Int originalModulePos;
    private ModuleBase originalModuleType; //different rotations are considered different module types, so this is needed to return to the original rotation
    private bool moduleCameFromLibrary => originalModulePos.x < 0;

    private bool draggingOverCells;
    private Vector2Int currentFocusCellPos;

    private ModuleBase[] shieldsCache;
    private ModuleBase[] attackPartiesCache;

    private BuilderCellGrid cellGrid;
    private BuilderModuleManager moduleManager;

    private VisualElement dragDropLayer;
    private VisualElement palette;
    private VisualElement trashOverlay;

    private Vector2 currentMousePos;

    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] AudioClip placeSound;
    [SerializeField] AudioClip deleteSound;
    [SerializeField] AudioClip failSound;

    private void Awake()
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

        UIDocument document = GetComponent<UIDocument>();
        dragDropLayer = document.rootVisualElement.Q(name: "drag-overlay");
        palette = document.rootVisualElement.Q(name: "palette");
        trashOverlay = palette.Q(name: "trash-overlay");
        trashOverlay.visible = false;
    }

    public void Grab(ModuleImage module)
    {
        if (currentDraggedModule != null)
            Release();

        originalModulePos = moduleManager.RemoveModule(module);
        originalModuleType = module.module;

        module.RemoveFromHierarchy();
        dragDropLayer.Add(module);

        currentDraggedModule = module;
        cellGrid.LightenCells(module.module.PlacementType);
        trashOverlay.visible = true;

        currentFocusCellPos = new Vector2Int(-1, -1);
        audioPlayer.PlayOneShot(pickupSound);
    }

    public void Release()
    {
        if (currentDraggedModule == null)
            return;
        if(!draggingOverCells) //means it's dragging over module library, aka trash
        {
            currentDraggedModule.RemoveFromHierarchy();
            if (!moduleCameFromLibrary)
                moduleManager.UnregisterModule(currentDraggedModule.module);
            currentDraggedModule = null;
            cellGrid.UnHighlightCells();
            cellGrid.UnLightenCells();
            trashOverlay.visible = false;
            audioPlayer.PlayOneShot(deleteSound);
        }
        else
        {
            if (cellGrid.CellsAvailable(currentDraggedModule.module.GetCollisionInfo(currentFocusCellPos)))
            {
                moduleManager.AddModule(currentDraggedModule, currentFocusCellPos);
                if (moduleCameFromLibrary)
                    moduleManager.RegisterModule(currentDraggedModule.module);

                currentDraggedModule = null;
                cellGrid.UnHighlightCells();
                cellGrid.UnLightenCells();
                trashOverlay.visible = false;
                audioPlayer.PlayOneShot(placeSound);
            }
            else
            {
                Revert(playSound: false);
                audioPlayer.PlayOneShot(failSound);
            }
        }
    }

    public void PlayDeleteSound()
    {
        audioPlayer.PlayOneShot(deleteSound);
    }

    public void Revert(bool playSound = true)
    {
        if (moduleCameFromLibrary)
        {
            currentDraggedModule.RemoveFromHierarchy();
            if (playSound)
                audioPlayer.PlayOneShot(deleteSound);
        }
        else
        {
            currentDraggedModule.module = originalModuleType;
            moduleManager.AddModule(currentDraggedModule, originalModulePos);
            if (playSound)
                audioPlayer.PlayOneShot(placeSound);
        }
        currentDraggedModule = null;
        cellGrid.UnHighlightCells();
        cellGrid.UnLightenCells();
        trashOverlay.visible = false;
    }

    public void Drag(Vector2 mousePos)
    {
        currentMousePos = mousePos;
        //check if cursor is over module library. if not, continue
        draggingOverCells = mousePos.y < palette.worldBound.yMin;
        if (draggingOverCells)
            DragOverGrid();
        else
            cellGrid.UnHighlightCells();

        Vector2 mouseOffset = (currentDraggedModule.module.GridBounds.min - Vector2.one * 0.5f) * cellGrid.cellSize;
        currentDraggedModule.transform.position = mousePos + mouseOffset;

        void DragOverGrid()
        {
            Vector2Int newClosestCellPos = cellGrid.GetClosestCellFromMouse(mousePos, currentDraggedModule.module.PlacementType);

            //TODO: Consider changing this to something that checks a property that indicates whether the module should auto-rotate
            if (currentDraggedModule.module.PlacementType == CellCategory.Edges)
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rotate();
        }
    }

    public void Rotate()
    {
        //TODO: Implement proper rotatable check that uses a property of the module rather than just checking if it's an OAP
        if (currentDraggedModule != null && currentDraggedModule.module.DisplayType.Contains("orc-attack-party"))
            UpdateAttackPartyRotation();
    }

    private void EnforceModuleBounds(ref Vector2Int targetCell)
    {
        //+y = down, -y = up
        RectInt bounds = currentDraggedModule.module.GridBounds;
        int top = targetCell.y + bounds.yMin;
        int bottom = top + bounds.height; //this is actually 1 row below the lowest point on the module, but this makes calculations easier
        if (top < 0)
            targetCell.y -= top; //top is negative so this will increase y, pushing the module down
        else if (bottom > cellGrid.gridHeight)
            targetCell.y -= (bottom - cellGrid.gridHeight);

        int left = targetCell.x + bounds.xMin;
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
        currentFocusCellPos = -Vector2Int.one;
        Drag(currentMousePos);
    }
}
