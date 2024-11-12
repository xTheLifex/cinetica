using Circuits.Components;
using Circuits.Utility;
using UnityEngine;

namespace Circuits
{
    public class CircuitController : Singleton<CircuitController>
    {
        public static Generator ActiveGenerator { get; private set; }
        private Utility.Logger _logger = new Utility.Logger("Circuit Controller");
        public static CircuitComponent[] AllComponents() => GameObject.FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None);
        public override void Awake()
        {
            base.Awake();
            ActiveGenerator = GameObject.FindFirstObjectByType<Generator>();
            if (!ActiveGenerator)
            {
                _logger.LogError("Failed to find generator.");
                return;
            }
        }

        public void Update()
        {
            UpdateCircuit();
        }
        public void UpdateCircuit()
        {
            var all = AllComponents();
            foreach (var c in all) if (!c.IsGenerator()) c.v = 0f;
            
            ActiveGenerator.EquivalentResistance = ActiveGenerator.GetEquivalentResistance();
            ActiveGenerator.i = ActiveGenerator.GetCurrent();
            foreach (var con in ActiveGenerator.connections)
            {
                con.SetCurrent(ActiveGenerator.GetCurrent());
            }
        }
    }
}