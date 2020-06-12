using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using UnityEngine.Events;
using VSX.Pooling;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Unity event for running functions when a projectile is launched by a projectile launcher
    /// </summary>
    [System.Serializable]
    public class OnProjectileLauncherProjectileLaunchedEventHandler : UnityEvent<GameObject> { }

    /// <summary>
    /// This class spawns a projectile prefab at a specified interval and with a specified launch velocity.
    /// </summary>
    public class ProjectileLauncher : MonoBehaviour, IRootTransformUser
    {

        [Header("Settings")]

        [SerializeField]
        protected Transform spawnPoint;

        [SerializeField]
        protected GameObject projectilePrefab;
        public GameObject ProjectilePrefab { get { return projectilePrefab; } }

        [SerializeField]
        protected float fireInterval = 0.15f;
        protected float lastFire;
        protected bool triggering = false;

        [SerializeField]
        protected bool usePoolManager;

        [Header("Events")]

        // Projectile launched event
        public OnProjectileLauncherProjectileLaunchedEventHandler onProjectileLaunched;

        protected Transform rootTransform;


        protected virtual void Reset()
        {
            spawnPoint = transform;
        }

        protected virtual void Awake()
        {
            // Allow immediate firing
            lastFire = -fireInterval;
        }

        protected virtual void Start()
        {

            if (rootTransform == null) rootTransform = transform.root;

            if (usePoolManager && PoolManager.Instance == null)
            {
                usePoolManager = false;
                Debug.LogWarning("No PoolManager component found in scene, please add one to pool projectiles.");
            }
        }


        public void SetRootTransform(Transform rootTransform)
        {
            this.rootTransform = rootTransform;
        }


        /// <summary>
        /// Start triggering the projectile launcher
        /// </summary>
        public virtual void StartTriggering()
        {
            triggering = true;
        }

        /// <summary>
        /// Stop triggering the projectile launcher
        /// </summary>
        public virtual void StopTriggering()
        {
            triggering = false;
        }

        /// <summary>
        /// Trigger the projectile launcher once
        /// </summary>
        public virtual void TriggerOnce()
        {
            if (CanFire()) LaunchProjectile();
        }

        // Check if firing is possible
        protected virtual bool CanFire()
        {
            if (Time.time - lastFire < fireInterval) return false;

            return true;
        }

        // Launch a projectile
        public virtual void LaunchProjectile()
        {
            // Get/instantiate the projectile
            GameObject projectileObject;
            
            if (usePoolManager)
            {
                projectileObject = PoolManager.Instance.Get(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                projectileObject = GameObject.Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            }

            lastFire = Time.time;

            Projectile projectileController = projectileObject.GetComponent<Projectile>();
            if (projectileController != null)
            {
                projectileController.SetSenderRootTransform(rootTransform);
            }

            // Call the event
            onProjectileLaunched.Invoke(projectileObject);

        }

        // Called every frame
        private void Update()
        {
            if (triggering && CanFire()) LaunchProjectile();
        }
    }
}