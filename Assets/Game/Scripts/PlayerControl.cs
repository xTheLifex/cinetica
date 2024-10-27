using UnityEngine;

namespace Circuits
{
    public class PlayerControl : MonoBehaviour
    {
        private Camera _mainCamera;

        [Header("Pan Settings")]
        public float panSpeed = 0.01f;
        public float maxDistance = 20f;
        public float returnSpeed = 0.5f;
        private Vector3 _originPoint;
        private Vector3 _targetPosition;
        private Vector3 _velocity = Vector3.zero;

        private void Start()
        {
            _mainCamera = Camera.main;
            _originPoint = _mainCamera.transform.position;
            _targetPosition = _originPoint;

            GameManager.OnPan.AddListener(PanCamera);
            GameManager.OnZoom.AddListener(ZoomCamera);
        }

        private void PanCamera(Vector2 delta)
        {
            if (!_mainCamera) return;

            Vector3 movement = new Vector3(-delta.x * panSpeed, 0, -delta.y * panSpeed);
            _targetPosition = _mainCamera.transform.position + movement;

            // Calculate distance from origin and adjust pan speed
            float distanceFromOrigin = Vector3.Distance(_targetPosition, _originPoint);
            if (distanceFromOrigin > maxDistance)
            {
                // Scale down movement speed based on distance
                float speedScale = Mathf.Clamp(1 - (distanceFromOrigin - maxDistance) / maxDistance, 0.1f, 1f);
                _targetPosition = Vector3.Lerp(_originPoint, _targetPosition, speedScale);
            }
        }

        private void Update()
        {
            if (!_mainCamera) return;

            // Move smoothly towards target position
            _mainCamera.transform.position = Vector3.SmoothDamp(_mainCamera.transform.position, _targetPosition, ref _velocity, 0.1f);

            // Automatically return to origin if too far
            float currentDistance = Vector3.Distance(_mainCamera.transform.position, _originPoint);
            if (currentDistance > maxDistance)
            {
                Vector3 returnDirection = (_originPoint - _mainCamera.transform.position).normalized;
                _mainCamera.transform.position = Vector3.SmoothDamp(_mainCamera.transform.position, _originPoint, ref _velocity, returnSpeed);
            }
        }

        private void ZoomCamera(float zoomAmount)
        {
            if (!_mainCamera) return;

            // Optional zoom feature; modify or comment out as needed
            float newZoom = Mathf.Clamp(_mainCamera.orthographicSize - zoomAmount, 5f, 20f);
            _mainCamera.orthographicSize = newZoom;
        }

        private void OnDisable()
        {
            GameManager.OnPan.RemoveListener(PanCamera);
            GameManager.OnZoom.RemoveListener(ZoomCamera);
        }
    }
}
