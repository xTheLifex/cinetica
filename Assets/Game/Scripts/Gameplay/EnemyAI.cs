using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static Cinetica.Utility.Utils;
using Logger = Cinetica.Utility.Logger;

namespace Cinetica.Gameplay
{
    public class EnemyAI : MonoBehaviour
    {
        public Difficulty difficulty;
        public float imprecision = 1f;
        private Logger _logger = new Logger("Enemy AI");
        public void Awake()
        {
            RoundManager.OnTurnStart.AddListener(MoveStart);
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
            RoundManager.OnTurnStart.RemoveListener(MoveStart);
            RoundManager.OnTurnEnd.RemoveListener(TurnEnd);
        }

        public void MoveStart()
        {   

        }

        public void TurnEnd()
        {
            
        }

        public IEnumerator ISelect()
        {
            // START
            _logger.Log("AI thinking start.");
            if (RoundManager.IsPlayerTurn()) yield break;
            var player = RoundManager.GetPlayer();
            var building = GetSelectedBuilding();
            var target = GetTargetBuilding();

            if (!building) _logger.LogWarning("No suitable building for selecting found!");
            if (!target) _logger.LogWarning("No suitable target found!");

            if (!building || !target)
            {
                player.subTextOverride = "O inimigo não parece agir...";
                yield return new WaitForSeconds(1f);
                player.subTextOverride = null;
                RoundManager.skip = true;
                yield break;
            }
            
            // BUILDING
            RoundManager.turnState = TurnState.SelectBuilding;
            player.SetTrackingTransform(building.transform);
            player.subTextOverride = "Inimigo selecionou " + building.name;
            RoundManager.selectedBuilding = building;
            yield return new WaitForSeconds(1.5f);
            
            // TARGET
            RoundManager.turnState = TurnState.SelectTarget;
            player.SetTrackingTransform(target.transform);
            player.subTextOverride = "O inimigo irá atacar o " + target.name;
            RoundManager.targetBuilding = target;
            yield return new WaitForSeconds(1.5f);
            
            // PARAMETERS
            RoundManager.turnState = TurnState.InputParameters;
            player.SetCameraToStaticPosition();
            RoundManager.force = GetForce();
            RoundManager.angle = GetAngle();
            player.subTextOverride = "O inimigo está decidindo os parâmetros...";
            yield return new WaitForSeconds(2f);
            
            // FINISH
            RoundManager.selectionsMade = true;
            player.subTextOverride = null;
            player.ResetCamera();
            _logger.Log($"AI made it's choice: F:{RoundManager.force}, A:{RoundManager.angle}");
        }

        public Building GetSelectedBuilding()
        {
            var weapons = Building.GetAllWeapons(Side.Enemy);
            
            if (difficulty == Difficulty.Hard)
            {
                // Try selecting railguns first
                var railgun = weapons.First(x => x.buildingType == BuildingType.Railgun);
                if (railgun) return railgun;
            }

            return Pick(weapons);
        }

        public Building GetTargetBuilding()
        {
            var plyWeapons = Building.GetAllWeapons(Side.Player);
            var plyBuildings = Building.GetAliveBuildings(Side.Player);
            
            if (difficulty == Difficulty.Hard)
            {
                var playerRailgun = plyWeapons.First(x => x.buildingType == BuildingType.Railgun);
                if (playerRailgun) return playerRailgun;
            }
            
            
            return Pick(plyBuildings.ToArray());
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