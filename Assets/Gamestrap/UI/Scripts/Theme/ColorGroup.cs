using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamestrap
{
    [System.Serializable]
    public class ColorGroup
    {
        public SerializableGuid instanceId;
        public string name = "";
        public string tag = "";
        public string regex = "";
        public EUIType uiMask = (EUIType)(-1);
        public List<ColorGroupDescendant> descendants;

        public List<NamedColor> colorList;
        public List<Modifier> effectors;
        
        private Dictionary<string, NamedColor> colors;

        #region Constructors
        public ColorGroup(string name)
        {
            instanceId = Guid.NewGuid();
            this.name = name;
            colorList = new List<NamedColor>();
            effectors = new List<Modifier>();
            descendants = new List<ColorGroupDescendant>();
        }

        public ColorGroup(ColorGroup colorGroup)
        {
            instanceId = Guid.NewGuid();
            this.name = colorGroup.name + "_copy";
            colorList = new List<NamedColor>();
            effectors = new List<Modifier>();
            descendants = new List<ColorGroupDescendant>();

            this.uiMask = colorGroup.uiMask;

            foreach (NamedColor nc in colorGroup.colorList) {
                colorList.Add(new NamedColor(nc));
            }

            foreach (Modifier effector in colorGroup.effectors)
            {
                effectors.Add(effector);
            }

            foreach (ColorGroupDescendant cgd in colorGroup.descendants) {
                descendants.Add(new ColorGroupDescendant(cgd));
            }


        }
        #endregion

        #region Color List Management
        public void Add(NamedColor nc)
        {
            colorList.Add(nc);
            ReloadColorList();
        }

        public void Remove(NamedColor nc)
        {
            colorList.Remove(nc);
            ReloadColorList();
        }

        public bool Contains(NamedColor nc)
        {
            return colorList.Contains(nc);
        }

        public bool TryGetColor(string name, out NamedColor nc)
        {
            if (colors == null) {
                ReloadColorList();
            }
            return colors.TryGetValue(name, out nc);
        }

        private void ReloadColorList()
        {
            if (colors != null) {
                foreach (NamedColor nc in colors.Values) {
                    if (nc != null)
                        nc.OnNameChange -= ReloadColorList;
                }
            }
            colors = new Dictionary<string, NamedColor>();
            if (colorList != null)
                foreach (NamedColor nc in colorList) {
                    if (colors.ContainsKey(nc.Name))
                        Debug.LogWarning(
                            string.Format("Duplicate Color Name '{0}' in '{1}'", name, nc.Name));
                    else {
                        colors.Add(nc.Name, nc);
                        nc.OnNameChange += ReloadColorList;
                    }
                }
        }
        #endregion

        public List<Type> GetTypes()
        {
            List<Type> types = new List<Type>();
            foreach (EUIType t in Enum.GetValues(typeof(EUIType))) {
                if ((t & uiMask) != 0)
                    types.Add(t.GetUIType());
            }
            return types;
        }

        public int IndexOf(NamedColor item)
        {
            return colorList.IndexOf(item);
        }

        public int IndexOf(string name)
        {
            return colorList.FindIndex((nc) => nc.Name == name);
        }

        public void Insert(int index, NamedColor item)
        {
            colorList.Insert(index, item);
            ReloadColorList();
        }

        public void RemoveAt(int index)
        {
            colorList.RemoveAt(index);
            ReloadColorList();
        }

        public void Clear()
        {
            colorList.Clear();
            ReloadColorList();
        }

        public void CopyTo(NamedColor[] array, int arrayIndex)
        {
            colorList.CopyTo(array, arrayIndex);
        }

        #region Properties
        public int Count {
            get {
                return colorList.Count;
            }
        }

        public bool IsReadOnly { get { return ((IList<NamedColor>)colorList).IsReadOnly; } }

        public IList List {
            get { return colorList; }
        }

        public List<string> Names {
            get {
                List<string> names = new List<string>();
                foreach (NamedColor nc in colorList)
                    names.Add(nc.Name);
                return names;
            }
        }
        #endregion

        #region Overload
        public NamedColor this[int i] {
            get {
                if (i >= 0 && i < colorList.Count) {
                    return colorList[i];
                }
                Debug.LogWarning(string.Format("Trying to get index out of range '{0}'", i));
                return null;
            }
            set {
                colorList[i] = value;
                ReloadColorList();
            }
        }
        #endregion
    }

}

