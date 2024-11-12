using Circuits.Components;
using Circuits.Utility;
using UnityEngine;

namespace Circuits
{
    public class CircuitController : Singleton<CircuitController>
    {
        public static Generator ActiveGenerator { get; private set; }
        private Utility.Logger _logger = new Utility.Logger("Circuit Controller");

        public override void Awake()
        {
            base.Awake();
            ActiveGenerator = GameObject.FindFirstObjectByType<Generator>();
            if (!ActiveGenerator)
            {
                _logger.LogError("Failed to find generator.");
                return;
            }

            string s = "";
            foreach (var c in GameObject.FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None))
            {
                s += $"{c.gameObject.name}: {c.GetEquivalentResistance()}\n";
            }

            _logger.Log(s);
            _logger.Log($"GENERATOR: {ActiveGenerator.GetEquivalentResistance()}");
        }
    }
}