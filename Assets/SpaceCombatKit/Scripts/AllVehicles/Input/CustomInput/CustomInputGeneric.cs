using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Create custom input using Unity Events.
    /// </summary>
    public class CustomInputGeneric : MonoBehaviour
    {

        [SerializeField]
        protected List<CustomInputEventItem> inputItems = new List<CustomInputEventItem>();

        // Update is called once per frame
        void Update()
        {
            // Run the input items
            for (int i = 0; i < inputItems.Count; ++i)
            {
                inputItems[i].ProcessEvents();
            }
        }
    }
}