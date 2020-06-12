using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Base class for a script that controls the camera for a specific type of vehicle.
    /// </summary>
    public class VehicleCameraController : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("Whether to specify the vehicle classes that this camera controller is for.")]
        [SerializeField]
        protected bool specifyCompatibleVehicleClasses;

        [Tooltip("The vehicle classes that this camera controller is compatible with.")]
        [SerializeField]
        protected List<VehicleClass> compatibleVehicleClasses = new List<VehicleClass>();

        // A reference to the vehicle camera
        protected VehicleCamera vehicleCamera;
        public VehicleCamera VehicleCamera { set { vehicleCamera = value; } }

        [Header("Starting Values")]

        [Tooltip("The camera view that is shown upon entering the vehicle.")]
        [SerializeField]
        protected VehicleCameraView startingView;

        [Tooltip("Whether to default to the first available view, if the startingView value is not set.")]
        [SerializeField]
        protected bool defaultToFirstAvailableView = true;

        // Whether this camera controller is currently activated
        protected bool controllerActive = false;
        public bool ControllerActive { get { return controllerActive; } }

        // Whether this camera controller is ready to be activated
        protected bool initialized = false;
        public bool Initialized { get { return initialized; } }

        
        /// <summary>
        /// Called to activate this camera controller (for example when the Vehicle Camera's target vehicle changes).
        /// </summary>
        public virtual void StartController()
        {
            // If this camera controller is ready, activate it.
            if (initialized) controllerActive = true;  
        }


        /// <summary>
        /// Called to deactivate this camera controller.
        /// incompatible vehicle.
        /// </summary>
        public virtual void StopController()
        {
            controllerActive = false;
        }

        /// <summary>
        /// Set the target vehicle for this camera controller.
        /// </summary>
        /// <param name="vehicle">The target vehicle.</param>
        /// <param name="startController">Whether to start the controller immediately.</param>
        public virtual void SetVehicle(Vehicle vehicle, bool startController)
        {

            StopController();

            initialized = false;

            if (vehicle == null) return;

            if (Initialize(vehicle))
            {
                initialized = true;
                vehicleCamera.SetView(startingView);
                if (startController) StartController();
            }
        }


        /// <summary>
        /// Initialize this camera controller with a vehicle.
        /// </summary>
        /// <param name="vehicle">Whether the camera controller succeeded in initializing.</param>
        /// <returns></returns>
        protected virtual bool Initialize(Vehicle vehicle)
        {          
            // If compatible vehicle classes are specified, check that the list contains this vehicle's class.
            if (specifyCompatibleVehicleClasses)
            {
                if (compatibleVehicleClasses.IndexOf(vehicle.VehicleClass) != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        protected virtual void CameraControllerFixedUpdate() { }
        protected virtual void FixedUpdate()
        {
            // If not activated or no camera view target selected, exit.
            if (!controllerActive || !vehicleCamera.HasCameraViewTarget) return;

            CameraControllerFixedUpdate();
        }

        protected virtual void CameraControllerUpdate() { }
        protected virtual void Update()
        {
            // If not activated or no camera view target selected, exit.
            if (!controllerActive || !vehicleCamera.HasCameraViewTarget) return;

            CameraControllerUpdate();
        }

        protected virtual void CameraControllerLateUpdate() { }
        protected virtual void LateUpdate()
        {
            // If not activated or no camera view target selected, exit.
            if (!controllerActive || !vehicleCamera.HasCameraViewTarget) return;

            CameraControllerLateUpdate();
        }
    }
}