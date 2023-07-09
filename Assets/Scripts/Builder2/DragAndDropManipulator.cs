using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder2
{
    public class DragAndDropManipulator : PointerManipulator
    {
        public delegate void SuccessfulDropHandler(PointerManipulator manipulator, VisualElement slot);

        public delegate void RejectedDropHandler(PointerManipulator manipulator);

        public static event SuccessfulDropHandler OnSuccessfulDrop;
        public static event RejectedDropHandler OnRejectedDrop;

        public DragAndDropManipulator(VisualElement draggable, VisualElement slotRoot, VisualElement dragVisualizer)
        {
            target = draggable;
            Root = slotRoot;
            DragVisualizer = dragVisualizer;
            AllSlots = Root.Query<VisualElement>(className: "slot").ToList();
        }

        public List<VisualElement> AllSlots { get; }

        private Vector2 TargetStartPosition { get; set; }
        private VisualElement OriginalParent { get; set; }
        private Vector3 PointerStartPosition { get; set; }
        private bool Enabled { get; set; }
        private VisualElement Root { get; }
        private VisualElement DragVisualizer { get; }


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
                Enabled = true;
            }
            else if (evt.button == (int) MouseButton.RightMouse)
            {
                target.RemoveFromHierarchy();
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
                var overlapping = AllSlots.Where(OverlapsTarget);
                var closestOverlapping = FindClosestSlot(overlapping.ToList());
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
                var overlapping = AllSlots.Where(OverlapsTarget);
                var closestOverlapping = FindClosestSlot(overlapping.ToList());
                if (closestOverlapping != null)
                {
                    if (closestOverlapping.childCount > 0)
                    {
                        Revert();
                        return;
                    }

                    OnSuccessfulDrop?.Invoke(this, closestOverlapping);
                    target.transform.position = VisualizerSpaceOfSlot(closestOverlapping) +
                                                ((Vector3) new Vector2(closestOverlapping.layout.width,
                                                    closestOverlapping.layout.height) / 2);
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
                var displacement = VisualizerSpaceOfSlot(slot) - target.transform.position;
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
            var slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
            return DragVisualizer.WorldToLocal(slotWorldSpace);
        }
    }
}