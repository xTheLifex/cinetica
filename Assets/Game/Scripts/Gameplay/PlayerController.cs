using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cinetica.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        // UI --------------------------------------------------------
        private UIDocument _document;
        private Building[] buildings;

        private Building selectedBuilding;
        private Building targetedBuilding;
        
        private Button _selectPrevious,
            _selectNext,
            _selectConfirm,
            _fireWeapon,
            _cancelTarget;

        private VisualElement _controls, 
            _infoPanel;
        
        // MISC ------------------------------------------------------
        [NonSerialized] public Camera cam;
        public float cameraSpeed = 5f;
        public bool hardFollow = false;
        public Transform cameraTarget; // Camera will focus around this target.
        public Transform cameraTransformOverride; // Camera will lock to this transform exactly.
        
        public void Awake()
        {
            cam = Camera.main;
            _document = GetComponent<UIDocument>();
            _infoPanel = _document.rootVisualElement.Q<VisualElement>("Infopanel");
            _controls = _document.rootVisualElement.Q<VisualElement>("Controls");
            
            _selectPrevious = _document.rootVisualElement.Q<Button>("Previous");
            _selectNext = _document.rootVisualElement.Q<Button>("Next");
            _selectConfirm = _document.rootVisualElement.Q<Button>("Select");
            _fireWeapon = _document.rootVisualElement.Q<Button>("FireWeapon");
            _cancelTarget = _document.rootVisualElement.Q<Button>("CancelTarget");

            buildings = GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None);
        }

        public void SelectNext() {}
        public void SelectPrevius() {}
        public void SelectConfirm() {}
        public void FireWeapon() {}
        public void CancelTarget() {}
        
        
        public void Update()
        {
            if (cameraTransformOverride)
                SnapCameraTo(cameraTransformOverride);
            else
                MoveCameraTowards(cameraTarget);
        }

        public void MoveCameraTowards(Transform t)
        {
            if (!cam) return;

            // Find the "CameraTarget" within the target object
            Transform camTarget = cameraTarget.Find("CameraTarget");
            if (camTarget != null)
            {
                // If "CameraTarget" exists, find "CameraPosition" within it
                Transform camPosition = camTarget.Find("CameraPosition");
        
                // Set camera target position and look-at point based on found objects
                Vector3 targetPos = camPosition != null ? camPosition.position : camTarget.position;
                Vector3 lookAtPos = camTarget.position;
        
                if (hardFollow)
                {
                    transform.position = targetPos;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed);
                }

                transform.LookAt(lookAtPos);
            }
            else
            {
                // Default behavior if "CameraTarget" is not found
                Vector3 targetPos = cameraTarget.position + (Vector3.up * 2f) + (Vector3.forward * 2f);
        
                if (hardFollow)
                {
                    transform.position = targetPos;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed);
                }

                transform.LookAt(cameraTarget.position + (Vector3.up * 2f));
            }
        }


        public void SnapCameraTo(Transform t)
        {
            if (!cam) return;
            transform.position = Vector3.Lerp(transform.position, cameraTransformOverride.position,
                Time.deltaTime * cameraSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraTransformOverride.rotation,
                Time.deltaTime * cameraSpeed);
        }
    }
}