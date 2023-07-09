using System;
using System.Collections.Generic;
using System.Linq;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder2
{
    public class DragAndDropManipulator : PointerManipulator
    {
        private ModuleBase _wip = null;

        private List<Slot> SlotSlots { get; set; }

        public delegate void RejectedDropHandler(PointerManipulator manipulator);

        public delegate void SuccessfulDropHandler(PointerManipulator manipulator, ModuleBase module,
            Slot slot);

        public delegate void DeletedHandler(PointerManipulator manipulator);

        public delegate bool CanDropHandler(PointerManipulator manipulator, ModuleBase module, Slot slot);

        public delegate void BeforeUnslotHandler(PointerManipulator manipulator, ModuleBase module, Slot slot);

        public DragAndDropManipulator(VisualElement draggable, VisualElement allSlotRoot,
            Dictionary<Tuple<int, int>, Slot> slots, VisualElement dragVisualizer)
        {
            target = draggable;
            AllSlotRoot = allSlotRoot;
            DragVisualizer = dragVisualizer;

            SlotSlots = slots.Values.ToList();
            AllVirtualSlots = slots.Values.Select(k => k.PlacementSlot).ToList();
            Debug.Log("found " + AllVirtualSlots.Count + " virtual slots");
            AllSlots = slots.Values.Select(k => k.PlacementSlot).ToList();
            Debug.Log("found " + AllSlots.Count + " virtual slots");
        }

        public static CanDropHandler CanDropCheck { get; set; }

        public List<VisualElement> AllSlots { get; }
        public List<VisualElement> AllVirtualSlots { get; }

        private Vector2 TargetStartPosition { get; set; }
        private Slot OriginalParent { get; set; }
        private Vector3 PointerStartPosition { get; set; }
        private bool Enabled { get; set; }
        public VisualElement AllSlotRoot { get; }
        private VisualElement DragVisualizer { get; }

        public static event SuccessfulDropHandler OnSuccessfulDrop;
        public static event RejectedDropHandler OnRejectedDrop;
        public static event DeletedHandler OnDeleted;
        public static event BeforeUnslotHandler BeforeUnslot;


        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        private void PointerDownHandler(PointerDownEvent evt)
        {
            if (evt.button == (int) MouseButton.LeftMouse)
            {
                Debug.Log("PointerDownHandler");
                // convert the position to the DragVisualizer's local space
                var visualSpace = DragVisualizer.WorldToLocal(target.LocalToWorld(target.layout.position));
                OriginalParent = SlotSlots.FirstOrDefault(x => x.PlacementSlot == target.parent);
                target.RemoveFromHierarchy();
                DragVisualizer.Add(target);
                target.transform.position = visualSpace;
                TargetStartPosition = visualSpace;
                PointerStartPosition = evt.position;
                target.CapturePointer(evt.pointerId);
                var type = ((ModuleImage) target).Type;
                AllSlotRoot.AddToClassList("dragging-module-" + type);
                _wip = ModuleBase.ModuleTypes[type]();
                Enabled = true;
            }
            else if (evt.button == (int) MouseButton.RightMouse)
            {
                var parent = SlotSlots.FirstOrDefault(x => x.PlacementSlot == target.parent);
                BeforeUnslot?.Invoke(this, ModuleBase.ModuleTypes[((ModuleImage) target).Type](), parent);
                target.RemoveFromHierarchy();
                OnDeleted?.Invoke(this);
            }
        }

        private void Revert()
        {
            if (OriginalParent != null)
            {
                target.RemoveFromHierarchy();
                OriginalParent.PlacementSlot.Add(target);
                target.transform.position = Vector3.zero;
                OnRejectedDrop?.Invoke(this);
                OriginalParent = null;
            }
        }

        /**
         * Move the target if the mouse pointer is captured.
         */
        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (Enabled && target.HasPointerCapture(evt.pointerId))
            {
                var delta = evt.position - PointerStartPosition;
                target.transform.position = new Vector2(
                    TargetStartPosition.x + delta.x,
                    TargetStartPosition.y + delta.y
                );
            }
        }

        /**
         * Release the mouse pointer if it is captured.
         */
        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (Enabled && target.HasPointerCapture(evt.pointerId)) target.ReleasePointer(evt.pointerId);
            // calls PointerCaptureOutHandler(...) as a side effect in ReleasePointer(...)
        }

        /**
         * Drop it or something
         */
        private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
        {
            if (Enabled)
            {
                foreach (var s in AllSlotRoot.GetClasses().Where(x => x.StartsWith("dragging-module-")).ToList())
                {
                    AllSlotRoot.RemoveFromClassList(s);
                }
                Enabled = false;
                var overlapping = SlotSlots.Where(OverlapsTarget);
                var closestOverlapping = FindClosestSlot(overlapping.ToList());
                if (closestOverlapping != null)
                {
                    if (closestOverlapping.Occupied)
                    {
                        Revert();
                        return;
                    }

                    if (!CanDropCheck?.Invoke(this, _wip, closestOverlapping) ?? false)
                    {
                        Revert();
                        return;
                    }

                    if (OriginalParent != null)
                        BeforeUnslot?.Invoke(this, _wip, OriginalParent);
                    target.RemoveFromHierarchy();
                    closestOverlapping.PlacementSlot.Add(target);
                    target.transform.position = Vector3.zero;
                    OnSuccessfulDrop?.Invoke(this, _wip, closestOverlapping);
                }
                else
                {
                    Revert();
                }
            }
        }

        private bool OverlapsTarget(Slot slot)
        {
            return target.worldBound.Overlaps(slot.PlacementSlot.worldBound);
        }

        private Slot FindClosestSlot(List<Slot> slots)
        {
            var bestSquaredDistance = float.MaxValue;
            Slot closest = null;
            var myCenter = (Vector3) ((Vector2) target.transform.position + target.layout.size / 2);
            foreach (var slot in slots)
            {
                var displacement = VisualizerSpaceOfSlot(slot.PlacementSlot) - myCenter;
                var squaredDistance = displacement.sqrMagnitude;
                if (squaredDistance < bestSquaredDistance)
                {
                    bestSquaredDistance = squaredDistance;
                    closest = slot;
                }
            }

            return closest;
        }

        private Vector3 VisualizerSpaceOfSlot(VisualElement slot)
        {
            // calculate the center of the slot
            var corner = slot.layout.position + slot.layout.size / 2;
            var slotWorldSpace = slot.parent.LocalToWorld(corner);
            return DragVisualizer.WorldToLocal(slotWorldSpace);
        }
    }
}