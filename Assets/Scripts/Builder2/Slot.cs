using UnityEngine.UIElements;

namespace Builder2
{
    public class Slot
    {
        public bool Occupied => PlacementSlot.childCount > 0 || _multiSlotOccupied;

        private bool _multiSlotOccupied;
        public VisualElement PlacementSlot { get; }

        public VisualElement VisualSlot { get; }
        
        public int X { get; }
        public int Y { get; }

        public Slot(int x, int y, VisualElement placementSlot, VisualElement visualSlot)
        {
            X = x;
            Y = y;
            PlacementSlot = placementSlot;
            VisualSlot = visualSlot;
        }
        
        public void MarkOccupied()
        {
            _multiSlotOccupied = true;
            VisualSlot.AddToClassList("occupied");
        }
        
        public void MarkUnoccupied()
        {
            _multiSlotOccupied = false;
            VisualSlot.RemoveFromClassList("occupied");
        }

        public void AddModule(VisualElement moduleElement)
        {
            VisualSlot.Add(moduleElement);
        }
    }
}