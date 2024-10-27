using System;
using UnityEngine;

namespace Circuits
{
    // TODO: Make movement independent on world positon, move entirely using screen delta.
    
    public class PlayerControl : MonoBehaviour
    {
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
            Debug.DrawRay(camPos, (pos - camPos).normalized * 64f, Color.magenta);
            #endif
        }
        
        private void TouchStart(Vector3 screenPos, Vector3 pos)
        {
            #if UNITY_EDITOR
            //if (Physics.Raycast(transform.position, dir - transform.position, out RaycastHit hit))
            var camPos = Camera.main!.transform.position;
            Debug.DrawRay(camPos, (pos - camPos).normalized * 64f, Color.green);
            #endif
        }

        private void TouchEnd(Vector3 screenPos, Vector3 pos)
        {
            #if UNITY_EDITOR
            //if (Physics.Raycast(transform.position, dir - transform.position, out RaycastHit hit))
            var camPos = Camera.main!.transform.position;
            Debug.DrawRay(camPos, (pos - camPos).normalized * 64f, Color.red);
            #endif
        }
    }
}