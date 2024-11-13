using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        private void Awake()
        {
            damageableComponent = GetComponent<Damageable>();
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