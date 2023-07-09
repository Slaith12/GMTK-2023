using System;
using System.Collections.Generic;
using System.Linq;
using UIs;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder2
{
    public class BuilderMain : MonoBehaviour
    {
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private AudioClip snapSound;
        [SerializeField] private AudioClip delSound;
        [SerializeField] private AudioClip failSound;

        [HideInInspector] public int nextLevel;

        private readonly List<DragAndDropManipulator> _dragAndDropManipulators = new();

        public static int InitialOrcs = 10;
        private readonly Dictionary<Tuple<int, int>, Slot> _vslots = new();
        private VisualElement _dragVisualizer;
        private int _orcs; // TODO
        private int _weight = 0;
        private List<VisualElement> _slots = new();

        private void Start()
        {
            _orcs = InitialOrcs;
            _weight = 0;
            var document = GetComponent<UIDocument>();

            var slotRoot = document.rootVisualElement.Q<VisualElement>("placements");

            var virtualSlotRoot = slotRoot.Q<VisualElement>("virtual-placements");

            var visualRows = slotRoot.Children().FirstOrDefault(child => child.ClassListContains("rows"));
            if (visualRows == null) throw new ArgumentException("Could not find rows element (visual rows)");

            var virtualRows = virtualSlotRoot.Q<VisualElement>(className: "rows");

            var virtualColumns = virtualRows.Children().ToList();
            var visualColumns = visualRows.Children().ToList();
            for (var x = 0; x < virtualColumns.Count; x++)
            {
                var virtualColumn = virtualColumns[x];
                var visualColumn = visualColumns[x];
                for (var y = 0; y < virtualColumn.childCount; y++)
                {
                    var virtualSlot = virtualColumn.Children().ElementAt(y);
                    var visualSlot = visualColumn.Children().ElementAt(y);
                    var slot = new Slot(x, y, virtualSlot, visualSlot);
                    _vslots.Add(new Tuple<int, int>(x, y), slot);
                }
            }

            _dragAndDropManipulators.Clear();
            _slots = document.rootVisualElement.Query<VisualElement>(null, "slot").ToList();
            _dragVisualizer = document.rootVisualElement.Q<VisualElement>("drag-overlay");
            _dragVisualizer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            document.rootVisualElement.Query<PaletteModule>(null, "palette-item")
                .ForEach(paletteModule =>
                {
                    Debug.Log("register: " + paletteModule);
                    paletteModule.Image.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        var copy = paletteModule.GetImageCopy();
                        copy.AddToClassList("module-in-use");
                        var rootSpace =
                            _dragVisualizer.WorldToLocal(
                                paletteModule.Image.LocalToWorld(paletteModule.Image.layout.position));
                        _dragVisualizer.Add(copy);
                        copy.transform.position = rootSpace;
                        _dragAndDropManipulators.Add(new DragAndDropManipulator(copy, slotRoot, _vslots,
                            _dragVisualizer));
                    });
                });

            DragAndDropManipulator.BeforeUnslot += (manipulator, module, slot) =>
            {
                Debug.Log("unslot");
                for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    var tuple = new Tuple<int, int>(slot.X + x, slot.Y + y);
                    if (!_vslots.TryGetValue(tuple, out var vslot)) continue;
                    if (!module.IsBlocked(x, y)) continue;
                    Debug.Log("unslot at " + tuple);
                    vslot.MarkUnoccupied();
                }

                _orcs -= module.Orcs;
                _weight -= module.Weight;
            };
            DragAndDropManipulator.OnSuccessfulDrop += (manipulator, module, slot) =>
            {
                audioPlayer.PlayOneShot(snapSound);
                for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    var tuple = new Tuple<int, int>(slot.X + x, slot.Y + y);
                    if (!_vslots.TryGetValue(tuple, out var vslot)) continue;
                    if (!module.IsBlocked(x, y)) continue;
                    vslot.MarkOccupied();
                }
                _orcs += module.Orcs;
                _weight += module.Weight;
            };
            DragAndDropManipulator.OnRejectedDrop += _ => { audioPlayer.PlayOneShot(failSound); };
            DragAndDropManipulator.OnDeleted += _ => { audioPlayer.PlayOneShot(delSound); };
            DragAndDropManipulator.CanDropCheck = (manipulator, type, slot) =>
            {
                for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    var tuple = new Tuple<int, int>(slot.X + x, slot.Y + y);
                    if (_vslots.TryGetValue(tuple, out var vslot))
                    {
                        var blocked = type.IsBlocked(x, y);
                        var occupied = vslot.Occupied;
                        Debug.Log("slot at " + tuple.Item1 + ", " + tuple.Item2 + " is " +
                                  (blocked ? "blocked" : "not blocked") + " and " +
                                  (occupied ? "occupied" : "not occupied"));
                        if (type.IsBlocked(x, y) && vslot.Occupied) return false;
                    }
                    else if (type.IsBlocked(x, y))
                    {
                        Debug.Log("rejected: " + type + " at " + slot.X + ", " + slot.Y +
                                  " because no slot found and blocked");
                        return false;
                    }
                }

                switch (type)
                {
                    case ShieldUp when slot.Y != 0:
                    case ShieldDown when slot.Y != 3:
                    case ShieldLeft when slot.X != 0:
                    case ShieldRight when slot.X != 7:
                        Debug.Log("rejected: constraints failed");
                        return false;

                    default:
                        return true;
                }
            };

            var siegeButton = document.rootVisualElement.Q("siege-button");
            siegeButton.RegisterCallback<ClickEvent>(evt =>
            {
                var slots = _vslots.Values.ToList();
                List<ModuleData> modules = new List<ModuleData>();
                foreach(Slot slot in slots)
                {
                    ModuleImage image = slot.PlacementSlot.Children().FirstOrDefault() as ModuleImage;
                    if (image == null)
                        continue;
                    ModuleBase module = ModuleBase.ModuleTypes[image.Type]();
                    modules.Add(new ModuleData(module, new Vector2(slot.X, slot.Y)));
                }
                GameManager.SetSiegeMachineData(modules);
                GameManager.GoToLevelSelect();
            });
        }

        private void RefreshDisplays()
        {
            var document = GetComponent<UIDocument>().rootVisualElement;
            var weightDisplay = document.Q<Label>("weight-display");
        }
    }
}