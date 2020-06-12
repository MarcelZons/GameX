using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    [System.Serializable]
    public class AuxiliaryCameraViewSettings
    {
        [Tooltip("The camera view that these settings will be applied to.")]
        public VehicleCameraView view;

        [Tooltip("Whether to copy the field of view of the main camera.")]
        public bool copyFieldOfView;
    }
}
