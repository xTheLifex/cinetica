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

        public override float GetVoltage()
        {
            foreach (var con in connections)
            {
                v = con.GetVoltage();
                return v;
            }

            return 0f;
        }

        public override float GetCurrent()
        {
            foreach (var con in connections)
            {
                i = con.GetCurrent();
                return i;
            }

            return 0f;
        }
    }
}