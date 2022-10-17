using UnityEditor;
using UnityEngine;
namespace Gamestrap
{
    public class StylesUI
    {
        private static GUISkin skin;
        public static GUISkin Skin{
            get {
                if (skin == null) {
                    skin = GraphicsLoader.GetSkin((EditorGUIUtility.isProSkin) ? "ui_skin_dark" : "ui_skin_light");
                    if (skin == null)
                        return new GUISkin();
                }
                return skin;
            }
        }

        public static Color borderColor = new Color(0.4f, 0.4f, 0.4f);

    #region Styles
        public static GUIStyle LeftColumn { get { return Skin.customStyles[0]; } }
        public static GUIStyle VerticalStretch { get { return Skin.customStyles[1]; } }
        public static GUIStyle TitleLabel { get { return Skin.customStyles[2]; } }
        public static GUIStyle CenteredLabel { get { return Skin.customStyles[3]; } }
        public static GUIStyle SmallLabel { get { return Skin.customStyles[4]; } }

        /// <summary>
        ///  Label that color will change in code ignoring Unity skin
        /// </summary>
        public static GUIStyle ColoredLabel { get { return Skin.customStyles[5]; } }
        public static GUIStyle Popup { get { return Skin.customStyles[6]; } }
        public static GUIStyle PopupWindow { get { return Skin.customStyles[7]; } }
        public static GUIStyle ColorButton { get { return Skin.customStyles[8]; } }
        public static GUIStyle IconButton { get { return Skin.customStyles[9]; } }
        public static GUIStyle IconPalette { get { return Skin.customStyles[10]; } }
        public static GUIStyle Icon { get { return Skin.customStyles[11]; } }
        public static GUIStyle NamedColor { get { return Skin.customStyles[12]; } }
        #endregion

        #region Content
        private static GUIContent editButton;
        public static GUIContent EditButton {
            get {
                if (editButton == null) {
                    editButton = new GUIContent(ColorGraphics.IconPen, "Edit Mode");
                }
                return editButton;
            }
        }

        private static GUIContent duplicateButton;
        public static GUIContent DuplicateButton {
            get {
                if (duplicateButton == null) {
                    duplicateButton = new GUIContent(ColorGraphics.IconDuplicate, "Duplicate");
                }
                return duplicateButton;
            }
        }

        private static GUIContent paletteButton;
        public static GUIContent PaletteButton {
            get {
                if (paletteButton == null) {
                    paletteButton = new GUIContent(ColorGraphics.IconPalette, "Color Palette Helper");
                }
                return paletteButton;
            }
        }

        private static GUIContent xButton;
        public static GUIContent XButton {
            get {
                if (xButton == null) {
                    xButton = new GUIContent(ColorGraphics.IconX, "Exit");
                }
                return xButton;
            }
        }

        private static GUIContent deleteTextButton;
        public static GUIContent DeleteTextButton {
            get {
                if (deleteTextButton == null) {
                    deleteTextButton = new GUIContent(ColorGraphics.IconX, "Clear");
                }
                return deleteTextButton;
            }
        }

        private static GUIContent helpButton;
        public static GUIContent HelpButton {
            get {
                if (helpButton == null) {
                    helpButton = new GUIContent(ColorGraphics.IconHelp, "Help");
                }
                return helpButton;
            }
        }
        #endregion
    }
}