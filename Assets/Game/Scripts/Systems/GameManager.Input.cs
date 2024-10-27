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
        public static UnityEvent<Vector2> OnPan = new UnityEvent<Vector2>();
        public static UnityEvent<float> OnZoom = new UnityEvent<float>();

        private float _initialDistance;
        private float _lastZoomFactor;
        private Vector2 _startScreenPosition;

        private void InitializeInput()
        {
            OnPan.RemoveAllListeners();
            OnZoom.RemoveAllListeners();
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();

            EnhancedTouch.Touch.onFingerDown += FingerDown;
            EnhancedTouch.Touch.onFingerUp += FingerUp;
            EnhancedTouch.Touch.onFingerMove += FingerMove;
        }

        private void FingerMove(Finger obj)
        {
            
        }

        private void UpdateInput()
        {
            if (EnhancedTouch.Touch.activeTouches.Count <= 0)
                return;

            EnhancedTouch.Touch finger = EnhancedTouch.Touch.activeTouches[0];
            if (!Camera.main) return;

            Vector2 delta = finger.screenPosition - _startScreenPosition;
            OnPan.Invoke(delta);

            // Pinch-to-zoom handling
            if (EnhancedTouch.Touch.activeTouches.Count == 2)
            {
                EnhancedTouch.Touch secondFinger = EnhancedTouch.Touch.activeTouches[1];
                float currentDistance = Vector2.Distance(finger.screenPosition, secondFinger.screenPosition);

                if (_initialDistance == 0)
                    _initialDistance = currentDistance;

                float zoomFactor = currentDistance / _initialDistance;
                if (Mathf.Abs(zoomFactor - _lastZoomFactor) > 0.01f)
                {
                    OnZoom.Invoke(zoomFactor - _lastZoomFactor);
                    _lastZoomFactor = zoomFactor;
                }
            }
        }

        private void FingerUp(Finger finger)
        {
            _initialDistance = 0;
            _lastZoomFactor = 0;
        }

        private void FingerDown(Finger finger)
        {
            _initialDistance = 0;
            _startScreenPosition = finger.screenPosition;
        }
    }
}
