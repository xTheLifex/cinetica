using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Logger = Cinetica.Utility.Logger;

namespace Cinetica.Gameplay
{
    public class RoundManager : MonoBehaviour
    {
        public static float angle = 0f;
        public static float velocity = 500f;
        
        public static Building selectedBuilding;
        public static Building targetBuilding;
        public static bool selectionsMade = false;
        public static bool skip = false;
        public static Turn turn = Turn.Player;

        public static RoundState roundState = RoundState.Playing;
        public static TurnState turnState = TurnState.PreTurn;

        public static UnityEvent OnTurnStart = new UnityEvent();
        public static UnityEvent OnTurnEnd = new UnityEvent();
        public static UnityEvent OnSelection = new UnityEvent();

        public static bool IsPlayerTurn() => turn == Turn.Player;
        public static PlayerController GetPlayer() => FindFirstObjectByType<PlayerController>();
        public static EnemyAI GetEnemy() => FindFirstObjectByType<EnemyAI>();

        private Logger _logger = new Logger("Round Manager");

        public Transform stageTransforms;

        public Transform playerCamStaticPos, enemyCamStaticPos;

        public static RoundManager Instance;
        public static GameObject GetProjectilePrefab() => Instance.projectilePrefab;
        public GameObject projectilePrefab;

        public string levelID; // Level ID in Level Entries 
        
        public bool ValidGame(Side side)
        {
            if (Building.GetCore(side).damageableComponent.health <= 0) return false;
            return (Building.GetSelectableBuildings(side)).Count > 0;
        }
        
        private void Awake()
        {
            Instance = this;
            OnTurnStart.RemoveAllListeners();
            OnTurnEnd.RemoveAllListeners();
            OnSelection.RemoveAllListeners();
            if (String.IsNullOrEmpty(levelID)) _logger.LogWarning("Level does not have a level ID set! It will not track progress!");
            StartCoroutine(IExecuteRound());
        }

        private IEnumerator IExecuteRound()
        {
            if (GameManager.Instance.IsLoadingLevel)
            {
                _logger.Log("Waiting for level to finish loading...");
                yield return new WaitUntil(() => GameManager.Instance.IsLoadingLevel == false);
            }
            
            _logger.Log("Starting Round...");
            yield return null;
            
            while (roundState == RoundState.Playing)
            {
                // CHECK FOR WIN OR LOSS
                if (!ValidGame(Side.Player))
                {
                    var player = GetPlayer();
                    player.ToggleEndScreen(true);
                    roundState = RoundState.Defeat;
                    break;
                }

                if (!ValidGame(Side.Enemy))
                {
                    var player = GetPlayer();
                    player.ToggleEndScreen(true);
                    if (!String.IsNullOrEmpty(levelID)) GameManager.playerData.SetLevelComplete(levelID);
                    GameManager.playerData.Save();
                    roundState = RoundState.Victory;
                    break;
                }
                
                // START
                _logger.Log($"Start {(IsPlayerTurn() ? "Player" : "Enemy")}'s turn");
                turnState = TurnState.PreTurn;
                yield return new WaitForSeconds(1f);
                OnTurnStart?.Invoke();

                // ====== SELECT TARGET & WEAPON & PARAMETERS
                _logger.Log("Waiting for target & weapon & parameters...");
                turnState = TurnState.SelectBuilding;
                yield return IWaitForSelection();
                OnSelection?.Invoke();

                // ====== WAIT FOR FIRING
                turnState = TurnState.WaitForResult;
                selectionsMade = false;
                _logger.Log("Waiting for weapon to fire...");
                var stage = GetClosestStageTransform(selectedBuilding.transform.position);
                if (stage) GetPlayer().SetStageCamera(selectedBuilding.transform, stage);
                yield return new WaitForSeconds(1f);
                yield return IFireWeapon();

                // ====== WAIT FOR HIT
                _logger.Log("Waiting for effects...");
                yield return IWaitForEffects();
                
                // ====== END TURN
                OnTurnEnd?.Invoke();
                _logger.Log("End of turn. Starting new turn soon.");

                selectedBuilding = null;
                targetBuilding = null;
                angle = 0f;
                velocity = 0f;
                selectionsMade = false;
                
                turn = (turn == Turn.Enemy) ? Turn.Player : Turn.Enemy;
                yield return null;
            }
        }

        private Transform GetClosestStageTransform(Vector3 pos) => stageTransforms.GetComponentsInChildren<Transform>()
            .OrderBy(t => Vector3.Distance(t.position, pos))
            .FirstOrDefault();

        
        private IEnumerator IWaitForSelection()
        {
            if (!IsPlayerTurn())
                yield return GetEnemy().ISelect();
            
            skip = false;
            yield return new WaitUntil(() => selectedBuilding || skip);
            skip = false;
            yield return new WaitUntil(() => targetBuilding || skip);
            skip = false;
            yield return new WaitUntil(() => selectionsMade || skip);
            
            _logger.Log("Selection ended with angle of " + angle  + " and velocity of "  + velocity);
        }

        private IEnumerator IFireWeapon()
        {
            //TODO: Sound effects
            // TODO: If its a railgun it should just shoot a ultimate mega blaster laser

            if (!projectilePrefab || !selectedBuilding)
                yield break;

            var obj = Instantiate(projectilePrefab, selectedBuilding.GetFiringPosition(), Quaternion.identity);
            
            var projectile = obj.GetComponent<Projectile>();
            if (!projectile)
            {
                Debug.LogError("Projectile fired without projectile component.");
                Destroy(obj);;
            }

            var horizontalRotation = selectedBuilding.horizontalAxis.rotation;
            var verticalRotation = Quaternion.Euler(angle, 0f, 0f);

            // Combine the horizontal and vertical rotations
            var fireRotation = horizontalRotation * verticalRotation;
            var initialVelocity = fireRotation * Vector3.back * velocity;
            projectile.Initialize(initialVelocity, selectedBuilding);

            var player = GetPlayer();
            var stage = GetClosestStageTransform(obj.transform.position);
            player.SetStageCamera(obj.transform, stage);
            
            yield return new WaitUntil(() => !obj);
            
            yield return null;
        }
        
        private IEnumerator IWaitForEffects()
        {
            yield break;
        }
    }
}