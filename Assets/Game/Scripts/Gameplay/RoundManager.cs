using System;
using System.Collections;
using System.Linq;
using Cinetica.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Logger = Cinetica.Utility.Logger;

namespace Cinetica.Gameplay
{
    public class RoundManager : MonoBehaviour
    {
        public static float angle = 0f;
        public static float force = 500f;
        
        public static Building selectedBuilding;
        public static Building targetBuilding;
        public static bool selectionsMade = false;
        public static Turn turn = Turn.Player;

        public static RoundState roundState = RoundState.Playing;
        public static TurnState turnState = TurnState.PreTurn;

        public static UnityEvent OnMoveStart = new UnityEvent();
        public static UnityEvent OnTurnEnd = new UnityEvent();

        public static UnityEvent OnSelection = new UnityEvent();

        public static bool IsPlayerTurn() => turn == Turn.Player;
        public static PlayerController GetPlayer() => FindFirstObjectByType<PlayerController>();
        public static EnemyAI GetEnemy() => FindFirstObjectByType<EnemyAI>();

        private Logger _logger = new Logger("Round Manager");

        public Transform stageTransforms;

        public Transform playerCamStaticPos, enemyCamStaticPos;

        public static RoundManager Instance;
        
        private void Awake()
        {
            Instance = this;
            OnMoveStart.RemoveAllListeners();
            OnTurnEnd.RemoveAllListeners();
            OnSelection.RemoveAllListeners();
            StartCoroutine(IExecuteRound());
        }

        private IEnumerator IExecuteRound()
        {
            _logger.Log("Starting Round...");
            yield return null;
            while (true)
            {
                // START
                _logger.Log($"Start {(IsPlayerTurn() ? "Player" : "Enemy")}'s turn");
                turnState = TurnState.PreTurn;
                yield return new WaitForSeconds(1f);
                OnMoveStart?.Invoke();

                // ====== SELECT TARGET & WEAPON & PARAMETERS
                _logger.Log("Waiting for target & weapon & parameters...");
                turnState = TurnState.SelectBuilding;
                yield return IWaitForSelection();
                OnSelection?.Invoke();

                // ====== WAIT FOR FIRING
                turnState = TurnState.WaitForResult;
                selectionsMade = false;
                _logger.Log("Waiting for weapon to fire...");
                yield return IFireWeapon();

                var stage = GetClosestStageTransform(selectedBuilding.transform.position);
                if (stage)
                {
                    // TODO: Replace with projectile.
                    GetPlayer().SetStageCamera(selectedBuilding.transform, stage);
                }

                // ====== WAIT FOR HIT
                _logger.Log("Waiting for effects...");
                yield return IWaitForEffects();
                
                // ====== END TURN
                OnTurnEnd?.Invoke();
                _logger.Log("End of turn. Starting new turn soon.");

                selectedBuilding = null;
                targetBuilding = null;
                angle = 0f;
                force = 0f;
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
            
            //turnState = TurnState.SelectBuilding;
            while (!selectedBuilding) yield return null;
            
            //turnState = TurnState.SelectTarget;
            while (!targetBuilding) yield return null;
            
            while (!selectionsMade) yield return null;
            
            _logger.Log("Selection ended with angle of " + angle  + " and force of "  + force);
        }

        private IEnumerator IFireWeapon()
        {
            yield return null;
        }
        
        private IEnumerator IWaitForEffects()
        {
            yield break;
            // TODO: Finish?
            var buildings = GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None).ToArray();
            foreach (var b in buildings)
                yield return b.IDisplayEffects();
        }
    }
}