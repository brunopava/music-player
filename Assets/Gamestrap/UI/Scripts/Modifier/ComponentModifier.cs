using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamestrap
{
    public abstract class ComponentModifier<T> : Modifier where T : Component
    {
        public T Component { get; set; }

        public override void Apply(GameObject target)
        {
            Component = target.GetComponent<T>();
            if (!Component)
            {
                Component = target.AddComponent<T>();
            }
            if (Component)
                Apply(Component);
        }

        public abstract void Apply(T component);
    }
}