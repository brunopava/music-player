using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gamestrap
{
    [System.Serializable]
    public class ColorGroupDescendant
    {

        public SerializableGuid target;

        public ColorGroupDescendant(SerializableGuid target)
        {
            this.target = target;
        }

        public ColorGroupDescendant(ColorGroupDescendant cgDescendant) 
            : this(cgDescendant.target)
        {

        }

    }

}
