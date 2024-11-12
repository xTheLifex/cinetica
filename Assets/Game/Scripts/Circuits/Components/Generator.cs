
using System.Linq;
using UnityEngine;

namespace Circuits.Components
{
    public class Generator : CircuitComponent
    {
        [Header("Generator")]
        public float voltage = 220;//Volts
        public override bool IsGenerator() => true;

        public override float GetEquivalentResistance()
        {
            var comps = GameObject.FindObjectsByType<Node>(FindObjectsSortMode.None).Where(x => x.primary == true).ToList();
            return comps.Sum(x => x.GetEquivalentResistance());
        }
    }
}