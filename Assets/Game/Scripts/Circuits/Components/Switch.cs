using UnityEngine;

namespace Circuits.Components
{
    public class Switch : CircuitComponent
    {
        [Header("Switch")]
        public bool open = false;
        public override bool IsSwitch() => true;

        public override bool HasPathTo(CircuitComponent comp)
        {
            if (open) return false;
            return base.HasPathTo(comp);
        }
    }
}