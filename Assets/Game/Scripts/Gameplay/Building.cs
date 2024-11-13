using System;
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

        private void Awake()
        {
            damageableComponent = GetComponent<Damageable>();
        }

        public static Building[] GetAllBuildings() => GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None).ToArray();
        public static List<Building> GetSelectable(Side side) => GetAllBuildings().Where(x => x.side == side && x.damageableComponent.health > 0f).ToList();
    }
}