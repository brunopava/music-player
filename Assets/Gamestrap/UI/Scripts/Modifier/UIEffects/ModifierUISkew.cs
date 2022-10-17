using UnityEngine;

namespace Gamestrap
{
    [CreateAssetMenu(fileName = "UI Skew", menuName = "Gamestrap/Modifier/UI Effect/Skew")]
    public class ModifierUISkew : ComponentModifier<SkewEffect>
    {
        public float skew;

        public override void Apply(SkewEffect target)
        {
            target.skew = skew;
        }
    }
}