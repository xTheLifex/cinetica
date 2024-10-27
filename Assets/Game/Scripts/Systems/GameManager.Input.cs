using Circuits.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Circuits
{
    public partial class GameManager : Singleton<GameManager>
    {
        [Header("Input")]
        public bool inputEnabled = true;
        public bool movementEnabled = true;
        public bool interactionEnabled = true;
        
        public static UnityEvent<Vector3, Vector3> OnTouch = new UnityEvent<Vector3, Vector3>();
        public static UnityEvent<Vector3, Vector3> OnTouchStart = new UnityEvent<Vector3, Vector3>();
        public static UnityEvent<Vector3, Vector3> OnTouchEnd = new UnityEvent<Vector3, Vector3>();
        
        private InputAction _touchPosition;
        private InputAction _touchPress;
        
        private bool _touching = false;
        private Vector3 _lastTouchPos = Vector3.zero;
        private void InitializeInput()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();

            EnhancedTouch.Touch.onFingerDown += FingerDown;
            EnhancedTouch.Touch.onFingerUp += FingerUp;
            EnhancedTouch.Touch.onFingerMove += FingerMove;
        }

        private Vector3 ComposeFingerPosVector(Vector2 fingerPos)
        {
            return new Vector3(fingerPos.x, fingerPos.y, Camera.main!.farClipPlane);
        }

        private void FingerMove(Finger finger)
        {
            if (!Camera.main) return;
            OnTouch?.Invoke(finger.screenPosition, Camera.main.ScreenToWorldPoint(ComposeFingerPosVector(finger.screenPosition)));
        }

        private void FingerUp(Finger finger)
        {
            if (!Camera.main) return;
            OnTouchEnd?.Invoke(finger.screenPosition, Camera.main.ScreenToWorldPoint(ComposeFingerPosVector(finger.screenPosition)));
        }

        private void FingerDown(Finger finger)
        {
            if (!Camera.main) return;
            OnTouchStart?.Invoke(finger.screenPosition, Camera.main.ScreenToWorldPoint(ComposeFingerPosVector(finger.screenPosition)));
        }
        
        private void UpdateInput()
        {
            
        }
    }
}