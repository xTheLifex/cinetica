using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Circuits
{
    public class PlayerControl : MonoBehaviour
    {
        [Header("Pan Settings")]
        public float panSpeed = 0.01f;
        public float maxDistance = 20f;
        public float returnSpeed = 0.5f;
        private Vector3 _originPoint;
        private Vector3 _targetPosition;
        private Vector3 _startPosition;
        private Bounds _bounds;
        private void Start()
        {
            _originPoint = Camera.main!.transform.position;
            _targetPosition = _originPoint;
            _bounds = new Bounds(_originPoint, new Vector3(maxDistance, maxDistance, maxDistance));
            EnhancedTouch.Touch.onFingerDown += FingerDown;

            GameManager.OnPan.AddListener(PanCamera);
            GameManager.OnZoom.AddListener(ZoomCamera);
        }

        private void FingerDown(EnhancedTouch.Finger obj)
        {
            _startPosition = Camera.main!.transform.position;
        }

        private void PanCamera(Vector2 delta)
        {
            if (!Camera.main) return;

            Vector3 movement = new Vector3(-delta.x * panSpeed, 0, -delta.y * panSpeed);
            var newPos = _startPosition + movement;

            // Calculate distance from origin and adjust pan speed
            float distanceFromOrigin = Vector3.Distance(newPos, _originPoint);
            if (distanceFromOrigin > maxDistance)
            {
                // Scale down movement speed based on distance
                float speedScale = Mathf.Clamp(1 - (distanceFromOrigin - maxDistance) / maxDistance, 0f, 1f);
                newPos = Vector3.Lerp(_originPoint, newPos, speedScale);
            }

            if (!_bounds.Contains(newPos))
                return;

            _targetPosition = newPos;
        }

        private void Update()
        {
            if (!Camera.main) return;

            // Move smoothly towards target position
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _targetPosition, 0.1f);
        }

        private void ZoomCamera(float zoomAmount)
        {
            if (!Camera.main) return;

            // Optional zoom feature; modify or comment out as needed
            float newZoom = Mathf.Clamp(Camera.main.orthographicSize - zoomAmount, 5f, 20f);
            Camera.main.orthographicSize = newZoom;
        }

        private void OnDisable()
        {
            GameManager.OnPan.RemoveListener(PanCamera);
            GameManager.OnZoom.RemoveListener(ZoomCamera);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.25f, 1f, 0.25f, 0.25f);
            Gizmos.DrawCube(Application.isPlaying ? _originPoint : Camera.main!.transform.position, new Vector3(maxDistance, maxDistance, maxDistance));
        }
    }
}
