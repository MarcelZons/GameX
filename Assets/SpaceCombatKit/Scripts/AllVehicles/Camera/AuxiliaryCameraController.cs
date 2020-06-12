using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// This class manages an auxiliary camera, which is any additional camera in your scene that must follow changes in the settings of the main camera.
    /// For example, a background camera that shows the environment.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class AuxiliaryCameraController : MonoBehaviour
    {

        protected Camera cachedCamera;

        [Tooltip("The settings for this camera for each Vehicle Camera View.")]
        [SerializeField]
        protected List<AuxiliaryCameraViewSettings> viewSettings = new List<AuxiliaryCameraViewSettings>();

        protected VehicleCameraView currentView;


        protected void Awake()
        {
            cachedCamera = GetComponent<Camera>();
        }

        /// <summary>
        /// Called when the camera view target changes on the vehicle camera.
        /// </summary>
        /// <param name="newCameraViewTarget"></param>
        public void OnCameraViewTargetChanged(CameraViewTarget newCameraViewTarget)
        {
            if (newCameraViewTarget != null) currentView = newCameraViewTarget.CameraView;
        }

        /// <summary>
        /// Called when the field of view on the main camera changes.
        /// </summary>
        /// <param name="newFieldOfView">The new field of view.</param>
        public void OnFieldOfViewChanged(float newFieldOfView)
        {
            for (int i = 0; i < viewSettings.Count; ++i)
            {
                if (viewSettings[i].view == currentView && viewSettings[i].copyFieldOfView)
                {
                    cachedCamera.fieldOfView = newFieldOfView;
                }
            }
        }
    }
}