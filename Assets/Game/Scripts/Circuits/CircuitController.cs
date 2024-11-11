using System;
using System.Collections.Generic;
using SpiceSharp;
using SpiceSharp.Components;
using UnityEngine;
using Circuits.Components;
using SpiceSharp.Simulations;
using static Circuits.Utility.Utils;

namespace Circuits
{
    public class CircuitController : MonoBehaviour
    {
        public Generator generator;
        private Circuit _circuit;
        private DC _simulation;
        private readonly Circuits.Utility.Logger _logger = new Circuits.Utility.Logger("Circuit Controller");
        private void Awake()
        {
            RecalculateCircuit();
        }

        public bool ValidateCircuit()
        {
            // TODO: See if it loops around. This shouldnt be needed since the player doesnt make his own circuits
            // but just in case i fuck up this should debug enough.
            return true;
        }

        public void RecalculateCircuit()
        {
            if (generator == null)
            {
                _logger.LogError("Failed to find main generator for circuit");
                return;
            }
            
            CircuitComponent[] components = GameObject.FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None);

            foreach (var comp in components)
            {
                _circuit.Add(comp.GetCircuitEntity());
            }
        }
        
        #if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            CircuitComponent[] components = GameObject.FindObjectsByType<CircuitComponent>(FindObjectsSortMode.None);
            foreach (var comp in components)
            {
                foreach (var other in components)
                {
                    if (comp == other) continue;
                    if (comp.positiveTerminal == "") continue;
                    if (other.negativeTerminal == "") continue;
                    if (comp.positiveTerminal == other.negativeTerminal)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawLine(comp.transform.position, other.transform.position);
                    }
                }
            }
        }
#endif
    }
}