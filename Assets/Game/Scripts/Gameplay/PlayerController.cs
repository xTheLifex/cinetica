using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;

namespace Cinetica.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        private UIDocument gui;
        
        private void Awake()
        {
            GameManager.Instance.OnFingerDown.AddListener(OnFingerDown);
        }

        public void OnFingerDown(Finger finger)
        {
            
        }
        
        public void MoveCameraTo(Transform t)
        {
            
        }
    }
}