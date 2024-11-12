
using UnityEngine;

namespace Circuits.Components
{
    public class Generator : CircuitComponent
    {
        [Header("Generator")]
        public float voltage = 220;//Volts
        public override bool IsGenerator() => true;
    }
}