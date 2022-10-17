using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Gamestrap
{
    [AddComponentMenu("UI/Gamestrap/Effects/Skew")]
    public class SkewEffect : GamestrapEffect
    {
        public float skew = 0f;

        public override void ModifyVerticesWrapper(List<UIVertex> vertexList)
        {
            if (skew != 0)
                ApplySkew(vertexList, 0, vertexList.Count);
        }

        public void ApplySkew(List<UIVertex> verts, int start, int end)
        {
            UIVertex vt;
            float bottomPos = verts.Min(t => t.position.y);
            float topPos = verts.Max(t => t.position.y);
            float height = topPos - bottomPos;
            for (int i = start; i < end; i++)
            {
                vt = verts[i];
                Vector3 v = vt.position;
                v.x += Mathf.Lerp(-skew, skew, (vt.position.y - bottomPos) / height);
                vt.position = v;

                verts[i] = vt;
            }
        }
    }
}
