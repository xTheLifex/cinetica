using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Cinetica.Utility
{
    public static class Utils
    {
        public static bool SceneExists(string name) => SceneUtility.GetBuildIndexByScenePath(name) >= 0;
        
        public static T NextOf<T>(this List<T> list, T obj)
        {
	        int index = list.IndexOf(obj);
	        if (index == -1 || index == list.Count - 1)
		        return list.FirstOrDefault(); // If not found or last element, return first element (circular behavior)
        
	        return list[index + 1];
        }

        public static T PreviousOf<T>(this List<T> list, T obj)
        {
	        int index = list.IndexOf(obj);
	        if (index == -1 || index == 0)
		        return list.LastOrDefault(); // If not found or first element, return last element (circular behavior)
        
	        return list[index - 1];
        }
        
        public static T Pick<T>(T[] elements)
        {
	        int index = Random.Range(0, elements.Length);
	        return elements[index];
        }
        
        public static T PickRandom<T>(this T[] elements)
        {
	        int index = Random.Range(0, elements.Length);
	        return elements[index];
        }
        
        /// <summary>
        /// Smoothly rotates the transform to look at the target position with a lerp effect.
        /// </summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="target">The target position to look at.</param>
        /// <param name="lookSpeed">The speed of the look rotation.</param>
        public static void LookAtLerp(this Transform transform, Vector3 target, float lookSpeed)
        {
	        // Calculate the direction to the target
	        Vector3 directionToTarget = (target - transform.position).normalized;
        
	        // Interpolate between the current forward direction and the target direction
	        Vector3 lerpedDirection = Vector3.Lerp(transform.forward, directionToTarget, Time.deltaTime * lookSpeed);
        
	        // Apply the new direction
	        transform.rotation = Quaternion.LookRotation(lerpedDirection);
        }

        /// <summary>
        /// Overload that accepts a Transform as the target.
        /// </summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="target">The target Transform to look at.</param>
        /// <param name="lookSpeed">The speed of the look rotation.</param>
        public static void LookAtLerp(this Transform transform, Transform target, float lookSpeed)
        {
	        LookAtLerp(transform, target.position, lookSpeed);
        }
    }
    
#if UNITY_EDITOR
	public static class DrawArrow
	{
		public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Gizmos.DrawRay(pos, direction);
			
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
			Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
		}

		public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Gizmos.color = color;
			Gizmos.DrawRay(pos, direction);
			
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
			Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
			Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
		}

		public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Debug.DrawRay(pos, direction);
			
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
			Debug.DrawRay(pos + direction, right * arrowHeadLength);
			Debug.DrawRay(pos + direction, left * arrowHeadLength);
		}
		public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
		{
			Debug.DrawRay(pos, direction, color);
			
			Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
			Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
			Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
			Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
		}
	}
#endif
}
