using System;
using System.Collections;
using Cinetica.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Cinetica.Gameplay
{
    public class RoundManager : Singleton<RoundManager>
    {
        private Turn _turn = Turn.Player;

        private Building selectedBuilding;
        private Building targetBuilding;
        private float _angle = 0f;
        private float _force = 500f;
        
        private const int MOVES_PER_TURN = 1;
        private int _moves = 1;
        
        public static UnityEvent OnPlayerTurn = new UnityEvent();
        public static UnityEvent OnEnemyTurn = new UnityEvent();

        public static UnityEvent OnTurnStart = new UnityEvent();
        public static UnityEvent OnTurnEnd = new UnityEvent();

        public bool IsPlayerTurn() => _turn == Turn.Player;
        public bool IsEnemyTurn() => _turn == Turn.Enemy;
        
        private void Awake()
        {
            
        }
    }
}