using System;
using UnityEngine;

namespace Gamestrap
{
    public class EditorHelper
    {

        public static void BackgroundColor(Color color, Action guiContent)
        {
            Color defaultColor = GUI.contentColor;
            GUI.contentColor = color;
            guiContent();
            GUI.contentColor = defaultColor;
        }

        /// <summary>
        /// For it to work you need to place before the element you want to check the event the following line:
        /// GUI.SetNextControlName("<UniqueName>");
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="type"></param>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool CheckEvent(string controlName, EventType type, KeyCode keyCode)
        {
            return controlName == GUI.GetNameOfFocusedControl()
                && Event.current.type == type 
                && keyCode == Event.current.keyCode;
        }

        public static bool IsColorDark(Color color) {
            return (1 - (0.299 * color.r + 0.587 * color.g + 0.114 * color.b)) > 0.5f;
        }

        public static void Draw(bool selected, GUIStyle style, Action action) {
            Color c = GUI.contentColor;
            GUI.contentColor = (selected) ? style.normal.textColor : style.active.textColor;
            action();
            GUI.contentColor = c;
        }

        public static bool DrawButton(bool selected, GUIStyle style, Func<bool> action)
        {
            Color c = GUI.contentColor;
            GUI.contentColor = (selected) ? style.active.textColor : style.normal.textColor;
            bool result = action();
            GUI.contentColor = c;
            return result;
        }

        public static bool DrawToggle(bool selected, GUIStyle style, Func<bool> action)
        {
            Color c = GUI.contentColor;
            GUI.contentColor = (selected) ? style.active.textColor : style.normal.textColor;
            bool result = action();
            GUI.contentColor = c;
            return result;
        }

        internal static bool Draw(bool editMode, Func<bool> p)
        {
            throw new NotImplementedException();
        }
    }
}