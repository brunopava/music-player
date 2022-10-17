using UnityEngine;

namespace Gamestrap
{
    [CreateAssetMenu(fileName = "UI Shadow", menuName = "Gamestrap/Modifier/UI Effect/Shadow")]
    public class ModifierUIShadow : ComponentModifier<ShadowEffect>
    {
        public Color color;
        public Vector2 distance;

        public override void Apply(ShadowEffect target)
        {
            target.effectColor = color;
            target.effectDistance = distance;
        }
    }
}