using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// An interface for any component that needs to have a reference to the floating origin and/or subscribe to the pre- and post- origin shift events
    /// </summary>
    public interface IFloatingOriginUser
    {
        /// <summary>
        /// A reference to
        /// </summary>
	    FloatingOriginManager FloatingOriginManager { set; }

        /// <summary>
        /// Called just before the floating origin shifts
        /// </summary>
        void OnPreOriginShift();

        /// <summary>
        /// Called just after the floating origin shifts
        /// </summary>
        void OnPostOriginShift();
    }
}
