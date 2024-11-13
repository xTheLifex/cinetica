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

        private Logger _logger = new Logger("Round Manager");

        public Transform stageTransforms;
        
        private void Awake()
        {
            StartCoroutine(IExecuteRound());
        }

        private IEnumerator IExecuteRound()
        {
            _logger.Log("Starting Round...");
            yield return new WaitForSeconds(2f);
            while (true)
            {
                _logger.Log("Starting Turn...");
                turnState = TurnState.PreTurn;
                yield return new WaitForSeconds(1f);
                OnTurnStart?.Invoke();
                while (_moves > 0)
                {
                    // START
                    _logger.Log("Start Move. Moves left: " + _moves);
                    
                    // ====== SELECT TARGET & WEAPON
                    _logger.Log("Waiting for target & weapon selection...");
                    yield return IWaitForSelection();
                    OnSelection?.Invoke();

                    var stage = GetClosestStageTransform(selectedBuilding.transform.position);
                    if (stage)
                    {
                        // TODO: Replace with projectile.
                        GetPlayer().SetStageCamera(selectedBuilding.transform, stage);
                    }
                    
                    // ====== WAIT FOR HIT
                    _logger.Log("Waiting for weapon to fire...");
                    yield return IFireWeapon();
                    
                    // ====== WAIT FOR EFFECTS
                    _logger.Log("Waiting for effects...");
                    turnState = TurnState.WaitForResult;
                    yield return IWaitForEffects();
                    
                    // END
                    _moves -= 1;
                }
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
            turnState = TurnState.SelectBuilding;
            while (!selectedBuilding) yield return null;
            
            turnState = TurnState.SelectTarget;
            while (!targetBuilding) yield return null;
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