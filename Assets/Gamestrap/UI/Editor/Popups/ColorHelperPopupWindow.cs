using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Gamestrap
{

    public class ColorHelperPopupWindow : EditorWindow
    {
        private readonly string editorSaveKey = "Gamestrap.ColorWindow";
        private readonly int historySlotCount = 10;
        private static string[] colorPickModes = { "Random", "Schemes", "Scene Colors" };
        private static Color[] colors = {
            GamestrapUIHelper.ColorRGBInt(74, 37, 68),
            GamestrapUIHelper.ColorRGBInt(206, 20, 90),
            GamestrapUIHelper.ColorRGBInt(141, 39, 137),
            GamestrapUIHelper.ColorRGBInt(37, 82, 102),
            GamestrapUIHelper.ColorRGBInt(41, 165, 220),
            GamestrapUIHelper.ColorRGBInt(126, 209, 232),
            GamestrapUIHelper.ColorRGBInt(54, 148, 104),
            GamestrapUIHelper.ColorRGBInt(134, 192, 63),
            GamestrapUIHelper.ColorRGBInt(211, 218, 33),
            GamestrapUIHelper.ColorRGBInt(255, 204, 0),
            GamestrapUIHelper.ColorRGBInt(255, 153, 0),
            GamestrapUIHelper.ColorRGBInt(255, 173, 67),
            GamestrapUIHelper.ColorRGBInt(242, 110, 37),
            GamestrapUIHelper.ColorRGBInt(255, 102, 0),
            GamestrapUIHelper.ColorRGBInt(239, 106, 65),
            GamestrapUIHelper.ColorRGBInt(230, 36, 45),
            GamestrapUIHelper.ColorRGBInt(137, 24, 16),
            GamestrapUIHelper.ColorRGBInt(239, 101, 101),
            GamestrapUIHelper.ColorRGBInt(134, 98, 57),
            GamestrapUIHelper.ColorRGBInt(91, 54, 21),
            GamestrapUIHelper.ColorRGBInt(192, 150, 109),
            GamestrapUIHelper.ColorRGBInt(200, 200, 200),
            GamestrapUIHelper.ColorRGBInt(128, 128, 128),
            GamestrapUIHelper.ColorRGBInt(51, 51, 51),
            GamestrapUIHelper.ColorRGBInt(21, 21, 21)
        };
        private static ColorHelperPopupWindow instance;
        private static int colorPickMode = 0;
        //private static SchemeType schemeType;

        private Color selectedColor;
        private Color baseSchemeColor;
        private List<Color> sceneColors = new List<Color>();
        private Color bgColor;
        private System.Action<Color> selectedColorCallback;
        private List<Color> lastUsedColors;

        public static void ShowPopup(Color baseColor, System.Action<Color> selectedColorCallback)
        {
            if (instance == null) {
                instance = ScriptableObject.CreateInstance<ColorHelperPopupWindow>();
                instance.wantsMouseMove = true;
            }
            instance.selectedColor = baseColor;
            instance.baseSchemeColor = baseColor;
            instance.selectedColorCallback = selectedColorCallback;
            Vector2 realPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            instance.position = new Rect(realPosition.x, realPosition.y, 310, 181);
            instance.ShowPopup();
            instance.Focus();
        }

        void OnEnable()
        {
            bgColor = GUI.backgroundColor;
            GetSceneColors();
        }

        private void OnLostFocus()
        {
            AssignColor(selectedColor);
        }

        void OnGUI()
        {
            GUILayout.BeginVertical(StylesUI.PopupWindow);
            GUILayout.BeginHorizontal();
            colorPickMode = GUILayout.Toolbar(colorPickMode, colorPickModes);
            if (GUILayout.Button("X", GUILayout.Width(20f))) {
                AssignColor(selectedColor);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(StylesUI.VerticalStretch);
            switch (colorPickMode) {
                case 0: OnShowSuggestions(); break;
                case 1: OnShowScheme(); break;
                case 2: OnShowSceneColors(); break;
            }
            GUILayout.EndVertical();
            GUILayout.Label("Color History", StylesUI.SmallLabel);
            GUILayout.BeginHorizontal();
            for(int i = 0; i < LastUsedColors.Count; i++) {
                GUI.backgroundColor = LastUsedColors[i];
                if (GUILayout.Button("", StylesUI.ColorButton)) {
                    AssignColor(LastUsedColors[i], true);
                }
            }
            GUI.backgroundColor = bgColor;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }

        private void OnShowSuggestions()
        {
            GUILayout.BeginHorizontal();
            int counter = 0;
            foreach (Color color in colors) {
                GUI.backgroundColor = color; // Sets the button color
                if (GUILayout.Button("", StylesUI.ColorButton)) {
                    AssignColor(color, true);
                }
                counter++;
                if (counter % 5 == 0) {
                    // Start a new row each 5
                    GUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.black;
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndHorizontal();
            GUI.backgroundColor = bgColor; // Resets the color background
        }

        private void OnShowScheme()
        {
            foreach (SchemeType schemeType in Enum.GetValues(typeof(SchemeType))) {
                Color[] paletteColors = GamestrapUIHelper.GetColorPalette(baseSchemeColor, schemeType);
                GUILayout.BeginHorizontal();
                GUILayout.Label(Regex.Replace(schemeType.ToString(), @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0"), GUILayout.Width(128));
                for (int i = 0; i < paletteColors.Length; i++) {
                    GUI.backgroundColor = paletteColors[i]; // Sets the button color
                    if (GUILayout.Button("", StylesUI.ColorButton)) {
                        AssignColor(paletteColors[i], true);
                    }
                }
                GUI.backgroundColor = bgColor; // Resets the color background
                GUILayout.EndHorizontal();
            }
        }

        private void OnShowSceneColors()
        {
            GUILayout.BeginHorizontal();
            int counter = 0;
            foreach (Color color in sceneColors) {
                GUI.backgroundColor = color; // Sets the button color
                if (GUILayout.Button("", StylesUI.ColorButton)) {
                    AssignColor(color, true);
                }
                counter++;
                if (counter % 5 == 0) {
                    // Start a new row each 5
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUI.backgroundColor = bgColor;
            GUILayout.EndHorizontal();
        }

        public void AssignColor(Color color, bool save = false)
        {
            if (selectedColorCallback != null)
                selectedColorCallback(color);
            if (save) {
                if (LastUsedColors.Contains(color)) {
                    LastUsedColors.Remove(color);
                }
                LastUsedColors.Insert(0, color);
                if (LastUsedColors.Count > historySlotCount) {
                    LastUsedColors.RemoveRange(9, LastUsedColors.Count - historySlotCount);
                }
                EditorPrefs.SetString(editorSaveKey, JsonUtility.ToJson(new ColorList(LastUsedColors)));
            }
            Close();
        }

        #region Scene Color Methods
        private void SearchSceneColors()
        {
            sceneColors.Clear();
            GetSceneColors();
        }

        public void GetSceneColors()
        {
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
            foreach (var root in GamestrapUIHelper.GetSceneGameObjectRoots())
#else
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
#endif
            {
                SearchColorsGameObject(root);
            }
        }

        private void SearchColorsGameObject(GameObject gameObject)
        {
            if (gameObject.GetComponent<UnityEngine.UI.Text>()) {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Text>().color);
            }
            if (gameObject.GetComponent<UnityEngine.UI.Image>()) {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Image>().color);
            }
            if (gameObject.GetComponent<UnityEngine.UI.Button>()) {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Button>());
            }
            if (gameObject.GetComponent<UnityEngine.UI.Toggle>()) {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Toggle>());
            }
            if (gameObject.GetComponent<UnityEngine.UI.Slider>()) {
                AddSceneColor(gameObject.GetComponent<UnityEngine.UI.Slider>());
            }

            if (gameObject.transform.childCount > 0) {
                for (int i = 0; i < gameObject.transform.childCount; i++) {
                    SearchColorsGameObject(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }

        private void AddSceneColor(Color color)
        {
            if (!sceneColors.Contains(color)) {
                sceneColors.Add(color);
            }
        }

        private void AddSceneColor(UnityEngine.UI.Selectable selectable)
        {
            UnityEngine.UI.ColorBlock colorBlock = selectable.colors;
            AddSceneColor(colorBlock.normalColor);
            // Uncomment if you want to include additional colorBlock colors
            //AddSceneColor(colorBlock.highlightedColor);
            //AddSceneColor(colorBlock.pressedColor);
            //AddSceneColor(colorBlock.disabledColor);
        }
        #endregion

        #region Properties

        public List<Color> LastUsedColors {
            get {
                if (lastUsedColors == null) {
                    lastUsedColors = new List<Color>();
                    if (EditorPrefs.HasKey(editorSaveKey)) {
                        lastUsedColors = JsonUtility.FromJson<ColorList>(EditorPrefs.GetString(editorSaveKey)).colors;
                    }
                }
                return lastUsedColors;
            }
        }
        #endregion

        private class ColorList
        {
            public List<Color> colors;

            public ColorList(List<Color> colors)
            {
                this.colors = colors;
            }
        }
    }
}
