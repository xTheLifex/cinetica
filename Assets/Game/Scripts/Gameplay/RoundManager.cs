using System;
using System.Collections;
using Cinetica.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Cinetica.Gameplay
{
    public class RoundManager : Singleton<RoundManager>
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
        private PlayerController GetPlayer() => FindFirstObjectByType<PlayerController>();
        
        private void Awake()
        {
            StartCoroutine(IExecuteRound());
        }

        private IEnumerator IExecuteRound()
        {
            while (true)
            {
                OnTurnStart?.Invoke();
                while (_moves > 0)
                {
                    yield return IWaitForSelection();
                    OnSelection?.Invoke();
                    yield return IFireWeapon();
                    yield return IWaitForEffects();
                    _moves -= 1;
                }
                OnTurnEnd?.Invoke();
                _moves = _movesPerTurn;
                selectedBuilding = null;
                targetBuilding = null;
                angle = 0f;
                force = 0f;
                
                turn = (turn == Turn.Enemy) ? Turn.Player : Turn.Enemy;
            }
        }
        
        private IEnumerator IWaitForSelection()
        {
            while (selectedBuilding == null && targetBuilding == null) yield return null;
        }

        private IEnumerator IFireWeapon()
        {
            yield return null;
        }
        
        private IEnumerator IWaitForEffects()
        {
            yield return new WaitForSeconds(2f);
        }
    }
}