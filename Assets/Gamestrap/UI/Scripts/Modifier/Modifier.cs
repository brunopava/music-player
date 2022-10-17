using UnityEngine;

namespace Gamestrap
{
    public abstract class Modifier : ScriptableObject {

        public abstract void Apply(GameObject target);
    }
}
