using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Associates a health type with a damage value (e.g. for weapon damage to different health types).
    /// </summary>
    [System.Serializable]
    public class HealthTypeDamageValue
    {
        [SerializeField]
        private HealthType healthType;
        public HealthType HealthType { get { return healthType; } }

        [SerializeField]
        private float damageValue;
        public float DamageValue { get { return damageValue; } }

        public HealthTypeDamageValue(HealthType healthType, float damageValue)
        {
            this.healthType = healthType;
            this.damageValue = damageValue;
        }
    }
}
