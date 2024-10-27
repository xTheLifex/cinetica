using System;
using UnityEngine;

namespace Circuits
{
    // TODO: Make movement independent on world positon, move entirely using screen delta.
    
    public class PlayerControl : MonoBehaviour
    {
        private Plane _movementPlane;
        private bool _moving = false;

        private void Start()
        {
            _movementPlane = new Plane(Vector3.up, 0f);
        }

        public void OnEnable()
        {
            GameManager.OnTouch.AddListener(Touch);
            GameManager.OnTouchStart.AddListener(TouchStart);
            GameManager.OnTouchEnd.AddListener(TouchEnd);
        }

        public void OnDisable()
        {
            GameManager.OnTouch.RemoveListener(Touch);
            GameManager.OnTouchStart.RemoveListener(TouchStart);
            GameManager.OnTouchEnd.RemoveListener(TouchEnd);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
        #endif

        private void Touch(Vector3 screenPos, Vector3 pos)
        {
            #if UNITY_EDITOR
            //if (Physics.Raycast(transform.position, dir - transform.position, out RaycastHit hit))
            var camPos = Camera.main!.transform.position;
            Debug.DrawRay(camPos, (pos - camPos).normalized * 8f, Color.magenta);
            #endif
            
            Vector3 dir = (pos - camPos).normalized;
            Ray ray = new Ray(camPos, dir);
            if (_movementPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Debug.DrawLine(hitPoint, hitPoint + Vector3.up * 8f, Color.green);
            }
        }
        
        private void TouchStart(Vector3 screenPos, Vector3 pos)
        {
            var camPos = Camera.main!.transform.position;
            #if UNITY_EDITOR
            Debug.DrawRay(camPos, (pos - camPos).normalized * 8f, Color.green);
            #endif
            
            Vector3 dir = (pos - camPos).normalized;
            Ray ray = new Ray(camPos, dir);
            if (_movementPlane.Raycast(ray, out float enter))
            {
                // Hit point in the movement plane.
                Vector3 hitPoint = ray.GetPoint(enter);
                Debug.DrawLine(hitPoint, hitPoint + Vector3.up * 8f, Color.green);
                
                _moving = true;
            }
        }

        private void TouchEnd(Vector3 screenPos, Vector3 pos)
        {
            _moving = false;
        }

        private void OnTap(Vector3 screenPos, Vector3 pos)
        {
            
        }
    }
}