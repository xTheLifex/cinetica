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
        private static Building selectedBuilding;
        private static Building targetBuilding;
        private static float _angle = 0f;
        private static float _force = 500f;
        
        private static int _movesPerTurn = 1;
        private static int _moves = 1;
        
        public static Turn turn = Turn.Player;

        public static RoundState roundState;
        public static TurnState turnState;
        
        public static UnityEvent OnPlayerTurn = new UnityEvent();
        public static UnityEvent OnEnemyTurn = new UnityEvent();

        public static UnityEvent OnTurnStart = new UnityEvent();
        public static UnityEvent OnTurnEnd = new UnityEvent();

        public static bool IsPlayerTurn() => turn == Turn.Player;
        public static bool IsEnemyTurn() => turn == Turn.Enemy;
        
        private void Awake()
        {
            
        }
    }
}