using UnityEngine;

namespace Circuits.Components
{
    public class Node : CircuitComponent
    {
        public override bool IsNode() => true;
        public bool primary = false;

        public override void Awake()
        {
            EquivalentResistance = GetEquivalentResistance();
        }
        
        #if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        }
        #endif
    }
}