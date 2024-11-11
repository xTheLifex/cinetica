using SpiceSharp.Components;
using SpiceSharp.Entities;
using UnityEngine;

namespace Circuits.Components
{
    public class CircuitComponent : MonoBehaviour
    {
        [Header("Terminals")]
        public string positiveTerminal, negativeTerminal;

        public string GetStringID() => gameObject.GetInstanceID().ToString();

        public virtual IEntity GetCircuitEntity() => new Resistor(GetStringID(), positiveTerminal, negativeTerminal, 1000.0);
    }
}