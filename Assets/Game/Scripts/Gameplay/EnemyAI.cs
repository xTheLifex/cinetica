using System;
using UnityEngine;

namespace Cinetica.Gameplay
{
    public class EnemyAI : MonoBehaviour
    {
        public Difficulty difficulty;
        public float imprecision = 1f;
        
        public void Awake()
        {
            RoundManager.OnTurnStart.AddListener(TurnStart);
            RoundManager.OnTurnEnd.AddListener(TurnEnd);
            switch (difficulty)
            {
                case Difficulty.Easy:
                    imprecision = 1f;
                    break;
                case Difficulty.Medium:
                    imprecision = 0.5f;
                    break;
                case Difficulty.Hard:
                    imprecision = 0.1f;
                    break;
            }
        }

        public void OnDestroy()
        {
            RoundManager.OnTurnStart.RemoveListener(TurnStart);
            RoundManager.OnTurnEnd.RemoveListener(TurnEnd);
        }

        public void TurnStart()
        {
            RoundManager.selectedBuilding = GetSelectedBuilding();
            RoundManager.targetBuilding = GetTargetBuilding();
            RoundManager.force = GetForce();
            RoundManager.angle = GetAngle();
        }

        public void TurnEnd()
        {
            
        }

        public Building GetSelectedBuilding()
        {
            // TODO Select weapons.
            return null;
        }

        public Building GetTargetBuilding()
        {
            // TODO: Select target
            return null;
        }

        public float GetForce()
        {
            // TODO: Select force based on selections above
            return 100f;
        }

        public float GetAngle()
        {
            // TODO: Get angle based on selections above
            return 0;
        }
    }
}