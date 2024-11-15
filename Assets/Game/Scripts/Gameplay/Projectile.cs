using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Cinetica.Gameplay
{
    public class Projectile : MonoBehaviour
    {
        public float radius = 0.5f;
        public UnityEvent OnHit = new UnityEvent();
        public Building owner;

        public float expiryTime = 30f;

        private float velocity;
        private float angle;
        private float time = 0f;
        private bool launched = false;

        public void Initialize(float angle, float force)
        {
            this.velocity = force;
            this.angle = angle;
            launched = true;
        }

        public void Update()
        {
            expiryTime -= Time.deltaTime;
            if (expiryTime <= 0f)
            {
                Debug.LogWarning($"Projectile {name} has expired. This shouldn't happen.");
                Explode();
            }
            
            if (launched) return;
            time += Time.deltaTime;
        }

        public void Explode()
        {
            Destroy(gameObject);
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}