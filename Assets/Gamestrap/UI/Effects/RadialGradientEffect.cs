using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamestrap
{
    [AddComponentMenu("UI/Gamestrap/Effects/Radial Gradient Effect")]
    public class RadialGradientEffect : GamestrapEffect
    {
        public Vector2 centerPosition;
        public float radius;
        public Color innerColor = Color.white;
        public Color outterColor = Color.white;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position + (Vector3)centerPosition, 2f);
            #if UNITY_EDITOR
            Handles.DrawWireDisc(transform.position + (Vector3)centerPosition, Vector3.forward, radius);
            #endif
        }

        public override void ModifyVerticesWrapper(List<UIVertex> vertexList)
        {
            if (!IsActive() || vertexList.Count < 4)
            {
                return;
            }

            if (radius == 0)
            {
                radius = 1;
            }
            for (int i = 0; i < vertexList.Count; i++)
            {
                UIVertex v = vertexList[i];

                v.color *= Color.Lerp(innerColor, outterColor, Mathf.Clamp01(((Vector2)v.position - centerPosition).magnitude / radius));
                vertexList[i] = v;
            }
        }

    }
}