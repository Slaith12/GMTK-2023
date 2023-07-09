using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UIs
{
    public class PaletteModule : VisualElement
    {
        private string m_Description;
        private string m_Label;

        private string m_ModuleType;

        public PaletteModule()
        {
            m_ModuleType = string.Empty;
            m_Label = string.Empty;
            m_Description = string.Empty;
            AddToClassList("palette-item");
            // the image - actual image is handled in the USS
            Add(GetImageCopy());
            // the stack
            var stack = new VisualElement();
            stack.AddToClassList("module-desc-stack");

            var label = new Label();
            label.AddToClassList("module-label");
            label.text = Label;
            stack.Add(label);

            var desc = new Label();
            desc.AddToClassList("module-desc");
            desc.text = Description;
            stack.Add(desc);

            Add(stack);
        }

        public string ModuleType
        {
            get => m_ModuleType;
            set
            {
                var image = this.Q<VisualElement>(className: "module-icon");
                if (image != null)
                {
                    image.RemoveFromClassList("module-type-" + m_ModuleType);
                    image.AddToClassList("module-type-" + value);
                }

                m_ModuleType = value;
            }
        }

        public string Label
        {
            get => m_Label;
            set
            {
                m_Label = value;
                var label = this.Q<Label>(className: "module-label");
                if (label != null) label.text = value;
            }
        }

        public string Description
        {
            get => m_Description;
            set
            {
                m_Description = value;
                var label = this.Q<Label>(className: "module-desc");
                if (label != null) label.text = value;
            }
        }

        public VisualElement Image => this.Q<VisualElement>(className: "module-icon");

        public VisualElement GetImageCopy()
        {
            var image = new VisualElement();
            image.AddToClassList("module-icon");
            image.AddToClassList("module-type-" + ModuleType);
            image.focusable = true;
            return image;
        }

        public new class UxmlFactory : UxmlFactory<PaletteModule, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription m_Description = new() {name = "description"};
            private readonly UxmlStringAttributeDescription m_Label = new() {name = "label"};
            private readonly UxmlStringAttributeDescription m_ModuleType = new() {name = "module-type"};

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((PaletteModule) ve).ModuleType = m_ModuleType.GetValueFromBag(bag, cc);
                ((PaletteModule) ve).Label = m_Label.GetValueFromBag(bag, cc);
                ((PaletteModule) ve).Description = m_Description.GetValueFromBag(bag, cc);
            }
        }
    }
}