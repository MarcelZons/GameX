using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Target selector for targets being tracked by a Tracker component;
    /// </summary>
    public class TrackerTargetSelector : TargetSelector
    {

        [SerializeField]
        protected Tracker tracker;
        

        protected override void Reset()
        {
            base.Reset();
            tracker = GetComponent<Tracker>();
        }

        protected virtual void Awake()
        {
            // Set the list of targets
            if (tracker != null)
            {
                trackables = tracker.Targets;
                tracker.onStoppedTracking.AddListener(OnStoppedTracking);
            }
        }
    }
}