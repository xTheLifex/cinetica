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
            
            _logger.Log($"REQ: {ActiveGenerator.GetEquivalentResistance()}");
        }
    }
}