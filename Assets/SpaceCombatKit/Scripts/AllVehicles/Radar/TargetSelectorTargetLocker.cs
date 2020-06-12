using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// A target locker that locks onto a target selector's selected target.
    /// </summary>
    public class TargetSelectorTargetLocker : TargetLocker
    {
        // The target selector
        [SerializeField]
        protected TargetSelector targetSelector;
        public TargetSelector TargetSelector
        {
            get { return targetSelector; }
            set { targetSelector = value; }
        }
    }
}
