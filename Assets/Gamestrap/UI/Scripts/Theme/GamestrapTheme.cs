using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Gamestrap
{
    /// <summary>
    /// Scriptable Object incharge of saving all of the UI settings of Gamestrap UI Toolkit
    /// </summary>
    [CreateAssetMenu(fileName = "Theme", menuName = "Gamestrap/Theme")]
    public class GamestrapTheme : ScriptableObject
    {
        public List<ColorGroup> colors = new List<ColorGroup>();

        public string[] GetAllColorNames()
        {
            List<string> list = new List<string>();
            foreach (ColorGroup cg in colors)
            {
                list.AddRange(cg.Names);
            }
            return list.Distinct().ToArray();
        }

        #region ColorGroup Methods

        public void AssignColorGroup(ColorGroup cg, GameObject g)
        {
            // Check Tags
            if (cg.tag != string.Empty && g.tag != cg.tag)
                return;
            // Check Regex
            if (cg.regex != string.Empty && !Regex.IsMatch(g.gameObject.name, cg.regex))
                return;

            List<Type> types = cg.GetTypes();
            foreach (Type type in types)
            {
                if (type == null)
                    continue;
                Component[] components = g.GetComponentsInChildren(type, true);
                foreach (Component c in components)
                {
                    if (c is Selectable)
                    {
                        AssignColorGroup(cg, c as Selectable);
                    }

                    if (c is Graphic)
                    {
                        AssignColorGroup(cg, c as Graphic);
                    }

                }

                IModifier[] effectors = g.GetComponents<IModifier>();
                foreach (IModifier e in effectors)
                {
                    if (Application.isPlaying)
                        Destroy((Component)e);
                    else
                        DestroyImmediate((Component)e);
                }

                foreach (Modifier e in cg.effectors)
                {
                    if (e) e.Apply(g);
                }

                foreach (ColorGroupDescendant cgd in cg.descendants)
                {
                    ColorGroup childCG = GetColorGroup(cgd);
                    foreach (Transform transform in g.transform)
                    {
                        AssignColorGroup(childCG, transform.gameObject);
                    }
                }
            }
        }

        private ColorGroup GetColorGroup(ColorGroupDescendant cgd)
        {
            return colors.Find(cg => cg.instanceId.Value == cgd.target.Value);
        }

        public ColorGroup GetColorGroup(string colorGroupName)
        {
            return colors.Find(cg => cg.name == colorGroupName);
        }

        public void AssignColorGroup(ColorGroup cg, Selectable s)
        {
            // Check Tags
            if (cg.tag != string.Empty && s.tag != cg.tag)
                return;
            // Check Regex
            if (cg.regex != string.Empty && !Regex.IsMatch(s.gameObject.name, cg.regex))
                return;

#if UNITY_EDITOR
            Undo.RecordObject(s, "Color Change");
#endif
            ColorBlock colors = s.colors;
            NamedColor namedColor;
            if (cg.TryGetColor(UIVars.Normal, out namedColor))
                colors.normalColor = namedColor.color;
            if (cg.TryGetColor(UIVars.Highlighted, out namedColor))
                colors.highlightedColor = namedColor.color;
            if (cg.TryGetColor(UIVars.Pressed, out namedColor))
                colors.pressedColor = namedColor.color;
            if (cg.TryGetColor(UIVars.Disabled, out namedColor))
                colors.disabledColor = namedColor.color;
            s.colors = colors;
        }

        public void AssignColorGroup(ColorGroup cg, Graphic g)
        {
            // Check Tags
            if (cg.tag != string.Empty && g.tag != cg.tag)
                return;
            // Check Regex
            if (cg.regex != string.Empty && !Regex.IsMatch(g.gameObject.name, cg.regex))
                return;
#if UNITY_EDITOR
            Undo.RecordObject(g, "Color Change");
#endif
            NamedColor color;
            if (cg.TryGetColor(UIVars.Color, out color))
                g.color = color.color;
        }
        #endregion

    }
}