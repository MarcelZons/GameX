using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class GunWeapon : MonoBehaviour
    {
        
        public float GetDamage(HealthType healthType)
        {
            DamageEmitter damageEmitter = GetComponentInChildren<DamageEmitter>();
            if (damageEmitter == null)
            {
                ProjectileLauncher projectileLauncher = GetComponentInChildren<ProjectileLauncher>();
                if (projectileLauncher != null)
                {
                    damageEmitter = projectileLauncher.ProjectilePrefab.GetComponent<DamageEmitter>();
                }
            }

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

        public float GetSpeed()
        {
            ProjectileLauncher projectileLauncher = GetComponentInChildren<ProjectileLauncher>();
            if (projectileLauncher != null)
            {
                return projectileLauncher.ProjectilePrefab.GetComponent<RigidbodyMover>().Velocity.magnitude;
            }

            BeamController beamController = GetComponentInChildren<BeamController>();
            if (beamController != null)
            {
                return float.MaxValue;
            }

            return 0;
        }
    }
}
