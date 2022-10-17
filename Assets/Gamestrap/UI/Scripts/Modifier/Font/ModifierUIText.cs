using UnityEngine;
using UnityEngine.UI;

namespace Gamestrap
{
    [CreateAssetMenu(fileName = "UI Text", menuName = "Gamestrap/Modifier/Text Override")]
    public class ModifierUIText : ComponentModifier<Text>
    {
        public Font font;

        [Tooltip("By checking this the modifier will apply all of the following attributes")]
        public bool overrideValues;
        public Color color = Color.black;
        public FontStyle fontStyle;
        public int fontSize;
        public float lineSpacing;
        public bool richText;
        public TextAlignment alignment;
        public VerticalWrapMode verticalOverflow;
        public HorizontalWrapMode horizontalOverflow;

        public bool resizeTextForBestFit;
        public int resizeTextMinSize;
        public int resizeTextMaxSize;

        public bool raycastTarget;

        public override void Apply(Text target)
        {
            target.font = font;

            if (!overrideValues)
                return;

            target.fontStyle = fontStyle;
            target.fontSize = fontSize;
            target.lineSpacing = lineSpacing;
            target.supportRichText = richText;
            target.verticalOverflow = verticalOverflow;
            target.horizontalOverflow = horizontalOverflow;
            target.color = color;

            target.resizeTextForBestFit = resizeTextForBestFit;
            target.resizeTextMinSize = resizeTextMinSize;
            target.resizeTextMaxSize = resizeTextMaxSize;

            target.raycastTarget = raycastTarget;

        }
    }
}