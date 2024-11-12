
using System.Linq;
using UnityEngine;

namespace Circuits.Components
{
    public class Generator : CircuitComponent
    {
        public override bool IsGenerator() => true;
        public override float GetCurrent() => v / GetEquivalentResistance();
        public override float GetEquivalentResistance()
        {
            var comps = GameObject.FindObjectsByType<Node>(FindObjectsSortMode.None).Where(x => x.primary == true).ToList();
            return comps.Sum(x => x.GetEquivalentResistance());
        }
    }
}