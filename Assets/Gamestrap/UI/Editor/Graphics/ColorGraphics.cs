using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamestrap
{

    public class ColorGraphics : MonoBehaviour
    {

        public static Texture2D ButtonNormal { get { return GraphicsLoader.Get("button_normal"); } }
        public static Texture2D ButtonSelected { get { return GraphicsLoader.Get("button_selected"); } }
        public static Texture2D ButtonPressed { get { return GraphicsLoader.Get("button_pressed"); } }

        public static Texture2D IconBrush { get { return GraphicsLoader.Get("icon_brush"); } }
        public static Texture2D IconDuplicate { get { return GraphicsLoader.Get("icon_duplicate"); } }
        public static Texture2D IconHelp { get { return GraphicsLoader.Get("icon_help"); } }
        public static Texture2D IconPalette { get { return GraphicsLoader.Get("icon_palette"); } }
        public static Texture2D IconPen { get { return GraphicsLoader.Get("icon_pen"); } }
        public static Texture2D IconX { get { return GraphicsLoader.Get("icon_x"); } }

    }

}
