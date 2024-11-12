
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
            var comps = GameObject.FindObjectsByType<Node>(FindObjectsSortMode.None);
            float req = 0f;
            foreach (var c in comps)
            {
                req += c.GetEquivalentResistance();
            }

            return req;
        }
    }
}