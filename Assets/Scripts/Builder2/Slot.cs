﻿using UnityEngine.UIElements;

namespace Builder2
{
    public class Slot
    {
        public bool Occupied => VisualSlot.childCount > 0 || _multiSlotOccupied;

        private bool _multiSlotOccupied;
        public VisualElement PlacementSlot { get; }

        public VisualElement VisualSlot { get; }

        public Slot(VisualElement placementSlot, VisualElement visualSlot)
        {
            PlacementSlot = placementSlot;
            VisualSlot = visualSlot;
        }
        
        public void MarkOccupied()
        {
            _multiSlotOccupied = true;
        }
        
        public void MarkUnoccupied()
        {
            _multiSlotOccupied = false;
        }

        public void AddModule(VisualElement moduleElement)
        {
            VisualSlot.Add(moduleElement);
        }
    }
}