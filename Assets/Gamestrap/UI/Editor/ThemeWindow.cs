using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using UnityEditor.Callbacks;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Gamestrap
{
    public class ThemeWindow : EditorWindow
    {
        #region constant and static variables
        private const string prefsKey = "gamestrap.uitheme";
        private const string editModeKey = "gamestrap.editmode";
        private static ThemeWindow window;
        private static Vector2 minSizeSmall = new Vector2(200f, 200f);
        private static Vector2 minSizeMedium = new Vector2(275f, 305f);
        #endregion

        #region Editor variables
        private GamestrapTheme theme;

        private static Color guiBGColor;
        private static Color guiColor;

        private GamestrapTheme[] themes;
        private bool editMode = true;

        #region Color Mode Variables
        private ReorderableList elementsRList;
        private Vector2 elementsRListSP;
        private ReorderableList colorsList;
        private ReorderableList palleteList;

        private ReorderableList elementModifierList;
        private ReorderableList elementDescendantsRList;
        private Vector2 elementRListSP;
        private ColorGroup selectedElement;
        private string[] colorNames;

        private Vector2 toolbarSP;
        #endregion

        #endregion

        [MenuItem("Assets/Save Editor Skin")]
        static public void SaveEditorSkin()
        {
            GUISkin skin = ScriptableObject.Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector)) as GUISkin;
            AssetDatabase.CreateAsset(skin, "Assets/EditorSkin.guiskin");
        }

        [MenuItem("Window/Gamestrap UI")]
        public static ThemeWindow ShowWindow()
        {
            window = (ThemeWindow)EditorWindow.GetWindow(typeof(ThemeWindow), false, "GSUI");
            window.minSize = minSizeMedium;
            window.autoRepaintOnSceneChange = true;
            return window;
        }

        //Temp, this shouldn't be here
        [MenuItem("Assets/Apply")]
        static public void InsertData()
        {
            var content = GameObject.Find("Content");
            var objects = Selection.objects;
            foreach (var o in objects) {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(o));
                var child = new GameObject(o.name);
                child.AddComponent<UnityEngine.UI.Image>().sprite = sprite;
                child.transform.SetParent(content.transform, false);
            }
        }

        [OnOpenAssetAttribute()]
        public static bool OpenInProjectWindow(int instanceID, int line)
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is GamestrapTheme)
            {
                GamestrapTheme theme = obj as GamestrapTheme;
                ThemeWindow window = ShowWindow();
                window.Theme = theme;
                return true;
            }
            return false;
        }

        #region Unity Editor GUI
        void OnEnable()
        {
            guiBGColor = GUI.backgroundColor;
            guiColor = GUI.color;

            if (EditorPrefs.HasKey(editModeKey))
                EditMode = EditorPrefs.GetBool(editModeKey);
            SetupColorRList();
            ApplyEditMode(elementsRList, EditMode);
            EditorApplication.modifierKeysChanged += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.modifierKeysChanged -= Repaint;
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            // Theme Selection
            Theme = (GamestrapTheme)EditorGUILayout.ObjectField(Theme, typeof(GamestrapTheme), false);
            if (Theme == null)
            {
                EditorGUILayout.HelpBox("Please select a theme", MessageType.Info);
                return;
            }
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 75f;
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(Theme, "Content change in theme");


            bool temp = EditorHelper.DrawToggle(EditMode, StylesUI.Icon, () =>
            {
                return GUILayout.Toggle(EditMode, StylesUI.EditButton, StylesUI.IconButton);
            });

            if (!temp && EditMode)
            {
                elementsRList.index = -1;
            }
            if (temp != EditMode)
            {
                ApplyEditMode(elementsRList, temp);
                this.minSize = (temp) ? minSizeMedium : minSizeSmall;
            }
            EditMode = temp;

            GUILayout.EndHorizontal();

            OnColorModeSelected();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Theme);
            }
            EditorGUIUtility.labelWidth = labelWidth;
        }

        private void ApplyEditMode(ReorderableList list, bool editable)
        {
            list.draggable = editable;
            list.displayAdd = editable;
            list.displayRemove = editable;
            list.draggable = editable;
        }

        private void OnColorModeSelected()
        {
            if (!EditMode)
            {
                elementsRListSP = EditorGUILayout.BeginScrollView(elementsRListSP);
                elementsRList.DoLayoutList();
                EditorGUILayout.EndScrollView();
                return;
            }
            GUILayout.BeginHorizontal();
            StylesUI.LeftColumn.fixedWidth = Mathf.Clamp(position.width * 0.3f, 50f, 300f);
            GUILayout.BeginVertical(StylesUI.LeftColumn);
            elementsRListSP = EditorGUILayout.BeginScrollView(elementsRListSP);
            elementsRList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            if (!EditMode)
                return;
            GUILayout.BeginVertical();
            if (elementsRList.index != -1 && SelectedCG != null)
            {
                GUILayout.Space(15f);
                GUILayout.BeginVertical("Box");
                GUILayout.Label("Element " + SelectedCG.name, StylesUI.TitleLabel);
                GUILayout.Space(10f);
                GUILayout.BeginHorizontal();
                SelectedCG.name = EditorGUILayout.TextField("Name", SelectedCG.name);
                if (EditorHelper.DrawButton(false, StylesUI.Icon, () =>
                    {
                        return GUILayout.Button(StylesUI.DuplicateButton, StylesUI.IconButton);
                    }))
                {
                    Theme.colors.Add(new ColorGroup(SelectedCG));
                    UpdateColorNames();
                }
                GUILayout.EndHorizontal();
                
                GUILayout.Space(5f);
                GUILayout.Label("Filter Properties", StylesUI.TitleLabel);
                GUILayout.Space(5f);
                // #if UNITY_2017_4_OR_NEWER
                EUIType t = (EUIType)EditorGUILayout.EnumFlagsField("Type", (EUIType)SelectedCG.uiMask, StylesUI.Popup);
                if (t != SelectedCG.uiMask)
                {
                    SelectedCG.uiMask = t;
                    UpdateColors(SelectedCG);
                }
                // #endif
                GUILayout.BeginHorizontal();
                SelectedCG.tag = EditorGUILayout.TagField("Tag", SelectedCG.tag, StylesUI.Popup);
                if (EditorHelper.DrawButton(false, StylesUI.Icon, () =>
                    {
                        return GUILayout.Button(StylesUI.DeleteTextButton, StylesUI.IconButton);
                    }))
                {
                    SelectedCG.tag = "";
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                SelectedCG.regex = EditorGUILayout.TextField("Regex", SelectedCG.regex);
                if (EditorHelper.DrawButton(false, StylesUI.Icon, () =>
               {
                   return GUILayout.Button(StylesUI.HelpButton, StylesUI.IconButton);
               }))
                {
                    Application.OpenURL("https://msdn.microsoft.com/en-us/library/az24scfc(v=vs.110).aspx");
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
                GUILayout.EndVertical();

                GUILayout.Space(10f);

                elementRListSP = EditorGUILayout.BeginScrollView(elementRListSP);

                if (colorsList != null)
                {
                    colorsList.DoLayoutList();
                    GUILayout.Space(15f);
                }

                if (palleteList != null)
                {
                    palleteList.DoLayoutList();
                    GUILayout.Space(15f);
                }

                if (elementModifierList != null)
                {
                    elementModifierList.DoLayoutList();
                    GUILayout.Space(15f);
                }

                if (elementDescendantsRList != null)
                {
                    elementDescendantsRList.DoLayoutList();
                }

                EditorGUILayout.EndScrollView();

                GUILayout.BeginHorizontal();
                GameObject prefab = null;
                if (Selection.activeGameObject) {
                    #if UNITY_2018_3_OR_NEWER
                    prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.activeGameObject);
                    #else
                    prefab = PrefabUtility.FindPrefabRoot(Selection.activeGameObject);
                    #endif
                }
                string smartSelection = "Scene";
                if (prefab && AssetDatabase.Contains(prefab))
                {
                    smartSelection = "Prefab";
                } else if (Selection.gameObjects.Length > 0) {
                    smartSelection = "Selection";
                }
                if (GUILayout.Button("Apply to " + smartSelection))
                {
                    AssignColorGroup(SelectedCG);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void UpdateColors(ColorGroup cg)
        {
            Dictionary<string, NamedColor> old = new Dictionary<string, NamedColor>();
            for (int i = 0; i < cg.Count; i++)
            {
                NamedColor nc = cg[i];
                if (old.ContainsKey(nc.Name))
                    continue;
                old.Add(nc.Name, nc);
            }
            EUIType type = cg.uiMask;

            // Private checker for adding new keys to color List
            Action<string> CheckColor = (k) =>
            {
                if (old.ContainsKey(k))
                {
                    if (!cg.Contains(old[k]))
                        cg.Add(old[k]);
                }
                else
                {
                    cg.Add(new NamedColor(k, Color.white));
                }
            };

            Action<string> RemoveColor = (k) =>
            {
                if (old.ContainsKey(k) && cg.Contains(old[k]))
                    cg.Remove(old[k]);
            };

            if (((EUIType.Text | EUIType.RawImage | EUIType.Image) & type) == 0)
            {
                RemoveColor(UIVars.Color);
            }
            else
            {
                CheckColor(UIVars.Color);
            }

            if (((EUIType.Selectable | EUIType.Button | EUIType.Toggle) & type) == 0)
            {
                RemoveColor(UIVars.Normal);
                RemoveColor(UIVars.Highlighted);
                RemoveColor(UIVars.Pressed);
                RemoveColor(UIVars.Disabled);
            }
            else
            {
                CheckColor(UIVars.Normal);
                CheckColor(UIVars.Highlighted);
                CheckColor(UIVars.Pressed);
                CheckColor(UIVars.Disabled);
            }
        }

        private void OnSelectionChange()
        {
            Repaint();
        }
        #endregion

        #region ReorderableLists

        private void SetupColorRList()
        {
            elementsRList = new ReorderableList(Theme.colors, typeof(ColorGroup), true, true, true, true);
            elementsRList.elementHeight = 40f;
            elementsRList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index == -1 && Theme.colors.Count > 0 || Theme.colors.Count == 0)
                    return;
                rect.y += 4;
                rect.height -= 8;
                DrawColorGroup(rect, Theme.colors[index], () =>
                {
                    if (EditMode)
                    {
                        elementsRList.index = index;
                        elementsRList.onSelectCallback.Invoke(elementsRList);
                    }
                    else
                    {
                        AssignColorGroup(Theme.colors[index]);
                    }
                });
            };
            elementsRList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Elements"); };
            elementsRList.onAddCallback = (ReorderableList list) =>
            {
                ColorGroup cg = new ColorGroup("New Element");
                cg.Add(new NamedColor("Normal", Color.white));
                cg.Add(new NamedColor("Highlighted", Color.white));
                cg.Add(new NamedColor("Pressed", Color.gray));
                cg.Add(new NamedColor("Disabled", Color.gray));
                Undo.RecordObject(Theme, "Element Added");
                Theme.colors.Add(cg);
                UpdateColorNames();
            };
            elementsRList.onRemoveCallback = (ReorderableList list) =>
            {
                Theme.colors.RemoveAt(list.index);
                SelectedCG = null;
                list.index = -1;
                UpdateColorNames();
            };
            elementsRList.onSelectCallback = (ReorderableList list) =>
            {
                SelectedCG = Theme.colors[list.index];
                GUI.FocusControl(null);
                UpdateColorNames();
            };
            elementsRList.onReorderCallback = (ReorderableList list) =>
            {
                UpdateColorNames();
            };
        }

        public static void DrawColorGroup(Rect rect, ColorGroup cg, Action onClick)
        {
            if (cg != null && cg.Count > 0)
                GUI.backgroundColor = StylesUI.borderColor;
            else
                GUI.backgroundColor = new Color(1, 1, 1, 0.5f);

            if (GUI.Button(rect, ""))
            {
                onClick();
            }
            GUI.backgroundColor = guiBGColor;

            if (cg.Count > 0)
            {

                rect.width -= 2;
                rect.height -= 2;
                rect.x += 1;
                rect.y += 1;
                if (cg.Count == 1)
                    EditorGUI.DrawRect(rect, cg[0].color);
                else
                {
                    Rect r = rect;
                    r.height = r.height * 0.7f;
                    EditorGUI.DrawRect(r, cg[0].color);
                    r.y += r.height;
                    r.height = rect.height * 0.3f;
                    r.width = r.width / (cg.Count - 1);
                    for (int i = 1; i < cg.Count; i++)
                    {
                        EditorGUI.DrawRect(r, cg[i].color);
                        r.x += r.width;
                    }
                }
            }
            GUIStyle textStyle = StylesUI.ColoredLabel;
            if (cg.Count > 0)
            {
                if (EditorHelper.IsColorDark(cg[0].color))
                    textStyle.normal.textColor = Color.white;
                else
                    textStyle.normal.textColor = Color.black;
            }
            else
                textStyle.normal.textColor = StylesUI.CenteredLabel.normal.textColor;

            rect.x += 2;
            rect.width -= 3;
            if (cg.Count > 1)
            {
                rect.y += 4f;
                rect.height = rect.height * 0.7f - 7f;
            }
            EditorGUI.LabelField(rect, cg.name, textStyle);
            GUI.color = guiColor;
        }

        public static void DrawColorGroupColor(NamedColor nc)
        {
            GUIStyle textStyle = StylesUI.NamedColor;
            guiBGColor = GUI.backgroundColor;
            GUI.backgroundColor = nc.color;
            GUILayout.Box("", textStyle);
            GUI.backgroundColor = guiBGColor;
        }

        public static void DrawColorGroupHorizontal(Rect rect, ColorGroup cg, Action onClick)
        {
            guiBGColor = GUI.backgroundColor;

            if (cg != null && cg.Count > 0)
                GUI.backgroundColor = StylesUI.borderColor;
            else
                GUI.backgroundColor = Color.clear;

            if (GUI.Button(rect, ""))
            {
                onClick();
            }
            GUI.backgroundColor = guiBGColor;

            if (cg.Count > 0)
            {

                rect.width -= 2;
                rect.height -= 2;
                rect.x += 1;
                rect.y += 1;
                if (cg.Count == 1)
                    EditorGUI.DrawRect(rect, cg[0].color);
                else
                {
                    Rect r = rect;
                    r.width = r.width * 0.7f;
                    EditorGUI.DrawRect(r, cg[0].color);
                    r.x += r.width;
                    r.width = rect.width * 0.3f;
                    r.width = r.width / (cg.Count - 1);
                    for (int i = 1; i < cg.Count; i++)
                    {
                        EditorGUI.DrawRect(r, cg[i].color);
                        r.x += r.width;
                    }
                }
            }
            GUIStyle textStyle = StylesUI.ColoredLabel;
            if (cg.Count > 0)
            {
                if (EditorHelper.IsColorDark(cg[0].color))
                    textStyle.normal.textColor = Color.white;
                else
                    textStyle.normal.textColor = Color.black;
            }
            else
                textStyle.normal.textColor = StylesUI.CenteredLabel.normal.textColor;
            rect.width *= 0.7f;
            rect.x += 5;
            EditorGUI.LabelField(rect, cg.name, textStyle);
            GUI.color = guiColor;
        }

        private void SetupColorGroupRList(ColorGroup cg)
        {
            if (cg == null)
                return;
            
            colorsList = new ReorderableList(cg.colorList, typeof(NamedColor), true, true, true, true);

            colorsList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Colors");
            };

            colorsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.width *= 0.5f;
                NamedColor nc = cg[index];
                string name = EditorGUI.TextField(rect, nc.Name);
                if (name != nc.Name)
                {
                    nc.Name = name;
                }
                rect.x += rect.width + 5;
                rect.width -= 35f;
                nc.color = EditorGUI.ColorField(rect, nc.color);
                rect.x += rect.width + 5;
                rect.y -= 2;
                rect.width = 30f;
                if (GUI.Button(rect, StylesUI.PaletteButton, StylesUI.IconPalette))
                {
                    ColorHelperPopupWindow.ShowPopup(nc.color, (c) => { nc.color = c; this.Repaint(); });
                }
            };
            colorsList.elementHeight = 16f;
            colorsList.onAddCallback = (ReorderableList r) =>
            {
                if (cg.Count > 0)
                    cg.Add(new NamedColor(cg[cg.Count - 1]));
                else
                    cg.Add(new NamedColor("New", Color.white));
            };
        }

        private void SetupPalleteList(ColorGroup cg)
        {
            if (cg == null)
                return;
            palleteList = new ReorderableList(cg.colorList, typeof(NamedColor));

            palleteList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "palleteList");
            };

            palleteList.elementHeight = 16f;
        }

        private void SetupElementEffectors(ColorGroup cg)
        {
            if (cg == null)
                return;

            elementModifierList = new ReorderableList(cg.effectors, typeof(Modifier));
            elementModifierList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Modifiers");
            };
            elementModifierList.elementHeight = 16f;
            elementModifierList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                cg.effectors[index] = (Modifier)EditorGUI.ObjectField(rect, cg.effectors[index], typeof(Modifier), false);
            };

            elementModifierList.onAddCallback = (ReorderableList r) =>
            {
                cg.effectors.Add(null);
            };
        }

        private void SetupColorGroupDescendantsRList(ColorGroup cg)
        {
            if (cg == null)
                return;

            elementDescendantsRList = new ReorderableList(cg.descendants, typeof(ColorGroupDescendant));
            elementDescendantsRList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "Nested Elements");
            };
            elementDescendantsRList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Action<ColorGroup> colorGroupSelected = (selectedCG) =>
                {
                    if (selectedCG != null)
                    {
                        cg.descendants[index].target = selectedCG.instanceId;
                        Repaint();
                    }
                };
                int colorIndex = Theme.colors.FindIndex((cg2) => cg.descendants[index].target.Value == cg2.instanceId.Value);
                rect.width -= 20;
                if (colorIndex == -1)
                {
                    if (GUI.Button(rect, "Select an Element"))
                    {
                        elementDescendantsRList.index = index;
                        ColorGroupPopupWindow.ShowPopup(null, Theme.colors, colorGroupSelected);
                    }
                }
                else
                {
                    ColorGroup child = Theme.colors[colorIndex];
                    DrawColorGroupHorizontal(rect, child, () =>
                    {
                        elementDescendantsRList.index = index;
                        ColorGroupPopupWindow.ShowPopup(child, Theme.colors, colorGroupSelected);
                    });
                }
            };
            elementDescendantsRList.onAddCallback = (ReorderableList r) =>
            {
                cg.descendants.Add(new ColorGroupDescendant(new SerializableGuid()));
            };
        }

        #endregion

        #region ColorList Management

        private void UpdateColorNames()
        {
            int i = 1;
            colorNames = Theme.colors.Select(x => i++ + ". " + x.name).ToArray();
        }

        #endregion

        #region Theme creating/Finding methods
        private void CreateTheme()
        {
            Theme = ScriptableObject.CreateInstance<GamestrapTheme>();

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Gamestrap UI/Basic Theme.asset");
            AssetDatabase.CreateAsset(Theme, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorPrefs.SetString(prefsKey, assetPathAndName);
        }
        #endregion

        #region Assign Color Group

        public void AssignColorGroup(ColorGroup colorGroup)
        {
            if (Selection.gameObjects.Length > 0)
            {
                foreach (GameObject g in Selection.gameObjects)
                {
                    #if UNITY_2018_3_OR_NEWER
                    var prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.activeGameObject);
                    #else
                    var prefab = PrefabUtility.FindPrefabRoot(Selection.activeGameObject);
                    #endif
                    if (prefab && AssetDatabase.Contains(prefab))
                    {
                        Theme.AssignColorGroup(colorGroup, prefab);
                        EditorUtility.SetDirty(prefab);
                    }
                    else
                    {
                        Theme.AssignColorGroup(colorGroup, g);
                        EditorUtility.SetDirty(g);
                    }
                }
            }
            else
            {
                var objects = SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject g in objects)
                {
                    Theme.AssignColorGroup(colorGroup, g);
                    EditorUtility.SetDirty(g);
                }
            }
        }

        #endregion

        #region Lazy loading Properties

        public string[] ColorNames
        {
            get
            {
                if (colorNames == null || colorNames.Length != Theme.colors.Count)
                {
                    UpdateColorNames();
                }
                return colorNames;
            }
        }

        public ColorGroup SelectedCG
        {
            get
            {
                return selectedElement;
            }

            set
            {
                if (value != selectedElement)
                {
                    SetupColorGroupRList(value);
                    SetupElementEffectors(value);
                    SetupColorGroupDescendantsRList(value);
                }
                selectedElement = value;
            }
        }

        public bool EditMode
        {
            get
            {
                return editMode;
            }

            set
            {
                editMode = value;
                EditorPrefs.SetBool(editModeKey, value);
            }
        }

        public GamestrapTheme Theme
        {
            get
            {
                if (theme == null && EditorPrefs.HasKey(prefsKey))
                {
                    string assetPath = EditorPrefs.GetString(prefsKey);
                    theme = AssetDatabase.LoadAssetAtPath<GamestrapTheme>(assetPath);
                }
                return theme;
            }
            set
            {
                if (Theme != value)
                {
                    theme = value;
                    if (value != null)
                    {
                        string assetPathAndName = AssetDatabase.GetAssetPath(Theme);
                        EditorPrefs.SetString(prefsKey, assetPathAndName);
                        SetupColorRList();
                    }
                }
            }
        }
        #endregion
    }
}