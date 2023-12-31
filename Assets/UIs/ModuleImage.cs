using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Builder2;
using UnityEngine;

namespace UIs
{
    public class ModuleImage : VisualElement
    {
        public ModuleImage() : this(null) {}

        public ModuleImage(ModuleBase module)
        {
            this.module = module;
            AddToClassList("module-icon");
            focusable = true;
        }

        private ModuleBase m_module;
        public ModuleBase module
        {
            get => m_module;
            set
            {
                if(m_module != null)
                    RemoveFromClassList($"module-type-{m_module.ModuleID}");
                m_module = value;
                if(m_module != null)
                    AddToClassList($"module-type-{m_module.ModuleID}");
            }
        }

        public void SetAsGridItem()
        {
            style.position = new StyleEnum<Position>(Position.Absolute);
            AddToClassList("module-in-use");
        }
        public string Type => m_module.ModuleID;

        public static implicit operator ModuleBase(ModuleImage image) { return image.module; }

        public new class UxmlFactory : UxmlFactory<ModuleImage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _type = new() {name = "type"};

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                string type = _type.GetValueFromBag(bag, cc);
                if(ModuleBase.ModuleTypes.ContainsKey(type))
                    ((ModuleImage) ve).module = ModuleBase.ModuleTypes[_type.GetValueFromBag(bag, cc)];
            }
        }
    }
}