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
        #region Declarations - Misc
        [NonSerialized] public Camera cam;
        public float cameraSpeed = 5f;
        public bool hardFollow = false;
        public string subTextOverride;
        public float velocity = 0f;
        public float angle = 0f;
        public Transform cameraTarget; // Camera will focus around this target.
        public Transform cameraTransformOverride; // Camera will lock to this transform exactly.
        public Transform stageTransform; // Will set camera to this position instead, looking at target.
        #endregion
        // ==================== USER INTERFACE ====================================================
        #region Declarations - UI
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

        private SliderInt _velocitySlider, 
            _angleSlider;
        
        public void Awake()
        {
            cam = Camera.main;
            _document = GetComponent<UIDocument>();
            _infoPanel = _document.rootVisualElement.Q<VisualElement>("Infopanel");
            _controls = _document.rootVisualElement.Q<VisualElement>("Controls");
            
            _selectPrevious = _document.rootVisualElement.Q<Button>("Previous");
            _selectNext = _document.rootVisualElement.Q<Button>("Next");
            _selectConfirm = _document.rootVisualElement.Q<Button>("Select");
            _cancelTarget = _document.rootVisualElement.Q<Button>("CancelTarget");

            _turnText = _document.rootVisualElement.Q<TextElement>("TurnText");
            _subText = _document.rootVisualElement.Q<TextElement>("SubText");

            _velocitySlider = _document.rootVisualElement.Q<SliderInt>("VelocitySlider");
            _angleSlider = _document.rootVisualElement.Q<SliderInt>("AngleSlider");

            friendlyBuildings = Building.GetAliveBuildings(Side.Player);
            enemyBuildings = Building.GetAliveBuildings(Side.Enemy);
            
            RoundManager.OnTurnStart.AddListener(OnMoveStart);
            RoundManager.OnTurnEnd.AddListener(OnTurnEnd);;

            _selectNext.clicked += SelectNext;
            _selectPrevious.clicked += SelectPrevious;
            _selectConfirm.clicked += SelectConfirm;
            _cancelTarget.clicked += CancelTarget;
        }

        public void Update()
        {
            UpdateUI();
            UpdateCamera();
        }

        public void OnDestroy()
        {
            RoundManager.OnTurnStart.RemoveListener(OnMoveStart);
            RoundManager.OnTurnEnd.RemoveListener(OnTurnEnd);
        }
        #endregion
        // ==================== METHODS ===========================================================
        #region Methods
        public void OnMoveStart()
        {
            // Update the selectables
            friendlyBuildings = Building.GetSelectableBuildings(Side.Player);
            enemyBuildings = Building.GetAliveBuildings(Side.Enemy);
            
            selectedBuilding = friendlyBuildings[0];
            targetedBuilding = enemyBuildings[0];

            var minAngle = selectedBuilding.minAngle;
            var maxAngle = selectedBuilding.maxAngle;
            var minVelocity = selectedBuilding.minVelocity;
            var maxVelocity = selectedBuilding.maxVelocity;
            
            angle = Mathf.Clamp(selectedBuilding.angle, minAngle, maxAngle);
            velocity = Mathf.Clamp(selectedBuilding.velocity, minVelocity, maxVelocity);

            _angleSlider.highValue = (int)maxAngle;
            _angleSlider.lowValue = (int)minAngle;
            _velocitySlider.highValue = (int)maxVelocity;
            _velocitySlider.lowValue = (int)minVelocity;
            
            _angleSlider.value = (int)angle;
            _velocitySlider.value = (int)velocity;
            
            ResetCamera();
            cameraTarget = selectedBuilding.transform;
        }
        
        public void OnTurnEnd()
        {
            
        }
        
        public void SelectNext()
        {
            Debug.Log("Select Next");
            if (!RoundManager.IsPlayerTurn()) return;
            if (RoundManager.turnState == TurnState.SelectBuilding)
            {
                selectedBuilding = friendlyBuildings.NextOf(selectedBuilding);
                cameraTarget = selectedBuilding.transform;
            }
            else if (RoundManager.turnState == TurnState.SelectTarget)
            {
                targetedBuilding = enemyBuildings.NextOf(targetedBuilding);
                cameraTarget = targetedBuilding.transform;
            }
        }
        
        public void SelectPrevious()
        {
            Debug.Log("Select Previous");
            if (!RoundManager.IsPlayerTurn()) return;
            if (RoundManager.turnState == TurnState.SelectBuilding)
            {
                selectedBuilding = friendlyBuildings.PreviousOf(selectedBuilding);
                cameraTarget = selectedBuilding.transform;
            }
            else if (RoundManager.turnState == TurnState.SelectTarget)
            {
                targetedBuilding = enemyBuildings.PreviousOf(targetedBuilding);
                cameraTarget = targetedBuilding.transform;
            }
        }

        public void SelectConfirm()
        {
            if (!RoundManager.IsPlayerTurn()) return;
            switch (RoundManager.turnState)
            {
                case TurnState.SelectBuilding:
                    // Building is selected. Go into Target mode.
                    RoundManager.selectedBuilding = selectedBuilding;
                    RoundManager.turnState = TurnState.SelectTarget;
                    return;
                case TurnState.SelectTarget:
                    // Target is selected. Go into parameter mode.
                    RoundManager.targetBuilding = targetedBuilding;
                    RoundManager.turnState = TurnState.InputParameters;
                    ResetCamera();
                    return;
                case TurnState.InputParameters:
                    // Fire weapon
                    RoundManager.angle = angle;
                    RoundManager.velocity = velocity;
                    RoundManager.selectionsMade = true;
                    return;
            }
        }

        public void CancelTarget()
        {
            if (!RoundManager.IsPlayerTurn()) return;

            RoundManager.turnState = TurnState.SelectBuilding;
            cameraTarget = selectedBuilding.transform;
        }
        #endregion
        // ==================== USER INTERFACE ====================================================
        #region UI
        public void UpdateUI()
        {
            if (RoundManager.roundState != RoundState.Playing)
            {
                _subText.visible = false;
                _infoPanel.visible = false;
                _controls.visible = false;
                _turnText.visible = true;
                _turnText.text = (RoundManager.roundState == RoundState.Victory ? "<color=green>Vitória</color>" : "<color=red>Derrota</color>");
                return;
            }

            switch (RoundManager.turnState)
            {
                case TurnState.PreTurn:
                    _infoPanel.visible = false;
                    _controls.visible = false;
                    _subText.visible = false;
                    _turnText.visible = false;
                    break;
                case TurnState.SelectBuilding:
                    _infoPanel.visible = false;
                    _controls.visible = true;
                    break;
                case TurnState.SelectTarget:
                    _infoPanel.visible = false;
                    _controls.visible = true;
                    if (targetedBuilding && RoundManager.IsPlayerTurn())
                        SetTrackingTransform(targetedBuilding.transform);
                    break;
                case TurnState.InputParameters:
                    _infoPanel.visible = true;
                    _controls.visible = true;
                    if (selectedBuilding && RoundManager.IsPlayerTurn())
                        SetTrackingTransform(selectedBuilding.transform);
                    break;
                case TurnState.WaitForResult:
                    _infoPanel.visible = false;
                    _controls.visible = false;
                    _subText.visible = false;
                    _turnText.visible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            

            if (RoundManager.IsPlayerTurn())
            {
                _controls.visible = true;
                _turnText.text = "<b>Seu Turno</b>";
                switch (RoundManager.turnState)
                {
                    case TurnState.SelectBuilding:
                        _subText.text = "Selecione uma Torreta...";
                        _selectConfirm.text = "Selecionar Torreta";
                        break;
                    case TurnState.SelectTarget:
                        _subText.text = "Selecione um Alvo...";
                        _selectConfirm.text = "Selecionar Alvo";
                        break;
                    case TurnState.InputParameters:
                        _subText.text = "Defina os Parâmetros de Lançamento";
                        _selectConfirm.text = "Confirmar Lançamento";
                        // TODO: Any restrictions to be passed to UI here and validated after too.
                        velocity = _velocitySlider.value;
                        angle = _angleSlider.value;
                        
                        RoundManager.angle = angle;
                        RoundManager.velocity = velocity;
                        break;
                    default:
                        _subText.visible = false;
                        break;
                }
            }
            else
            {
                _turnText.text = "<color=red>Turno do Oponente</color>";
                _infoPanel.visible = false;
                _controls.visible = false;
            }
            
            _subText.visible = RoundManager.turnState is not (TurnState.WaitForResult or TurnState.PreTurn);
            _turnText.visible = RoundManager.turnState !=  TurnState.PreTurn && RoundManager.roundState != RoundState.Defeat && RoundManager.roundState != RoundState.Victory;
            _controls.visible = RoundManager.turnState is not (TurnState.PreTurn or TurnState.WaitForResult);
            if (subTextOverride != null)
            {
                _subText.visible = true;
                _subText.text = subTextOverride;
            }
            


            if (selectedBuilding && RoundManager.turnState is TurnState.SelectBuilding)
            {
                _selectNext.SetEnabled(true);
                _selectPrevious.SetEnabled(true);
                _selectConfirm.SetEnabled(selectedBuilding.buildingType is BuildingType.Railgun or BuildingType.Turret);
            }
            else if (targetedBuilding && RoundManager.turnState is TurnState.SelectTarget)
            {
                _selectNext.SetEnabled(true);
                _selectPrevious.SetEnabled(true);
                _selectConfirm.SetEnabled(targetedBuilding.damageableComponent.health > 0f);
            }
            else
            {
                _selectConfirm.SetEnabled(RoundManager.turnState is TurnState.InputParameters);
                _selectNext.SetEnabled(false);
                _selectPrevious.SetEnabled(false);
            }
        }
        #endregion
        // ==================== CAMERA ============================================================
        #region Camera
        public void UpdateCamera()
        {
            if (cameraTransformOverride)
                SnapCameraTo(cameraTransformOverride);
            else
            {
                if (stageTransform)
                    MoveCameraToStagePosition(stageTransform);
                else if(cameraTarget)
                    MoveCameraTowards(cameraTarget);
            }
        }

        public void SetStageCamera(Transform lookAt, Transform stage)
        {
            ResetCamera();
            stageTransform = stage;
            cameraTarget = lookAt;
        }

        public void SetTrackingTransform(Transform t)
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

        public void SetCameraToStaticPosition()
        {
            if (RoundManager.Instance.playerCamStaticPos && RoundManager.IsPlayerTurn())
                SetStaticTransform(RoundManager.Instance.playerCamStaticPos);
            else if (RoundManager.Instance.enemyCamStaticPos && !RoundManager.IsPlayerTurn())
                SetStaticTransform(RoundManager.Instance.enemyCamStaticPos);
        }
        
        public void MoveCameraToStagePosition(Transform t)
        {
            if (!stageTransform) return;
            transform.position = Vector3.Lerp(transform.position, stageTransform.position, Time.deltaTime * cameraSpeed);
            if (!cameraTarget) return;
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
        #endregion
    }
}