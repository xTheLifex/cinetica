using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 velocity; // Initial velocity
    public float expiryTime = 5f; // Time before the projectile expires
    private bool launched = false; // Whether the projectile has been launched
    private float time = 0f; // Elapsed time since launch

    public void Initialize(Vector3 initialVelocity)
    {
        velocity = initialVelocity;
        launched = true;
    }

    public void Update()
    {
        expiryTime -= Time.deltaTime;
        if (expiryTime <= 0f)
        {
            Debug.LogWarning($"Projectile {name} has expired. This shouldn't happen.");
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
    }

    private void Explode()
    {
        // Explosion logic here
        Debug.Log($"{name} exploded!");
        Destroy(gameObject);
    }
}