using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UIs
{
    public class ModuleImage : VisualElement
    {
        public ModuleImage()
        {
            Type = string.Empty;
        }

        public string Type { get; set; }

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
                ((ModuleImage) ve).Type = _type.GetValueFromBag(bag, cc);
            }
        }
    }
}