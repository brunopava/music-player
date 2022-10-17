using UnityEngine;

namespace Gamestrap
{
    [CreateAssetMenu(fileName = "UI Gradient", menuName = "Gamestrap/Modifier/UI Effect/Gradient")]
    public class ModifierUIGradient : ComponentModifier<GradientEffect>
    {
        public Color top;
        public Color bottom;

        public override void Apply(GradientEffect target)
        {
            target.top = top;
            target.bottom = bottom;
        }
    }
}