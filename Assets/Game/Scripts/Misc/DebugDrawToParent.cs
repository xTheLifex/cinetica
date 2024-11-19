using UnityEngine;

namespace Cinetica.Utility
{
    public class DebugDrawToParent : MonoBehaviour
    {
        #if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.parent.position);
        }
#endif
    }
}