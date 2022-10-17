using UnityEngine;
using UnityEngine.UI;

namespace Gamestrap
{
    [AddComponentMenu("UI/Gamestrap/Effects/Shadow")]
    public class ShadowEffect : Shadow, IModifier
    {
        /**
         * This class was made so Gamestrap could actually instantiate to the inspector the shadow effect
         * since it doesn't let me do it for the original Shadow effect.
         **/
    }
}
