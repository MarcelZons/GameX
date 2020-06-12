using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Unity event for running functions when a collision occurs
    /// </summary>
    [System.Serializable]
    public class OnCollisionEnterEventHandler : UnityEvent<Collision> { }

    /// <summary>
    /// This class provides a vehicle with a Health component.
    /// </summary>
    public class Health : ModuleManager
    {

        // All the Damageables loaded onto this vehicle
        protected List<Damageable> damageables = new List<Damageable>();

        [Header("Settings")]

        [Tooltip("Whether this component should handle collision events in its OnCollisionEnter function.")]
        [SerializeField]
        protected bool handleCollisionEvents = true;
 
        [Header("Events")]

        // Collision event
        public OnCollisionEnterEventHandler onCollisionEnter;

   
        protected override void Awake()
        {
            base.Awake();
            DamageReceiver[] damageReceivers = transform.GetComponentsInChildren<DamageReceiver>();
            foreach(DamageReceiver damageReceiver in damageReceivers)
            {
                onCollisionEnter.AddListener(damageReceiver.OnCollision);
            }

            damageables = new List<Damageable>(transform.GetComponentsInChildren<Damageable>());
        }

        // Called when a collision occurs
        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (handleCollisionEvents)
            {
                // Call the collision event
                onCollisionEnter.Invoke(collision);
            }
        }

        /// <summary>
        /// Called every time a new module is mounted at a module mount.
        /// </summary>
        /// <param name="moduleMount">The module mount where the new module was loaded.</param>
        protected override void OnModuleMounted(Module module)
        {
            // Get a reference to any Damageable on the new module 
            Damageable damageable = module.GetComponent<Damageable>();
            if (damageable != null)
            {
                if (!damageables.Contains(damageable))
                {
                    damageables.Add(damageable);
                }
            }
        }


        /// <summary>
        /// Called every time a module is unmounted at a module mount.
        /// </summary>
        /// <param name="moduleMount">The module mount where the new module was unmounted.</param>
        protected override void OnModuleUnmounted(Module module)
        {

            // Remove any references to a Damageable on this module 
            Damageable damageable = module.GetComponent<Damageable>();
            if (damageable != null)
            {
                if (damageables.Contains(damageable))
                {
                    damageables.Remove(damageable);
                }
            }
        }
        
        /// <summary>
        /// Reset the health to starting conditions.
        /// </summary>
        public virtual void ResetHealth()
        {
            // Reset all of the damageables to starting conditions.
            foreach (Damageable damageable in damageables)
            {
                damageable.Restore();
            }
        }

        /// <summary>
        /// Get the maximum health for a specified health type.
        /// </summary>
        /// <param name="healthType">The health type being queried.</param>
        /// <returns>The maximum health.</returns>
        public virtual float GetMaxHealthByType(HealthType healthType)
        {
            float maxHealth = 0;

            for (int i = 0; i < damageables.Count; ++i)
            {
                if (damageables[i].HealthType == healthType)
                {
                    maxHealth += damageables[i].HealthCapacity;
                }
            }

            return maxHealth;
        }


        /// <summary>
        /// Get the current health for a specified health type.
        /// </summary>
        /// <param name="healthType">The health type being queried.</param>
        /// <returns>The current health.</returns>
        public virtual float GetCurrentHealthByType(HealthType healthType)
        {
            float currentHealth = 0;

            for (int i = 0; i < damageables.Count; ++i)
            {
                if (damageables[i].HealthType == healthType)
                {
                    currentHealth += damageables[i].CurrentHealth;
                }
            }

            return currentHealth;
        }

        /// <summary>
        /// Get the fraction of health remaining of a specified type.
        /// </summary>
        /// <param name="healthType">The health type.</param>
        /// <returns>The health fraction remaining</returns>
        public virtual float GetCurrentHealthFractionByType(HealthType healthType)
        {

            float currentHealth = 0;
            float maxHealth = 0.00001f;

            for (int i = 0; i < damageables.Count; ++i)
            {
                if (damageables[i].HealthType == healthType)
                {
                    currentHealth += damageables[i].CurrentHealth;
                    maxHealth += damageables[i].HealthCapacity;
                }
            }

            return currentHealth / maxHealth;
        }
    }
}