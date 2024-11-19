using Cinetica;
using Cinetica.Gameplay;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    public Vector3 velocity; // Initial velocity
    public float expiryTime = 10f; // Time before the projectile expires
    public Building owner; // The building that fired this projectile
    public float radius = 0.25f; // Radius of collision detection
    public AudioClip impactSound; // The impact sound effect.
    
    private int damage = 1;
    private bool launched = false; // Whether the projectile has been launched
    private float time = 0f; // Elapsed time since launch

    private AudioSource audSrc;
    private Vector3 initialPosition;

    private void Awake()
    {
        audSrc = GetComponent<AudioSource>();
    }

    public void Initialize(Vector3 initialVelocity, Building owner)
    {
        velocity = initialVelocity;
        launched = true;
        initialPosition = transform.position;
        this.owner = owner;
        this.damage = owner.damage;
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
            if (col.transform.parent)
            {
                var shield = col.transform.parent.GetComponent<Building>();
                if (shield)
                {
                    if (shield.buildingType == BuildingType.ShieldGenerator)
                    {
                        // We hit a shield generator.
                        if (shield.shieldCharge > 0)
                        {
                            shield.shieldCharge -= 1;
                            // hacky fix for AI shooting this even tho its disabled.
                            if (shield.shieldCharge <= 0)
                                shield.damageableComponent.Damage(shield.damageableComponent.maxHealth);
                            Explode();
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            
            var building = col.GetComponent<Building>();
            if (!building)
                building = col.transform.parent.GetComponent<Building>();
            
            if (building)
            {
                if (owner)
                {
                    // No self-collide
                    if (owner.gameObject == building.gameObject) continue;
                    
                    // No friendly fire.
                    if (owner.side == building.side) continue;
                }
                
                building.damageableComponent.Damage(damage);
                
            }
            Debug.Log($"{name} collided with {col.gameObject.name}");
            Explode();
            break;
        }
    }

    private void Explode()
    {
        // TODO: Effects
        if (impactSound) audSrc.PlayOneShot(impactSound);
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