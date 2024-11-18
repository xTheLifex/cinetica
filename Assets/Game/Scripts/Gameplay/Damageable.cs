using UnityEngine;
using UnityEngine.Events;

namespace Cinetica.Gameplay
{
    public class Damageable : MonoBehaviour
    {
        public int maxHealth = 3;
        public int health = 1;

        public bool godmode = false;
        
        public void Damage(int amount)
        {
            if (godmode) return;
            health = Mathf.Clamp(health - amount, 0, maxHealth);
            Debug.Log($"{gameObject.name} took {amount} damage.");
        }

        public void Heal(int amount)
        {
            health = Mathf.Clamp(health + amount, 0, maxHealth);
        }
    }
}