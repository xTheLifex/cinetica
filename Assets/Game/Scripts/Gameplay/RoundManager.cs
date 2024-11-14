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
        private static int _movesPerTurn = 1;
        private static int _moves = 1;
        
        public static float angle = 0f;
        public static float force = 500f;
        
        public static Building selectedBuilding;
        public static Building targetBuilding;
        
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
        
        private void Awake()
        {
            Instance = this;
            OnTurnStart.RemoveAllListeners();
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
                _logger.Log("Starting Turn...");
                turnState = TurnState.PreTurn;
                yield return new WaitForSeconds(1f);
                OnTurnStart?.Invoke();

                // START
                _logger.Log("Start Move.");
                
                // ====== SELECT TARGET & WEAPON & PARAMETERS
                _logger.Log("Waiting for target & weapon & parameters...");
                turnState = TurnState.SelectBuilding;
                yield return IWaitForSelection();
                OnSelection?.Invoke();
                
                // ====== WAIT FOR FIRING
                turnState = TurnState.WaitForResult;
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
                
                OnTurnEnd?.Invoke();
                _logger.Log("End of turn. Starting new turn soon.");
                
                _moves = _movesPerTurn;
                selectedBuilding = null;
                targetBuilding = null;
                angle = 0f;
                force = 0f;
                
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