using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinetica.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinetica.Gameplay
{
    [RequireComponent(typeof(Damageable))]
    public class Building : MonoBehaviour
    {
        public Side side = Side.Player;
        public Damageable damageableComponent;
        public BuildingType buildingType = BuildingType.Dummy;

        [FormerlySerializedAs("maxForce")] public float maxVelocity = 100f; // Turret: 100, Railgun: 1K
        public float minVelocity = 5f;
        public float maxAngle = 45f;
        public float minAngle = 0f;

        public float angle = 0f;
        [FormerlySerializedAs("velocity")] public float velocity = 0f;

        public Transform horizontalAxis;
        public Transform verticalAxis;
        public Transform firePosition;

        public float maxCharge = 1000f;
        public float charge = 1000f;
        public float chargeRecovery = 100f;

        public GameObject shield;
        
        private void Awake()
        {
            damageableComponent = GetComponent<Damageable>();
            RoundManager.OnTurnStart.AddListener(TurnStart);
            RoundManager.OnTurnEnd.AddListener(TurnEnd);
        }

        private void OnDestroy()
        {
            RoundManager.OnTurnEnd.RemoveListener(TurnEnd);
            RoundManager.OnTurnStart.RemoveListener(TurnStart);
        }

        private void TurnStart()
        {
            if (!shield) return;
            
            if (buildingType == BuildingType.ShieldGenerator)
            {
                shield.SetActive(side == Side.Player ? !RoundManager.IsPlayerTurn() : RoundManager.IsPlayerTurn());
            }
        }
        
        private void TurnEnd()
        {
            if (RoundManager.roundState != RoundState.Playing) return;
            if (damageableComponent.health <= 0f) return;
            
            charge += chargeRecovery;
            charge = Mathf.Clamp(charge, 0f, maxCharge);
        }

        public void Update()
        {
            if (RoundManager.selectedBuilding != this) return;
            var target = RoundManager.targetBuilding;
            if (target == null || horizontalAxis == null || verticalAxis == null) return;

            angle = RoundManager.angle;
            velocity = RoundManager.velocity;
            
            // Horizontal rotation: Smoothly look at the target in the horizontal plane only
            Vector3 directionToTarget = (target.transform.position - horizontalAxis.transform.position).normalized;
            directionToTarget.y = 0; // Ensure only horizontal rotation
            Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget); // Invert direction to face forward
            horizontalAxis.transform.rotation = Quaternion.Lerp(horizontalAxis.transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Vertical rotation: Set angle based on "angle" variable
            verticalAxis.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }



        public static Building[] GetAllBuildings() => GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None).ToArray();
        public static Building GetCore(Side side) => FindObjectsByType<Building>(FindObjectsSortMode.None).FirstOrDefault(x => x.buildingType == BuildingType.Core && x.side == side);
        public static Building[] GetAllWeapons(Side side) => GetAllBuildings().Where(x => x.side == side &&
            (x.buildingType == BuildingType.Railgun || x.buildingType == BuildingType.Turret)).ToArray();
        public static Building[] GetSelectableWeapons(Side side) =>
            GetAllWeapons(side).Where(x => x.damageableComponent.health > 0f).ToArray();
        public static List<Building> GetAliveBuildings(Side side) => GetAllBuildings().Where(x => x.side == side && x.damageableComponent?.health > 0f).ToList();
        public static List<Building> GetSelectableBuildings(Side side) => GetAliveBuildings(side).Where(x =>
            x.buildingType is
                (BuildingType.Turret or BuildingType.Railgun)).ToList();
        public Vector3 GetFiringPosition() => firePosition != null ? firePosition.position : transform.position;
        
        public IEnumerator IDisplayEffects()
        {
            var player = RoundManager.GetPlayer();
            player.SetTrackingTransform(gameObject.transform);
            yield return new WaitForSeconds(1f);
            // TODO: Show effects for stuff like recharging
            yield return new WaitForSeconds(0.5f);
        }
    }
}