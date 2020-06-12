using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class PoweredDamageable : Damageable, IPowerConsumer
    {

        Power power;
        public Power Power { set { power = value; } }

        public float rechargeRate;


        private void Update()
        {
            if (power == null) return;
            
            if (destroyed && !canHealAfterDestroyed) return;

            if (!Mathf.Approximately(currentHealth, startingHealth))
            {
                float diff = startingHealth - currentHealth;
                float recharge = rechargeRate * Time.deltaTime;
                recharge = Mathf.Min(recharge, diff);
                recharge = Mathf.Min(recharge, power.GetStoredPower(PoweredSubsystemType.Health));

                power.DrawStoredPower(PoweredSubsystemType.Health, recharge);
                currentHealth += recharge;
                currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);
            }
        }
    }
}