using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gamestrap
{
    [System.Flags]
    public enum EUIType
    {
        Image       = 0x1,
        RawImage    = 0x2,
        Text        = 0x4,
        Selectable  = 0x8,
        Button      = 0x10,
        Toggle      = 0x20,
        Slider      = 0x40,
        Scrollbar   = 0x80,
        Dropdown    = 0x100,
        InputField  = 0x200,
        ScrollRect  = 0x400,
        Custom      = 0x800,
    }

    public static class EUITypeExtension
    {
        public static Type GetUIType(this EUIType uiType)
        {
            switch(uiType) {
                case EUIType.Image: return typeof(Image);
                case EUIType.RawImage: return typeof(RawImage);
                case EUIType.Text: return typeof(Text);
                case EUIType.Selectable: return typeof(Selectable);
                case EUIType.Button: return typeof(Button);
                case EUIType.Toggle: return typeof(Toggle);
                case EUIType.Slider: return typeof(Slider);
                case EUIType.Scrollbar: return typeof(Scrollbar);
                case EUIType.Dropdown: return typeof(Dropdown);
                case EUIType.InputField: return typeof(InputField);
                case EUIType.ScrollRect: return typeof(ScrollRect);
                case EUIType.Custom : return typeof(ISearchableComponent);
                default: return null;
            }
        }
    }

    // Subset of EUType
    [System.Flags]
    public enum EUIEffectType
    {
        Image = 0x1,
        RawImage = 0x2,
        Text = 0x4
    }
}