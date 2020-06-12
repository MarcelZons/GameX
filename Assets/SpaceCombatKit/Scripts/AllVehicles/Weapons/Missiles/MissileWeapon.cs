using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Base class for a missile weapon
    /// </summary>
    public class MissileWeapon : MonoBehaviour, IRootTransformUser
    {

        [SerializeField]
        protected TargetLocker targetLocker;

        [SerializeField]
        protected ProjectileLauncher projectileLauncher;

        // Store information about the module owner
        protected Transform ownerRootTransform;
        protected Rigidbody ownerRigidbody;



        public void SetRootTransform(Transform rootTransform)
        {
            this.ownerRootTransform = rootTransform;
        }

        /// <summary>
        /// Event called when a missile is launched.
        /// </summary>
        /// <param name="missileObject">The missile gameobject</param>
        public void OnMissileLaunched(GameObject missileObject)
        {
            Missile missile = missileObject.GetComponent<Missile>();
            if (missile == null)
            {
                Debug.LogWarning("Launched missile has no Missile component. Please add one.");
            }
            else
            {
                // Set missile parameters
                missile.SetTarget(targetLocker.Target);
                missile.SetLockState(targetLocker.LockState == LockState.Locked ? LockState.Locked : LockState.NoLock);
                missile.SetSenderRootTransform(ownerRootTransform);
                if (ownerRigidbody != null) missile.SetVelocity(ownerRigidbody.velocity);
            }
        }


        public float GetMissileDamage(HealthType healthType)
        {
            DamageEmitter damageEmitter = projectileLauncher.ProjectilePrefab.GetComponentInChildren<DamageEmitter>();
            if (damageEmitter != null)
            {
                for (int i = 0; i < damageEmitter.HealthTypeDamageValues.Count; ++i)
                {
                    if (damageEmitter.HealthTypeDamageValues[i].HealthType == healthType)
                    {
                        return damageEmitter.HealthTypeDamageValues[i].DamageValue;
                    }
                }
                return 0;
            }
            else
            {
                return 0;
            }
        }


        public float GetMissileRange()
        {
            return projectileLauncher.ProjectilePrefab.GetComponent<TargetLocker>().LockingRange;
        }


        public float GetMissileSpeed()
        {
            return projectileLauncher.ProjectilePrefab.GetComponent<VehicleEngines3D>().GetDefaultMaxSpeedByAxis(false).magnitude;
        }


        public float GetMissileAgility()
        {
            return projectileLauncher.ProjectilePrefab.GetComponent<VehicleEngines3D>().MaxSteeringForces.magnitude;
        }
    }
}