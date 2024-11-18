using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static Cinetica.Utility.Utils;
using Logger = Cinetica.Utility.Logger;
using Random = UnityEngine.Random;

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
                case Difficulty.Impossible:
                    imprecision = 0f;
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
            player.subTextOverride = "O inimigo está decidindo os parâmetros...";
            var launchParameters = GetLaunchParameters(building, target);
            RoundManager.velocity = Mathf.Clamp(launchParameters.vel + (imprecision * Random.Range(-5f, 5f)), building.minVelocity, building.maxVelocity);
            RoundManager.angle = Mathf.Clamp(launchParameters.angle + (imprecision * Random.Range(-5f, 5f)), building.minAngle, building.maxAngle);
            yield return new WaitForSeconds(1f);
            
            // FINISH
            RoundManager.selectionsMade = true;
            player.subTextOverride = null;
            player.ResetCamera();
            _logger.Log($"AI made it's choice: Velocity: {RoundManager.velocity}, Angle:{RoundManager.angle}");
        }

        public Building GetSelectedBuilding()
        {
            var weapons = Building.GetAllWeapons(Side.Enemy);
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

        public (float angle, float vel) GetLaunchParameters(Building weapon, Building target)
        {
            var defaults = (5f, 10f);
            var prefab = RoundManager.GetProjectilePrefab();
            if (!prefab) return defaults;

            var projectile = prefab.GetComponent<Projectile>();
            if (!projectile) return defaults;

            var radius = projectile.radius;
            var minAngle = weapon.minAngle;
            var maxAngle = weapon.maxAngle;

            var maxVelocity = weapon.maxVelocity;
            var minVelocity = weapon.minVelocity;

            var expireTime = projectile.expiryTime;

            for (var angle = maxAngle; angle > minAngle; angle -= 1f)
            {
                for (var velocity = maxVelocity; velocity > minVelocity; velocity -= 5f)
                {
                    weapon.angle = angle;
                    weapon.velocity = velocity;
                    if (Building.SimulateShot(weapon, target, angle, velocity, radius * 0.75f, expireTime, true, 1f))
                    {
                        _logger.Log($"AI found launch parameters: (angle: {angle}, velocity: {velocity})");
                        return (angle, velocity);
                    }
                }
            }

            _logger.LogWarning("Can't find a good angle to shoot projectile");
            return defaults;
        }
    }
}