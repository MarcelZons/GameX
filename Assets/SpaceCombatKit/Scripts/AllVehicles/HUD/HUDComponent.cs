using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public enum HUDAnchorType
    {
        AnchorTransform,
        Camera,
        None
    }

    // Stores whether to activate or deactivate the HUD Component for a specific view.
    [System.Serializable]
    public class HUDComponentCameraViewSettings
    {
        public VehicleCameraView cameraView;
        public bool shownInView = true;
        public HUDAnchorType anchorTypeForView;
        public Transform anchorTransform;
        public Vector3 positionOffset;
    }

    /// <summary>
    /// Base class for a managing different sections of the HUD.
    /// </summary>
    public class HUDComponent : MonoBehaviour
    {

        [Header("HUD Component")]

        [SerializeField]
        protected Camera hudCamera;
        public Camera HUDCamera { get { return hudCamera; } }

        [SerializeField]
        protected List<HUDComponentCameraViewSettings> cameraViewSettings = new List<HUDComponentCameraViewSettings>();
        public List<HUDComponentCameraViewSettings> CameraViewSettings { get { return cameraViewSettings; } }

        protected bool activated = false;
        public bool Activated { get { return activated; } }


        protected virtual void Reset()
        {
            VehicleCameraView[] views = (VehicleCameraView[])System.Enum.GetValues(typeof(VehicleCameraView));
            foreach (VehicleCameraView view in views)
            {
                HUDComponentCameraViewSettings settings = new HUDComponentCameraViewSettings();

                settings.cameraView = view;
                settings.shownInView = true;
                settings.anchorTypeForView = HUDAnchorType.AnchorTransform;
                settings.anchorTransform = transform.parent;
                settings.positionOffset = transform.localPosition;

                cameraViewSettings.Add(settings);
            }
        }


        public virtual void SetCamera(Camera hudCamera)
        {
            this.hudCamera = hudCamera;
        }

        /// <summary>
        /// Activate this HUD Component
        /// </summary>
        public virtual void Activate()
        {
            gameObject.SetActive(true);
            activated = true;
        }

        /// <summary>
        /// Deactivate this HUD component
        /// </summary>
        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
            activated = false;
        }

        /// <summary>
        /// Called to update this HUD Component.
        /// </summary>
        public virtual void OnUpdateHUD() { }

        public virtual void OnCameraViewChanged(CameraViewTarget newCameraViewTarget) { }
       
    }
}