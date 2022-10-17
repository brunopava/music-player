using UnityEngine;
using UnityEngine.Events;

namespace Gamestrap
{
    [System.Serializable]
    public class NamedColor
    {
        [SerializeField]
        private string name;

        public Color color;
        public event UnityAction OnNameChange;

        public NamedColor(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }
        
        public NamedColor(NamedColor namedColor) : this(namedColor.name, namedColor.color)
        {
            
        }

        public string Name {
            get {
                return name;
            }
            set {
                if (value != name) {
                    this.name = value;
                    if (OnNameChange != null)
                        OnNameChange();
                }
            }
        }
    }

}
