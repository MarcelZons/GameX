using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Add this to any object that needs to be shifted when the floating origin shifts.
    /// </summary>
    public class FloatingOriginChild : MonoBehaviour
    {
        // Get the floating position of this object.
        public Vector3 FloatingPosition
        {
            get { return (transform.position - FloatingOriginManager.Instance.transform.position); }
        }

        // List of all the floating origin users
        protected List<IFloatingOriginUser> floatingOriginUsers = new List<IFloatingOriginUser>();
        

        // Use this for initialization
        void Awake()
        {
            if (FloatingOriginManager.Instance != null)
            {
                // Register this floating origin child
                FloatingOriginManager.Instance.Register(this);
            }

            // Provide a reference to the scene origin manager transform for all floating origin users
            floatingOriginUsers = new List<IFloatingOriginUser>(transform.GetComponentsInChildren<IFloatingOriginUser>());
            foreach (IFloatingOriginUser floatingOriginUser in floatingOriginUsers)
            {
                floatingOriginUser.FloatingOriginManager = FloatingOriginManager.Instance;
            }
        }

        /// <summary>
        /// Called before the floating origin shifts.
        /// </summary>
        public virtual void OnPreOriginShift()
        {
            // Send the event to all the floating origin users
            for (int i = 0; i < floatingOriginUsers.Count; ++i)
            {
                floatingOriginUsers[i].OnPreOriginShift();
            }
        }

        /// <summary>
        /// Called after the floating origin shifts.
        /// </summary>
        public virtual void OnPostOriginShift()
        {
            // Send the event to all the floating origin users
            for (int i = 0; i < floatingOriginUsers.Count; ++i)
            {
                floatingOriginUsers[i].OnPostOriginShift();
            }
        }
    }
}