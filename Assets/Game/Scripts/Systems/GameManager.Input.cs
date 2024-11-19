using Cinetica.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Cinetica
{
    public partial class GameManager : Singleton<GameManager>
    {
        private float _initialDistance;
        private float _lastZoomFactor;
        private Vector2 _startScreenPosition;

        public UnityEvent<Finger> OnFingerDown = new UnityEvent<Finger>();
        public UnityEvent<Finger> OnFingerUp = new UnityEvent<Finger>();
        public UnityEvent<Finger> OnFingerMove = new UnityEvent<Finger>();
        
        private void InitializeInput()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();

            EnhancedTouch.Touch.onFingerDown += FingerDown;
            EnhancedTouch.Touch.onFingerUp += FingerUp;
            EnhancedTouch.Touch.onFingerMove += FingerMove;
        }

        private void FingerMove(Finger finger)
        {
            OnFingerMove?.Invoke(finger);
        }
        
        private void FingerUp(Finger finger)
        {
            OnFingerUp?.Invoke(finger);            
        }

        private void FingerDown(Finger finger)
        {
            OnFingerDown?.Invoke(finger);
        }
    }
}
