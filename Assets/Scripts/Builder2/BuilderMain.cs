using System;
using System.Collections.Generic;
using System.Linq;
using UIs;
using Unity.VisualScripting;
using UnityEditor;
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

        public static int InitialOrcs = 21;
        private readonly Dictionary<Vector2Int, Slot> _vslots = new(); //lists 
        private VisualElement _dragVisualizer; //displays item that is being dragged by player (doesn't actually move, only a container)
        private int _orcs;

        private void Start()
        {
            _orcs = InitialOrcs;
            var document = GetComponent<UIDocument>(); //get the ui document

            var slotRoot = document.rootVisualElement.Q<VisualElement>("placements"); //get the ui layer where the slot sprites are

            var virtualSlotRoot = slotRoot.Q<VisualElement>("virtual-placements"); //get the ui layer where the modules are placed

            var visualRows = slotRoot.Children().FirstOrDefault(child => child.ClassListContains("rows")); //get the ui element that contains the table of slot sprites
            if (visualRows == null) throw new ArgumentException("Could not find rows element (visual rows)");

            var virtualRows = virtualSlotRoot.Q<VisualElement>(className: "rows"); //get the ui element that contains the table of placed modules

            //convert the element tables into arrays of the columns
            var virtualColumns = virtualRows.Children().ToList();
            var visualColumns = visualRows.Children().ToList();

            //go through each slot and register it in _vslots using the coordinates and ui elements associated with it
            for (var x = 0; x < virtualColumns.Count; x++)
            {
                var virtualColumn = virtualColumns[x];
                var visualColumn = visualColumns[x];
                for (var y = 0; y < virtualColumn.childCount; y++)
                {
                    var virtualSlot = virtualColumn.Children().ElementAt(y);
                    var visualSlot = visualColumn.Children().ElementAt(y);
                    var slot = new Slot(x, y, virtualSlot, visualSlot);
                    _vslots.Add(new Vector2Int(x, y), slot);
                }
            }

            //initialize drag and drop functionality
            _dragVisualizer = document.rootVisualElement.Q<VisualElement>("drag-overlay");
            _dragVisualizer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            //go through every module available to the player and register them
            document.rootVisualElement.Query<PaletteModule>(null, "palette-item")
                .ForEach(paletteModule =>
                {
                    Debug.Log("register: " + paletteModule);
                    //when clicked, make a copy of the module and place it on drag visualizer to be dragged by the player
                    paletteModule.Image.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        VisualElement copy = paletteModule.GetImageCopy();
                        copy.AddToClassList("module-in-use");
                        Vector2 rootSpace =
                            _dragVisualizer.WorldToLocal(
                                paletteModule.Image.LocalToWorld(paletteModule.Image.layout.position));
                        _dragVisualizer.Clear();
                        _dragVisualizer.Add(copy);
                        copy.transform.position = rootSpace;
                        new DragAndDropManipulator(copy, slotRoot, _vslots.Values.ToList(), _dragVisualizer, evt);
                        _orcs -= ModuleBase.ModuleTypes[((ModuleImage)copy).Type]().Orcs;
                        RefreshDisplays();
                    });
                });

            DragAndDropManipulator.BeforeUnslot += OnDragAndDropManipulatorOnBeforeUnslot;
            DragAndDropManipulator.OnSuccessfulDrop += OnDragAndDropManipulatorOnOnSuccessfulDrop;
            DragAndDropManipulator.OnRejectedDrop += OnDragAndDropManipulatorOnOnRejectedDrop;
            DragAndDropManipulator.OnDeleted += OnDragAndDropManipulatorOnOnDeleted;
            DragAndDropManipulator.CanDropCheck = CanDropCheck;

            var siegeButton = document.rootVisualElement.Q("siege-button");
            siegeButton.RegisterCallback<ClickEvent>(evt =>
            {
                if (document.rootVisualElement.Q<VisualElement>("status-header").ClassListContains("test-fail")) return;
                var modules = GetSlottedModules();
                GameManager.SetSiegeMachineData(modules);
                DragAndDropManipulator.CanDropCheck = null;

                DragAndDropManipulator.BeforeUnslot -= OnDragAndDropManipulatorOnBeforeUnslot;
                DragAndDropManipulator.OnSuccessfulDrop -= OnDragAndDropManipulatorOnOnSuccessfulDrop;
                DragAndDropManipulator.OnRejectedDrop -= OnDragAndDropManipulatorOnOnRejectedDrop;
                DragAndDropManipulator.OnDeleted -= OnDragAndDropManipulatorOnOnDeleted;
                GameManager.GoToLevelSelect();
            });
            RefreshDisplays();
        }

        private void OnDragAndDropManipulatorOnBeforeUnslot(PointerManipulator manipulator, ModuleBase module, Slot slot)
        {
            Debug.Log("unslot");
            for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
            {
                var pos = new Vector2Int(slot.X + x, slot.Y + y);
                if (!_vslots.TryGetValue(pos, out var vslot)) continue;
                if (!module.IsBlocked(x, y)) continue;
                Debug.Log("unslot at " + pos);
                vslot.MarkUnoccupied();
            }

            RefreshDisplays();
        }

        private void OnDragAndDropManipulatorOnOnSuccessfulDrop(PointerManipulator manipulator, ModuleBase module, Slot slot)
        {
            audioPlayer.PlayOneShot(snapSound);
            for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
            {
                var pos = new Vector2Int(slot.X + x, slot.Y + y);
                if (!_vslots.TryGetValue(pos, out var vslot)) continue;
                if (!module.IsBlocked(x, y)) continue;
                vslot.MarkOccupied();
            }

            RefreshDisplays();
        }

        private void OnDragAndDropManipulatorOnOnRejectedDrop(PointerManipulator _)
        {
            audioPlayer.PlayOneShot(failSound);
        }

        private void OnDragAndDropManipulatorOnOnDeleted(ModuleBase module)
        {
            audioPlayer.PlayOneShot(delSound);
            _orcs += module.Orcs;
            RefreshDisplays();
        }

        private bool CanDropCheck(PointerManipulator manipulator, ModuleBase type, Slot slot)
        {
            for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
            {
                var pos = new Vector2Int(slot.X + x, slot.Y + y);
                if (_vslots.TryGetValue(pos, out var vslot))
                {
                    var blocked = type.IsBlocked(x, y);
                    var occupied = vslot.Occupied;
                    Debug.Log("slot at " + pos.x + ", " + pos.y + " is " + (blocked ? "blocked" : "not blocked") + " and " + (occupied ? "occupied" : "not occupied"));
                    if (type.IsBlocked(x, y) && vslot.Occupied) return false;
                }
                else if (type.IsBlocked(x, y))
                {
                    Debug.Log("rejected: " + type + " at " + slot.X + ", " + slot.Y + " because no slot found and blocked");
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
        }

        private List<ModuleData> GetSlottedModules()
        {
            var slots = _vslots.Values.ToList();
            List<ModuleData> modules = new List<ModuleData>();
            foreach (Slot slot in slots)
            {
                ModuleImage image = slot.PlacementSlot.Children().FirstOrDefault() as ModuleImage;
                if (image == null)
                    continue;
                ModuleBase module = ModuleBase.ModuleTypes[image.Type]();
                modules.Add(new ModuleData(module, new Vector2(slot.X, slot.Y)));
            }

            return modules;
        }

        private void RefreshDisplays()
        {
            var document = GetComponent<UIDocument>().rootVisualElement;
            var panel = document.Q<VisualElement>("status-header");
            var orcDisplay = panel.Q<Label>("orcs-test-info");
            var button = panel.Q<Button>("siege-button");
            orcDisplay.text = "" + _orcs;
            if (_orcs < 0)
            {
                button.text = "Not enough orcs!";
                panel.RemoveFromClassList("test-pass");
                panel.AddToClassList("test-fail");
            }
            else
            {
                var slots = GetSlottedModules();
                if (slots.FirstOrDefault(e => e.type is Cockpit) == null)
                {
                    button.text = "No cockpit!";
                    panel.RemoveFromClassList("test-pass");
                    panel.AddToClassList("test-fail");
                    return;
                }
                button.text = "Siege!";
                panel.AddToClassList("test-pass");
                panel.RemoveFromClassList("test-fail");
                
            }
        }
    }
}