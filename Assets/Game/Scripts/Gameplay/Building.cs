using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinetica.Utility;
using UnityEngine;

namespace Cinetica.Gameplay
{
    [RequireComponent(typeof(Damageable))]
    public class Building : MonoBehaviour
    {
        public Side side = Side.Player;
        public Damageable damageableComponent;
        public BuildingType buildingType = BuildingType.Dummy;

        public float maxForce = 100f; // Turret: 100, Railgun: 1K
        public float maxAngle = 45f;
        public float minAngle = 0f;

        public float angle = 0f;
        public float force = 0f;

        public Transform horizontalAxis;
        public Transform verticalAxis;
        public Transform firePosition;
        
        private void Awake()
        {
            damageableComponent = GetComponent<Damageable>();
        }

        public void Update()
        {
            if (RoundManager.selectedBuilding != this) return;
            var target = RoundManager.targetBuilding;
            if (target == null || horizontalAxis == null || verticalAxis == null) return;

            angle = RoundManager.angle;
            force = RoundManager.force;
            
            // Horizontal rotation: Smoothly look at the target in the horizontal plane only
            Vector3 directionToTarget = (target.transform.position - horizontalAxis.transform.position).normalized;
            directionToTarget.y = 0; // Ensure only horizontal rotation
            Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget); // Invert direction to face forward
            horizontalAxis.transform.rotation = Quaternion.Lerp(horizontalAxis.transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Vertical rotation: Set angle based on "angle" variable
            verticalAxis.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }



        public static Building[] GetAllBuildings() => GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None).ToArray();

        public static Building[] GetAllWeapons(Side side) => GetAllBuildings().Where(x => x.side == side &&
            (x.buildingType == BuildingType.Railgun || x.buildingType == BuildingType.Turret)).ToArray();
        public static List<Building> GetAliveBuildings(Side side) => GetAllBuildings().Where(x => x.side == side && x.damageableComponent?.health > 0f).ToList();
        
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