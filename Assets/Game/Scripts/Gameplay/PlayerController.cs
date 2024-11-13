using System;
using System.Collections.Generic;
using System.Linq;
using Cinetica.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Cinetica.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        
        // ==================== MISC ===========================================================
        [NonSerialized] public Camera cam;
        public float cameraSpeed = 5f;
        public bool hardFollow = false;
        
        public Transform cameraTarget; // Camera will focus around this target.
        public Transform cameraTransformOverride; // Camera will lock to this transform exactly.
        public Transform stageTransform; // Will set camera to this position instead, looking at target.
        
        // ==================== UI ===========================================================
        private UIDocument _document;
        private List<Building> friendlyBuildings = new List<Building>();
        private List<Building> enemyBuildings = new List<Building>();

        private Building selectedBuilding;
        private Building targetedBuilding;
        
        private Button _selectPrevious,
            _selectNext,
            _selectConfirm,
            _fireWeapon,
            _cancelTarget;

        private TextElement _turnText, _subText;

        private VisualElement _controls, 
            _infoPanel;
        

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

            _turnText = _document.rootVisualElement.Q<TextElement>("TurnText");
            _subText = _document.rootVisualElement.Q<TextElement>("SubText");

            friendlyBuildings = Building.GetSelectable(Side.Player);
            enemyBuildings = Building.GetSelectable(Side.Enemy);
            
            RoundManager.OnTurnStart.AddListener(OnTurnStart);
            RoundManager.OnTurnEnd.AddListener(OnTurnEnd);;
        }

        public void Update()
        {
            UpdateUI();
            UpdateCamera();
        }

        public void OnDestroy()
        {
            RoundManager.OnTurnStart.RemoveListener(OnTurnStart);
            RoundManager.OnTurnEnd.RemoveListener(OnTurnEnd);
        }

        // ==================== METHODS ===========================================================
        
        public void OnTurnStart()
        {
            // Update the selectables
            friendlyBuildings = Building.GetSelectable(Side.Player);
            enemyBuildings = Building.GetSelectable(Side.Enemy);

            selectedBuilding = friendlyBuildings[1];
            targetedBuilding = enemyBuildings[1];
        }
        
        public void OnTurnEnd()
        {
            
        }
        
        public void SelectNext()
        {
            if (!RoundManager.IsPlayerTurn()) return;
            if (RoundManager.turnState == TurnState.SelectWeapon)
                selectedBuilding = friendlyBuildings.NextOf(selectedBuilding);
            else if(RoundManager.turnState == TurnState.SelectTarget)
                targetedBuilding = enemyBuildings.NextOf(targetedBuilding);
        }

        public void SelectPrevious()
        {
            if (!RoundManager.IsPlayerTurn()) return;
            if (RoundManager.turnState == TurnState.SelectWeapon)
                selectedBuilding = friendlyBuildings.PreviousOf(selectedBuilding);
            else if(RoundManager.turnState == TurnState.SelectTarget)
                targetedBuilding = enemyBuildings.PreviousOf(targetedBuilding);
        }

        public void SelectConfirm()
        {
            if (!RoundManager.IsPlayerTurn()) return;
        }

        public void FireWeapon()
        {
            if (!RoundManager.IsPlayerTurn()) return;
        }

        public void CancelTarget()
        {
            if (!RoundManager.IsPlayerTurn()) return;
        }

        public void UpdateUI()
        {
            if (RoundManager.roundState != RoundState.Playing)
            {
                _subText.visible = false;
                _infoPanel.visible = false;
                _controls.visible = false;
                _turnText.visible = true;
                _turnText.text = (RoundManager.roundState == RoundState.Victory ? "Vitória" : "Derrota");
                return;
            }
            
            if (RoundManager.IsPlayerTurn())
            {
                _turnText.text = "Seu Turno";
                _subText.text = RoundManager.turnState == TurnState.SelectTarget
                    ? "Selecionar Torreta"
                    : "Selecionar Arma";
            }
            else
            {
                _turnText.text = "Turno do Oponente";
                _infoPanel.visible = false;
                _controls.visible = false;
            }
            
            _subText.visible = RoundManager.turnState != TurnState.WaitForResult;
            _turnText.visible = RoundManager.turnState != TurnState.WaitForResult;
        }

        // ==================== CAMERA ===========================================================
        public void UpdateCamera()
        {
            if (cameraTransformOverride)
                SnapCameraTo(cameraTransformOverride);
            else
            {
                if (stageTransform)
                    MoveCameraToStagePosition(stageTransform);
                else
                    MoveCameraTowards(cameraTarget);
            }
        }

        public void SetStageCamera(Transform lookAt, Transform stage)
        {
            ResetCamera();
            stageTransform = stage;
            cameraTarget = lookAt;
        }

        public void SetTrackingObject(Transform t)
        {
            ResetCamera();
            cameraTarget = t;
        }

        public void SetStaticTransform(Transform t)
        {
            ResetCamera();
            cameraTransformOverride = t;
        }
        
        public void ResetCamera()
        {
            cameraTransformOverride = null;
            cameraTarget = null;
            stageTransform = null;
        }
        
        public void MoveCameraToStagePosition(Transform t)
        {
            transform.position = Vector3.Lerp(transform.position, stageTransform.position, Time.deltaTime * cameraSpeed);   
            transform.LookAt(cameraTarget.position + (Vector3.up * 1f));
        }
        
        public void MoveCameraTowards(Transform target)
        {
            if (!cam) return;

            // Find the "CameraTarget" within the target object
            Transform camTarget = target.Find("CameraTarget");
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
                Vector3 targetPos = target.position + (Vector3.up * 2f) + (Vector3.forward * 2f);
        
                if (hardFollow)
                {
                    transform.position = targetPos;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraSpeed);
                }

                transform.LookAt(target.position + (Vector3.up * 2f));
            }
        }


        public void SnapCameraTo(Transform t)
        {
            if (!cam) return;
            transform.position = Vector3.Lerp(transform.position, t.position,
                Time.deltaTime * cameraSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, t.rotation,
                Time.deltaTime * cameraSpeed);
        }
    }
}