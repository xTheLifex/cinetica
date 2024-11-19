using UnityEngine;
using UnityEngine.Events;

namespace Cinetica.Gameplay
{
    public class Damageable : MonoBehaviour
    {
        public int maxHealth = 3;
        public int health = 1;

        public bool godmode = false;

        public UnityEvent<int, int> OnTakeDamage = new UnityEvent<int, int>();
        public UnityEvent<int, int> OnHeal = new UnityEvent<int, int>();
        public UnityEvent OnZero = new UnityEvent();
        
        public void Damage(int amount)
        {
            var lastHealth = health;
            if (godmode) return;
            health = Mathf.Clamp(health - amount, 0, maxHealth);
            
            if (Mathf.Abs(health - lastHealth) <= 0) return; // Don't call events if nothing changed.
            Debug.Log($"{gameObject.name} took {amount} damage.");
            OnTakeDamage?.Invoke(amount, health);
            if (health <= 0)
                OnZero?.Invoke();
        }

        public void Heal(int amount)
        {
            var lastHealth = health;
            health = Mathf.Clamp(health + amount, 0, maxHealth);
            if (Mathf.Abs(health - lastHealth) <= 0) return; // Don't call events if nothing changed.
            OnHeal?.Invoke(amount, health);
        }
    }
}