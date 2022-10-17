using UnityEngine;

namespace Gamestrap
{
    [CreateAssetMenu(fileName = "UI RadialGradient", menuName = "Gamestrap/Modifier/UI Effect/Radial Gradient")]
    public class ModifierUIRadialGradient : ComponentModifier<RadialGradientEffect>
    {
        public Vector2 centerPosition;
        public float radius;
        public Color innerColor;
        public Color outterColor;

        public override void Apply(RadialGradientEffect target)
        {
            target.centerPosition = centerPosition;
            target.radius = radius;
            target.innerColor = innerColor;
            target.outterColor = outterColor;
        }
    }
}