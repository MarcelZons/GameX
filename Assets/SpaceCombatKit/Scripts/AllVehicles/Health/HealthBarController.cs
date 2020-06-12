using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Controls a health bar in the UI.
    /// </summary>
    public class HealthBarController : MonoBehaviour
    {

        [SerializeField]
        protected Health health;

        [SerializeField]
        protected Image healthBar;

        [SerializeField]
        protected HealthType healthType;

        // Called every frame
        private void Update()
        {
            // Update health bar
            healthBar.fillAmount = health.GetCurrentHealthFractionByType(healthType);
        }
    }
}
