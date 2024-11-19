using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cinetica.Gameplay
{
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent((typeof(Animator)))]
    public class Building : MonoBehaviour
    {
        public Side side = Side.Player;
        public Damageable damageableComponent;
        public BuildingType buildingType = BuildingType.Dummy;

        [FormerlySerializedAs("maxForce")] public float maxVelocity = 100f; // Turret: 100, Railgun: 1K
        public float minVelocity = 5f;
        public float maxAngle = 45f;
        public float minAngle = 0f;

        public float angle = 0f;
        [FormerlySerializedAs("velocity")] public float velocity = 0f;

        public Transform horizontalAxis;
        public Transform verticalAxis;
        public Transform firePosition;
        public Transform aimModePosition;

        public int shieldCharge = 3;
        public int damage = 1;

        public GameObject shield;

        public Material activeCoreMaterial;
        public Material deadCoreMaterial;
        
        private AudioSource audioSource;
        private Animator animator;
        private bool wasDamaged = false; // Was damaged this turn?
        private bool wasDestroyed = false; // Was destroyed this turn?
        
        public AudioClip fireSound; // If it's the core, this is the damage sound instead.
        public AudioClip explodeSound; // Sound when destroyed
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            damageableComponent = GetComponent<Damageable>();
            RoundManager.OnTurnStart.AddListener(TurnStart);
            RoundManager.OnTurnEnd.AddListener(TurnEnd);
            damageableComponent.OnTakeDamage.AddListener(TakeDamage);
            damageableComponent.OnZero.AddListener(Destroyed);
        }

        private void TakeDamage(int amount, int newHP)
        {
            wasDamaged = true;
        }

        private void Destroyed()
        {
            wasDestroyed = true;
        }
        

        private void OnDestroy()
        {
            RoundManager.OnTurnEnd.RemoveListener(TurnEnd);
            RoundManager.OnTurnStart.RemoveListener(TurnStart);
            if (!damageableComponent) return;
            damageableComponent.OnTakeDamage.RemoveListener(TakeDamage);
            damageableComponent.OnZero.RemoveListener(Destroyed);
        }

        private void TurnStart()
        {
            wasDamaged = false;
            wasDestroyed = false;
            
            // Shield stuff
            if (!shield) return;
            if (buildingType == BuildingType.ShieldGenerator)
            {
                if (shieldCharge > 0)
                    shield.SetActive(side == Side.Player ? !RoundManager.IsPlayerTurn() : RoundManager.IsPlayerTurn());
                else
                    shield.SetActive(false);
            }
        }
        
        private void TurnEnd()
        {
            
        }

        public void Update()
        {
            if (RoundManager.selectedBuilding != this) return;
            var target = RoundManager.targetBuilding;
            if (target == null || horizontalAxis == null || verticalAxis == null) return;

            angle = RoundManager.angle;
            velocity = RoundManager.velocity;
            
            #if UNITY_EDITOR
            var prefab = RoundManager.GetProjectilePrefab();
            if (prefab)
            {
                var projectile = prefab.GetComponent<Projectile>();
                if (projectile)
                {
                    SimulateShot(this, target, angle, velocity, projectile.radius, projectile.expiryTime, true);
                }
            }
            #endif
                
            AdjustTowards(target);
        }

        public void AdjustTowards(Building target)
        {
            // Horizontal rotation: Smoothly look at the target in the horizontal plane only
            Vector3 directionToTarget = (target.transform.position - horizontalAxis.transform.position).normalized;
            directionToTarget.y = 0; // Ensure only horizontal rotation
            Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget); // Invert direction to face forward
            horizontalAxis.transform.rotation = Quaternion.Lerp(horizontalAxis.transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Vertical rotation: Set angle based on "angle" variable
            verticalAxis.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }



        public static Building[] GetAllBuildings() => GameObject.FindObjectsByType<Building>(FindObjectsSortMode.None).ToArray();
        public static Building GetCore(Side side) => FindObjectsByType<Building>(FindObjectsSortMode.None).FirstOrDefault(x => x.buildingType == BuildingType.Core && x.side == side);
        public static Building[] GetAllWeapons(Side side) => GetAllBuildings().Where(x => x.side == side &&
            (x.buildingType == BuildingType.Railgun || x.buildingType == BuildingType.Turret)).ToArray();
        public static List<Building> GetAliveBuildings(Side side) => GetAllBuildings().Where(x => x.side == side && x.damageableComponent?.health > 0).ToList();
        public static List<Building> GetSelectableBuildings(Side side) => GetAliveBuildings(side).Where(x =>
            x.buildingType is
                (BuildingType.Turret or BuildingType.Railgun)).ToList();
        public Vector3 GetFiringPosition() => firePosition != null ? firePosition.position : transform.position;
        public IEnumerator IDisplayEffects()
        {
            var player = RoundManager.GetPlayer();
            
            if (wasDamaged && buildingType is BuildingType.Core)
            {
                if (!activeCoreMaterial || !activeCoreMaterial) yield break;
                
                var hp = damageableComponent.health;
                if (hp >= 3) yield break;
                
                var blockout = transform.Find("Blockout");
                if (!blockout) yield break;

                var life1 = blockout.Find("Life 1");
                var life2 = blockout.Find("Life 2");
                if (!life1 || !life2) yield break;

                var life1mr = life1.GetComponent<MeshRenderer>();
                var life2mr = life2.GetComponent<MeshRenderer>();
                if (!life1mr || !life2mr) yield break;
                
                player.SetTrackingTransform(gameObject.transform);
                yield return new WaitForSeconds(1f);
                
                if (hp == 2)
                {
                    life1mr.material = deadCoreMaterial;
                    life2mr.material = activeCoreMaterial;
                }
                if (hp <= 1)
                {
                    life1mr.material = deadCoreMaterial;
                    life2mr.material = deadCoreMaterial;
                }
                
                if (fireSound)
                {
                    audioSource.PlayOneShot(fireSound);
                    yield return new WaitForSeconds(fireSound.length);
                }
                
            }

            if (wasDestroyed)
            {
                animator.enabled = true;
                player.SetTrackingTransform(gameObject.transform);
                yield return new WaitForSeconds(0.5f);
                if (explodeSound) audioSource.PlayOneShot(explodeSound);
                if (buildingType is BuildingType.Core) animator.Play("Core Explode");
                if (buildingType is BuildingType.Turret) animator.Play("Turret Explode");
                if (buildingType is BuildingType.Railgun) animator.Play("Railgun Explode");
                if (horizontalAxis)
                {
                    var explosion = horizontalAxis.Find("Explosion");
                    var fire = horizontalAxis.Find("Fire");
                    if (explosion)
                        explosion.gameObject.SetActive(true);
                    if (fire)
                        fire.gameObject.SetActive(true);
                }
                
                yield return new WaitForSeconds(1.5f);
                //animator.enabled = false;
            }

            yield return null;
        }
        
        /// <summary>
        /// Simulates a projectile shot to check if it hits the target.
        /// </summary>
        /// <param name="weapon">The firing weapon.</param>
        /// <param name="target">The target building.</param>
        /// <param name="angle">The firing angle.</param>
        /// <param name="velocity">The firing velocity.</param>
        /// <param name="radius">The projectile's collision radius.</param>
        /// <param name="expireTime">The projectile's expiry time.</param>
        /// <param name="debugDraw">If true, draws debug lines in the editor.</param>
        /// <returns>True if the shot hits the target; otherwise, false.</returns>
        public static bool SimulateShot(
            Building weapon,
            Building target,
            float angle,
            float velocity,
            float radius = 0.5f,
            float expireTime = 5f,
            bool debugDraw = false,
            float debugTime = 0f)
        {
            const float timeStep = 0.05f;
            var horizontalRotation = weapon.horizontalAxis.rotation;
            var verticalRotation = Quaternion.Euler(angle, 0f, 0f);
            var fireRotation = horizontalRotation * verticalRotation;
            var simulatedPosition = weapon.GetFiringPosition();
            var simulatedVelocity = fireRotation * Vector3.back * velocity;

            for (var time = 0f; time < expireTime; time += timeStep)
            {
                var lastPos = simulatedPosition;
                simulatedVelocity += Physics.gravity * timeStep;
                simulatedPosition += simulatedVelocity * timeStep;

                #if UNITY_EDITOR
                if (debugDraw)
                    Debug.DrawLine(lastPos, simulatedPosition, Color.red, debugTime);
                #endif

                weapon.AdjustTowards(target);
                foreach (var col in Physics.OverlapSphere(simulatedPosition, radius))
                {
                    var parent = col.transform.parent;
                    if (col.CompareTag("Building"))
                        continue;
                    if (parent)
                    {
                        var building = parent.GetComponent<Building>();
                        if (building && !col.transform.CompareTag("AI Target"))
                        {
                            // We hit a shield. Just calculate past it.
                            if (building.buildingType == BuildingType.ShieldGenerator)
                                continue;
                        }
                        
                        if (building && building.side == Side.Player && building == target
                            && col.transform.CompareTag("AI Target"))
                        {
                            #if UNITY_EDITOR
                            if (debugDraw)
                                Debug.DrawLine(simulatedPosition, simulatedPosition + Vector3.up * 5f, Color.green, debugTime);
                            #endif
                            return true;
                        }
                    }

                    return false;
                }
            }

            return false;
        }
    }
}