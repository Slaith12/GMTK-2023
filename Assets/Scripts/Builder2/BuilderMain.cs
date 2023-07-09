using System.Collections.Generic;
using UIs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Builder2
{
    public class BuilderMain : MonoBehaviour
    {
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private AudioClip snapSound;
        [SerializeField] private AudioClip failSound;
        
        private readonly List<DragAndDropManipulator> _dragAndDropManipulators = new();
        private VisualElement _dragVisualizer;
        private List<VisualElement> _slots = new();

        private void Start()
        {
            var document = GetComponent<UIDocument>();
            _dragAndDropManipulators.Clear();
            _slots = document.rootVisualElement.Query<VisualElement>(null, "slot").ToList();
            _dragVisualizer = document.rootVisualElement.Q<VisualElement>("drag-overlay");
            _dragVisualizer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

            var slotRoot = document.rootVisualElement.Q<VisualElement>("rows");
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
                        _dragAndDropManipulators.Add(new DragAndDropManipulator(copy, slotRoot, _dragVisualizer));
                    });
                });

            DragAndDropManipulator.OnSuccessfulDrop += (_, _) =>
            {
                audioPlayer.PlayOneShot(snapSound);
            };
            DragAndDropManipulator.OnRejectedDrop += _ =>
            {
                audioPlayer.PlayOneShot(failSound);
            };
        }

        private void OnGUI()
        {
        }
    }
}