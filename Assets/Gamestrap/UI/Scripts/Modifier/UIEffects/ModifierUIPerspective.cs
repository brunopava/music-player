using UnityEngine;

namespace Gamestrap
{
    [CreateAssetMenu(fileName = "UI Perspective", menuName = "Gamestrap/Modifier/UI Effect/Perspective")]
    public class ModifierUIPerspective : ComponentModifier<PerspectiveEffect>
    {
        public float perspective;

        public override void Apply(PerspectiveEffect target)
        {
            target.perspective = perspective;
        }
    }
}