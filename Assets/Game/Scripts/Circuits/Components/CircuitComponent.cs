using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Circuits.Components
{
    public class CircuitComponent : MonoBehaviour
    {
        [Header("Circuit Component")]
        public List<CircuitComponent> connections = new List<CircuitComponent>();
        [FormerlySerializedAs("resistance")] [InspectorName("Resistance")] public float r = 0f;
        [FormerlySerializedAs("voltage")] [InspectorName("Voltage")] public float v = 0f;
        [FormerlySerializedAs("current")] [InspectorName("Current")] public float i = 0f;
        public bool merging = false;
        [FormerlySerializedAs("EquivalentResitance")] public float EquivalentResistance = 0f;
        public CircuitComponent[] FindPrevious() => CircuitController.AllComponents().Where(x => x.connections.Contains(this)).ToArray();
        
        public virtual bool IsGenerator() => false;
        public virtual bool IsSwitch() => false;
        public virtual bool IsNode() => false;

        public virtual bool HasConnectionTo(CircuitComponent comp) => connections.Contains(comp);
        
        // Checks if there’s a path to the given component
        public virtual bool HasPathTo(CircuitComponent comp)
        {
            if (comp == this) return true;
            if (comp.IsSwitch() && ((Switch)comp).open) return false;
            foreach (var con in connections)
                if (con.HasPathTo(comp)) return true;
            return false;
        }

        public virtual bool HasDirectPathTo(CircuitComponent comp) => (comp.connections.Count <= 1) && HasPathTo(comp);
        public virtual float GetCurrent() => i;
        public virtual float GetVoltage() => v;
        
        public void SetCurrent(float newCurrent)
        {
            i = newCurrent;
            v = IsNode() ? EquivalentResistance * i : r * i;
            foreach (var con in connections)
            {
                if (con.IsGenerator()) continue;
                if (connections.Count <= 1)
                    con.SetCurrent(i);
                 else
                    con.SetVoltage(v);
            }
        }

        public void SetVoltage(float newVoltage)
        {
            v = newVoltage;
            i = v / (IsNode() ? EquivalentResistance : r);
            foreach (var con in connections)
            {
                if (con.IsGenerator()) continue;
                if (connections.Count <= 1)
                    con.SetCurrent(i);
                else
                    con.SetVoltage(v);
            }            
        }
        
        public virtual void Awake() {}

        // Calculates equivalent resistance for this component.
        public virtual float GetEquivalentResistance()
        {
            float sumInv = 0f;
            foreach (var con in connections)
            {
                // Skip generators and components not connected to the active generator
                if (con.IsGenerator()) continue;
                if (!IsNode() && con.merging) continue;
                if (!con.HasPathTo(CircuitController.ActiveGenerator)) continue;

                float r = con.GetEquivalentResistance();
                if (r > 0f)
                    sumInv += 1 / r; // Add the inverse resistance (conductance)
            }

            // Calculate the equivalent resistance of the parallel network
            float req = sumInv > 0f ? 1 / sumInv : 0f;

            // Return total resistance including the base component's resistance
            EquivalentResistance = r + req; // Debug, TODO: Delete
            return r + req;
        }

        #if UNITY_EDITOR
        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            foreach(var con in connections)
            {
                Gizmos.DrawLine(transform.position, con.transform.position);
            }
        }

        public virtual void OnDrawGizmosSelected()
        {
            foreach(var con in connections)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(con.transform.position, 1f);
                foreach (var second in con.connections)
                {
                    Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.5f);
                    Gizmos.DrawWireSphere(second.transform.position, 1f);
                }
            }
        }
#endif
    }
}