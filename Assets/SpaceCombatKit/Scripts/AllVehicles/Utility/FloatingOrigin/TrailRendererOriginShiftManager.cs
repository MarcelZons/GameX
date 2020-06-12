using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Clears trail renderers before a floating origin shift. Make sure all trail renderers to be cleared are children of the gameobject that this component is added to.
    /// Make sure that this component is a child of a FloatingOriginChild component.
    /// </summary>
    public class TrailRendererOriginShiftManager : MonoBehaviour, IFloatingOriginUser
    {
        // All the trail renderers that are children of this transform
        protected List<TrailRenderer> childTrailRenderers = new List<TrailRenderer>();

        // A reference to the floating origin manager
        protected FloatingOriginManager floatingOriginManager;
        public FloatingOriginManager FloatingOriginManager
        {
            set
            {
                this.floatingOriginManager = value;
            }
        }

        protected virtual void Awake()
        {
            // Collect references to all child trail renderers
            childTrailRenderers = new List<TrailRenderer>(transform.GetComponentsInChildren<TrailRenderer>());
        }

        /// <summary>
        /// Called just before a floating origin shift.
        /// </summary>
        public virtual void OnPreOriginShift()
        {
            // Clear the trails to prevent glitches/stretching after the scene is shifted.
            for(int i = 0; i < childTrailRenderers.Count; ++i)
            {
                childTrailRenderers[i].Clear();
            }
        }

        /// <summary>
        /// Called just after a floating origin shift.
        /// </summary>
        public virtual void OnPostOriginShift() { }

    }
}

