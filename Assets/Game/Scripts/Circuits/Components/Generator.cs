using SpiceSharp.Components;
using SpiceSharp.Entities;
using UnityEngine;

namespace Circuits.Components
{
    public class Generator : CircuitComponent
    {
        public override IEntity GetCircuitEntity() =>
            new VoltageSource(GetStringID(), positiveTerminal, negativeTerminal, 220.0);
    }
}