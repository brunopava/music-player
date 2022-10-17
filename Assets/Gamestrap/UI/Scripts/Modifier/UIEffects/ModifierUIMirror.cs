using UnityEngine;

namespace Gamestrap
{
    [CreateAssetMenu(fileName = "UI Mirror", menuName = "Gamestrap/Modifier/UI Effect/Mirror")]
    public class ModifierUIMirror : ComponentModifier<MirrorEffect>
    {
        public float scale;
        public Vector2 offset;
        public Color top;
        public Color bottom;

        public override void Apply(MirrorEffect target)
        {
            target.scale = scale;
            target.offset = offset;
            target.top = top;
            target.bottom = bottom;
        }
    }
}