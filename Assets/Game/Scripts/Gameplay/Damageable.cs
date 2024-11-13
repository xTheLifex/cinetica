using UnityEngine;
using UnityEngine.Events;

namespace Cinetica.Gameplay
{
    public class Damageable : MonoBehaviour
    {
        public float maxHealth = 100f;
        public float health = 100f;

        public bool godmode = false;
        
        private UnityEvent OnZero = new UnityEvent();
        private UnityEvent OnDamaged = new UnityEvent();
        
        public void Damage(float amount)
        {
            if (godmode) return;
            health = Mathf.Clamp(health - amount, 0f, maxHealth);
        }

        public void Heal(float amount)
        {
            health = Mathf.Clamp(health + amount, 0f, maxHealth);
        }
    }
}