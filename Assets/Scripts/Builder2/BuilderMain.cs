﻿using System;
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

        private readonly List<DragAndDropManipulator> _dragAndDropManipulators = new();
        private VisualElement _dragVisualizer;
        private List<VisualElement> _slots = new();

        private readonly Dictionary<Tuple<int, int>, Slot> _vslots = new();

        private void Start()
        {
            var document = GetComponent<UIDocument>();
            

            var slotRoot = document.rootVisualElement.Q<VisualElement>("placements");

            var virtualSlotRoot = slotRoot.Q<VisualElement>("virtual-placements");

            var visualRows = slotRoot.Children().FirstOrDefault(child => child.ClassListContains("rows"));
            if (visualRows == null)
            {
                throw new ArgumentException("Could not find rows element (visual rows)");
            }

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
                    var slot = new Slot(virtualSlot, visualSlot);
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
                        _dragAndDropManipulators.Add(new DragAndDropManipulator(copy, slotRoot, _vslots, _dragVisualizer));
                    });
                });

            DragAndDropManipulator.OnSuccessfulDrop += (_, _, _) => { audioPlayer.PlayOneShot(snapSound); };
            DragAndDropManipulator.OnRejectedDrop += _ => { audioPlayer.PlayOneShot(failSound); };
            DragAndDropManipulator.OnDeleted += _ => { audioPlayer.PlayOneShot(delSound); };
            DragAndDropManipulator.CanDropCheck = (manipulator, type, slot) =>
            {
                
                return true;
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _dragAndDropManipulators.ForEach(e => e.Rotate());
            }
        }
    }
}