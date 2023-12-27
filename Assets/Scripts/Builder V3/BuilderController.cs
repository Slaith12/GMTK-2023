using System;
using System.Collections.Generic;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BuilderDragDropManager))]
public class BuilderController : MonoBehaviour
{
    private BuilderDragDropManager dragDrop;
    private BuilderModuleManager moduleManager;
    private BuilderCellGrid cellGrid;

    private float dragDistance;
    const float MIN_DRAG = 20;

    #region Initialization

    private void Awake()
    {
        dragDrop = GetComponent<BuilderDragDropManager>();
        moduleManager = GetComponent<BuilderModuleManager>();
        cellGrid = GetComponent<BuilderCellGrid>();
    }

    private void Start()
    {
        UIDocument document = GetComponent<UIDocument>();

        document.rootVisualElement.Query<PaletteModule>(className: "palette-item").ForEach(RegisterPaletteItem);

        VisualElement grid = document.rootVisualElement.Q(name: "placements");
        grid.RegisterCallback<PointerDownEvent>(OnGridClicked);

        moduleManager.siegeButton.RegisterCallback<ClickEvent>(OnSiegeButtonClicked);
    }

    #endregion

    private void OnSiegeButtonClicked(ClickEvent _)
    {
        if (!moduleManager.ValidSetup())
            return;
        List<ModuleData> modules = new List<ModuleData>();
        foreach(KeyValuePair<ModuleImage, Vector2Int> module in moduleManager.GetModules())
        {
            modules.Add(new ModuleData(module.Key, module.Value));
        }
        GameManager.SetSiegeMachineData(modules);
        GameManager.GoToLevelSelect();
    }

    #region Module Palette Inputs

    private void RegisterPaletteItem(PaletteModule module)
    {
        module.RegisterCallback<PointerDownEvent, PaletteModule>(OnPaletteModuleClicked, module);
    }

    private void OnPaletteModuleClicked(PointerDownEvent evt, PaletteModule module)
    {
        if (evt.button == 0)
        {
            ModuleImage newModule = module.GetImageCopy();
            GrabModule(newModule, evt.position);
        }
    }

    #endregion

    #region Module Grid Inputs

    private void OnGridClicked(PointerDownEvent evt)
    {
        if (!cellGrid.IsMouseOnCell(evt.position))
            return;

        Vector2Int slot = cellGrid.GetClosestCellFromMouse(evt.position);
        ModuleImage module = moduleManager.GetModuleAt(slot);
        if (module == null)
            return;

        if (evt.button == 0)
            GrabModule(module, evt.position);
        else if (evt.button == 1) 
        {
            moduleManager.RemoveModule(module);
            moduleManager.UnregisterModule(module);
            dragDrop.PlayDeleteSound();
        }
    }

    private void OnHeldModuleClicked(PointerDownEvent evt, ModuleImage module)
    {
        if (!module.HasMouseCapture())
            return;
        dragDistance = MIN_DRAG * 2;
    }

    private void OnHeldModuleDrag(PointerMoveEvent evt, ModuleImage module)
    {
        dragDistance += evt.deltaPosition.magnitude;
        dragDrop.Drag(evt.position);
    }

    private void OnHeldModuleReleased(PointerUpEvent evt, ModuleImage module)
    {
        if (!module.HasMouseCapture())
            return;
        //if the user released the mouse soon after clicking, use "click and drag" logic instead of "hold and drag"
        if (evt.button == 0 && dragDistance > MIN_DRAG)
        {
            dragDrop.Release();
            module.ReleaseMouse();
        }
        else if(evt.button == 1)
        {
            dragDrop.Revert();
            module.ReleaseMouse();
        }

    }

    private void GrabModule(ModuleImage module, Vector2 mousePos)
    {
        module.RegisterCallback<PointerDownEvent, ModuleImage>(OnHeldModuleClicked, module);
        module.RegisterCallback<PointerUpEvent, ModuleImage>(OnHeldModuleReleased, module);
        module.RegisterCallback<MouseCaptureOutEvent, ModuleImage>(OnHeldModuleReleaseMouse, module);
        module.RegisterCallback<PointerMoveEvent, ModuleImage>(OnHeldModuleDrag, module);
        dragDrop.Grab(module);
        dragDrop.Drag(mousePos);
        dragDistance = 0;
        module.CaptureMouse();
    }

    private void OnHeldModuleReleaseMouse(MouseCaptureOutEvent evt, ModuleImage module)
    {
        module.UnregisterCallback<PointerDownEvent, ModuleImage>(OnHeldModuleClicked);
        module.UnregisterCallback<PointerUpEvent, ModuleImage>(OnHeldModuleReleased);
        module.UnregisterCallback<MouseCaptureOutEvent, ModuleImage>(OnHeldModuleReleaseMouse);
        module.UnregisterCallback<PointerMoveEvent, ModuleImage>(OnHeldModuleDrag);
    }

    #endregion
}
