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

        public delegate void RejectedDropHandler(PointerManipulator manipulator);

        public delegate void SuccessfulDropHandler(PointerManipulator manipulator, ModuleBase module,
            VisualElement slot);

        public delegate void DeletedHandler(PointerManipulator manipulator);

        public delegate bool CanDropHandler(PointerManipulator manipulator, ModuleBase module, VisualElement slot);

        public DragAndDropManipulator(VisualElement draggable, VisualElement slotRoot,
            Dictionary<Tuple<int, int>, Slot> slots, VisualElement dragVisualizer)
        {
            target = draggable;
            Root = slotRoot;
            DragVisualizer = dragVisualizer;

            AllVirtualSlots = slots.Values.Select(k => k.PlacementSlot).ToList();
            Debug.Log("found " + AllVirtualSlots.Count + " virtual slots");
            AllSlots = slots.Values.Select(k => k.PlacementSlot).ToList();
            Debug.Log("found " + AllSlots.Count + " virtual slots");
        }

        public static CanDropHandler CanDropCheck { get; set; }

        public List<VisualElement> AllSlots { get; }
        public List<VisualElement> AllVirtualSlots { get; }

        private Vector2 TargetStartPosition { get; set; }
        private VisualElement OriginalParent { get; set; }
        private Vector3 PointerStartPosition { get; set; }
        private bool Enabled { get; set; }
        private VisualElement Root { get; }
        private VisualElement DragVisualizer { get; }

        public static event SuccessfulDropHandler OnSuccessfulDrop;
        public static event RejectedDropHandler OnRejectedDrop;
        public static event DeletedHandler OnDeleted;


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
                OriginalParent = target.parent;
                target.RemoveFromHierarchy();
                DragVisualizer.Add(target);
                target.transform.position = visualSpace;
                TargetStartPosition = visualSpace;
                PointerStartPosition = evt.position;
                target.CapturePointer(evt.pointerId);
                var type = ((ModuleImage) target).Type;
                _wip = ModuleBase.ModuleTypes[type]();
                Enabled = true;
            }
            else if (evt.button == (int) MouseButton.RightMouse)
            {
                target.RemoveFromHierarchy();
                OnDeleted?.Invoke(this);
            }
        }

        public void Rotate()
        {
            if (Enabled && _wip != null)
            {
                _wip.RotateCW();
                target.transform.rotation = Quaternion.Euler(0, 0, _wip.GetRotationAngle());
            }
        }

        private void Revert()
        {
            if (OriginalParent != null)
            {
                target.RemoveFromHierarchy();
                OriginalParent.Add(target);
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
                Enabled = false;
                var overlapping = AllVirtualSlots.Where(OverlapsTarget);
                var closestOverlapping = FindClosestSlot(overlapping.ToList());
                if (closestOverlapping != null)
                {
                    if (closestOverlapping.childCount > 0)
                    {
                        Revert();
                        return;
                    }

                    target.RemoveFromHierarchy();
                    closestOverlapping.Add(target);
                    target.transform.position = Vector3.zero;
                    OnSuccessfulDrop?.Invoke(this, _wip, closestOverlapping);
                }
                else
                {
                    Revert();
                }
            }
        }

        private bool OverlapsTarget(VisualElement slot)
        {
            return target.worldBound.Overlaps(slot.worldBound);
        }

        private VisualElement FindClosestSlot(List<VisualElement> slots)
        {
            var bestSquaredDistance = float.MaxValue;
            VisualElement closest = null;
            foreach (var slot in slots)
            {
                var displacement = VisualizerSpaceOfSlot(slot) -
                                   (target.transform.position + (Vector3) slot.layout.size / 2);
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
            var corner = slot.layout.position - slot.layout.size / 2;
            var slotWorldSpace = slot.parent.LocalToWorld(corner);
            return DragVisualizer.WorldToLocal(slotWorldSpace);
        }
    }
}