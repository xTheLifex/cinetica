using System;
using UnityEngine;

namespace Circuits
{
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

        private void Touch(Vector3 pos)
        {
            #if UNITY_EDITOR
            Debug.DrawRay(pos, pos + Vector3.up * 10f, Color.magenta);
            #endif
        }
        
        private void TouchStart(Vector3 pos)
        {
            #if UNITY_EDITOR
            Debug.Log("Touch Start on  " + pos);
            Debug.DrawRay(pos, pos + Vector3.up * 10f, Color.green, 1f);
            #endif
        }

        private void TouchEnd(Vector3 pos)
        {
            #if UNITY_EDITOR
            Debug.Log("Touch End on  " + pos);
            Debug.DrawRay(pos, pos + Vector3.up * 10f, Color.red, 1f);
            #endif
        }
    }
}