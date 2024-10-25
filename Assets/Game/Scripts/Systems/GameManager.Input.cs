using Circuits.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Circuits
{
    [RequireComponent(typeof(PlayerInput))]
    public partial class GameManager : Singleton<GameManager>
    {
        [Header("Input")]
        public bool inputEnabled = true;
        public bool movementEnabled = true;
        public bool interactionEnabled = true;
        
        
        public static UnityEvent<Vector3> OnTouch = new UnityEvent<Vector3>();
        public static UnityEvent<Vector3> OnTouchStart = new UnityEvent<Vector3>();
        public static UnityEvent<Vector3> OnTouchEnd = new UnityEvent<Vector3>();

        private PlayerInput _playerInput;
        private InputAction _touchPosition;
        private InputAction _touchPress;
        
        private bool _touching = false;
        private Vector3 _lastTouchPos = Vector3.zero;
        private void InitializeInput()
        {
            _playerInput = GetComponent<PlayerInput>();
            _touchPosition = _playerInput.actions["TouchPosition"];
            _touchPress = _playerInput.actions["TouchPress"];

            _touchPress.performed += Touch;
        }
        
        
        private void UpdateInput()
        {
            
        }

        private void Touch(InputAction.CallbackContext context)
        {
            if (!inputEnabled) return;
            var cam = Camera.main;
            if (!cam) return;
            Vector3 position = cam.ScreenToWorldPoint(_touchPosition.ReadValue<Vector2>());
            position.y = 0f;
            
            if (_touchPress.WasCompletedThisFrame())
                OnTouchStart.Invoke(position);
            if (_touchPress.WasReleasedThisFrame())
                OnTouchEnd.Invoke(position);
            OnTouch.Invoke(position);
        
            _lastTouchPos = position;
        }
    }
}