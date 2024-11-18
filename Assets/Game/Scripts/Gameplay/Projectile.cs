using System;
using Cinetica;
using Cinetica.Gameplay;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity; // Initial velocity
    public float expiryTime = 10f; // Time before the projectile expires
    public Building owner; // The building that fired this projectile
    public float baseDamage = 25; // Amout of damage this gun should cause
    public float radius = 0.25f; // Radius of collision detection
    
    private bool launched = false; // Whether the projectile has been launched
    private float time = 0f; // Elapsed time since launch

    
    private Vector3 initialPosition;
    public void Initialize(Vector3 initialVelocity, Building owner)
    {
        velocity = initialVelocity;
        launched = true;
        initialPosition = transform.position;
        this.owner = owner;
    }

    public void Update()
    {
        expiryTime -= Time.deltaTime;
        if (expiryTime <= 0f)
        {
            Explode();
            return;
        }

        if (!launched) return;

        // Update time
        time += Time.deltaTime;

        // Apply gravity
        velocity += Physics.gravity * Time.deltaTime;

        // Update position based on velocity
        transform.position += velocity * Time.deltaTime;
        
        foreach (var col in Physics.OverlapSphere(transform.position, radius))
        {
            var shield = col.transform.parent.GetComponent<Building>();
            if (shield)
            {
                if (shield.buildingType == BuildingType.ShieldGenerator)
                {
                    // We hit a shield generator.
                    if (shield.charge > 0f)
                    {
                        shield.charge -= baseDamage;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            
            var building = col.GetComponent<Building>();
            if (building)
            {
                if (owner)
                {
                    // No self-collide
                    if (owner.gameObject == building.gameObject) continue;
                    
                    // No friendly fire.
                    if (owner.side == building.side) continue;
                }
                
                building.damageableComponent.Damage(velocity.magnitude * baseDamage);
                
            }
            Debug.Log($"{name} collided with {col.gameObject.name}");
            Explode();
        }
    }

    private void Explode()
    {
        // TODO: Effects
        Destroy(gameObject);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}