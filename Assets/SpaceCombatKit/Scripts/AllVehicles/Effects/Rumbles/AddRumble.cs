using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class AddRumble : MonoBehaviour
    {

        [SerializeField]
        protected float maxRumbleLevel = 1;

        private void OnEnable()
        {
            // Add a rumble
            if (RumbleManager.Instance != null)
            {
                RumbleManager.Instance.AddRumble(transform.position, maxRumbleLevel, 0.05f, 0.2f, 0.6f);
            }
        }
    }
}