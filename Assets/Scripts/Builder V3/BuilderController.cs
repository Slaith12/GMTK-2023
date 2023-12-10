using System;
using System.Collections.Generic;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

public class BuilderController : MonoBehaviour
{
    private BuilderDragDropManager dragDrop;
    private BuilderModuleManager moduleManager;
    private BuilderCellGrid cellGrid;

    private float dragDistance;
    const float MIN_DRAG = 20;

    #region Initialization

    private void Start()
    {
        dragDrop = GetComponent<BuilderDragDropManager>();
        moduleManager = GetComponent<BuilderModuleManager>();
        cellGrid = GetComponent<BuilderCellGrid>();

        UIDocument document = GetComponent<UIDocument>();
        document.rootVisualElement.Query<PaletteModule>(className: "palette-item").ForEach(RegisterPaletteItem);
    }

    #endregion

    #region Module Palette Inputs

    private void RegisterPaletteItem(PaletteModule module)
    {
        module.RegisterCallback<PointerDownEvent, PaletteModule>(OnPaletteModuleClicked, module);
    }

    private void OnPaletteModuleClicked(PointerDownEvent evt, PaletteModule module)
    {
        if (evt.button == 0)
            CreateAndDragModule(module);
    }

    private void CreateAndDragModule(PaletteModule paletteModule)
    {
        ModuleImage newModule = paletteModule.GetImageCopy();
        RegisterGridItem(newModule);
        newModule.CaptureMouse();
    }

    #endregion

    #region Module Grid Inputs

    private void RegisterGridItem(ModuleImage module)
    {
        module.RegisterCallback<PointerDownEvent, ModuleImage>(OnGridModuleClicked, module);
        module.RegisterCallback<PointerUpEvent, ModuleImage>(OnGridModuleReleased, module);
        module.RegisterCallback<MouseCaptureEvent, ModuleImage>(OnGridModuleCaptureMouse, module);
        module.RegisterCallback<MouseCaptureOutEvent, ModuleImage>(OnGridModuleReleaseMouse, module);
    }

    private void OnGridModuleClicked(PointerDownEvent evt, ModuleImage module)
    {
        if (module.HasMouseCapture())
        {
            if (evt.button == 0)
            {
                dragDrop.Release();
                module.ReleaseMouse();
            }
        }
        else
        {
            if (evt.button == 0)
                module.CaptureMouse();
        }
    }

    private void OnGridModuleDrag(PointerMoveEvent evt, ModuleImage module)
    {
        dragDistance += evt.deltaPosition.magnitude;
        dragDrop.Drag(evt.position);
    }

    private void OnGridModuleReleased(PointerUpEvent evt, ModuleImage module)
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
        }
    }

    private void OnGridModuleCaptureMouse(MouseCaptureEvent evt, ModuleImage module)
    {
        module.RegisterCallback<PointerMoveEvent, ModuleImage>(OnGridModuleDrag, module);
        dragDrop.Grab(module);
        dragDistance = 0;
    }

    private void OnGridModuleReleaseMouse(MouseCaptureOutEvent evt, ModuleImage module)
    {
        module.UnregisterCallback<PointerMoveEvent, ModuleImage>(OnGridModuleDrag);
    }

    #endregion
}
