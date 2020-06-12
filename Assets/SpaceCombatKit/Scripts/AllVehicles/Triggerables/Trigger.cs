using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{  

    [System.Serializable]
    public class Trigger
    {

        public int triggerIndex;

        public bool triggerSequentially = false;

        public float triggerInterval = 0.25f;

        [HideInInspector]
        public int lastTriggeredIndex = -1;

        [HideInInspector]
        public float lastTriggeredTime;

        [HideInInspector]
        public bool isTriggering = false;
        
    }
}