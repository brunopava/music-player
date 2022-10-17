using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gamestrap
{
    public class ColorGroupPopupWindow : EditorWindow
    {
        private static float rowHeight = 30;
        private static int maxRows = 7;
        private static ColorGroupPopupWindow instance;
        private ColorGroup selectedColor;
        private System.Action<ColorGroup> selectedColorCallback;
        private List<ColorGroup> colors;
        private Vector2 scrollV;

        public static void ShowPopup(ColorGroup baseColor, List<ColorGroup> colors, System.Action<ColorGroup> selectedColorCallback)
        {
            if (instance == null) {
                instance = ScriptableObject.CreateInstance<ColorGroupPopupWindow>();
            }
            instance.selectedColor = baseColor;
            instance.colors = colors;
            instance.selectedColorCallback = selectedColorCallback;
            Vector2 realPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Vector2 size = new Vector2(200f, Mathf.Min(rowHeight * maxRows, instance.MaxHeight));
            instance.position = new Rect(realPosition.x, realPosition.y, size.x, size.y);
            instance.ShowPopup();
            instance.Focus();
        }

        private void OnLostFocus()
        {
            AssignColorGroup(selectedColor);
        }

        void OnGUI()
        {
            Rect r = this.position;
            r.x = 0;
            r.y = 0;
            Rect scrollRect = r;
            GUI.Box(r, "");
            scrollRect.height = MaxHeight;
            if (colors.Count > maxRows)
                scrollRect.width -= 15;
            scrollV = GUI.BeginScrollView(r, scrollV, scrollRect);
     
            Rect buttonRect = scrollRect;
            buttonRect.height = rowHeight;
            foreach (ColorGroup colorGroup in colors) {
                if (colorGroup == selectedColor)
                    continue;
                ThemeWindow.DrawColorGroupHorizontal(buttonRect, colorGroup, () =>
                {
                    AssignColorGroup(colorGroup);
                });
                buttonRect.y += rowHeight;
            }
            GUI.EndScrollView();
        }

        private void AssignColorGroup(ColorGroup cg)
        {
            selectedColorCallback(cg);
            Close();
        }

        private float MaxHeight {
            get {
                return rowHeight * colors.Count - ((selectedColor == null) ? 0 : rowHeight);
            }
        }
    }
}
