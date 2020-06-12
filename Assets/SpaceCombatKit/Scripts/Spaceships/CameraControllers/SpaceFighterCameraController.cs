using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Space
{
    /// <summary>
    /// This class controls the camera for a space fighter.
    /// </summary>
    public class SpaceFighterCameraController : VehicleCameraController
    {
      
        [Header("Boost Effects")]

        // The field of view to use when boosting.
        [SerializeField]
        private float boostFieldOfView = 75;

        // How fast the field of view changes when boost is activated or deactivated.
        [SerializeField]
        private float boostFieldOfViewLerpSpeed = 0.1f;

        // Cached references for necessary parts of the vehicle
        private Rigidbody vehicleRigidbody;
        private Engines vehicleEngines;

        
        /// <summary>
        /// Activate this vehicle camera controller.
        /// </summary>
        protected override bool Initialize(Vehicle vehicle)
        {

            if (!base.Initialize(vehicle)) return false;

            // Cache references
            vehicleRigidbody = vehicle.GetComponent<Rigidbody>();
            vehicleEngines = vehicle.GetComponent<Engines>();

            return true;
        }


        // Physics update
        protected override void CameraControllerFixedUpdate()
        {
            
            float spinLateralOffset = 0;
            if (vehicleRigidbody != null)
            {
                // Calculate the lateral offset from the camera view target based on the spin.
                spinLateralOffset = vehicleCamera.SelectedCameraViewTarget.SpinOffsetCoefficient *
                                            vehicleCamera.SelectedCameraViewTarget.transform.InverseTransformDirection(vehicleRigidbody.angularVelocity).z;
            }

            // Calculate the target position for the camera 
            Vector3 targetPosition = vehicleCamera.SelectedCameraViewTarget.transform.TransformPoint(new Vector3(-spinLateralOffset, 0f, 0f));

            // Lerp toward the target position
            vehicleCamera.transform.position = (1 - vehicleCamera.SelectedCameraViewTarget.PositionFollowStrength) * vehicleCamera.transform.position +
                                        vehicleCamera.SelectedCameraViewTarget.PositionFollowStrength * targetPosition;

            // Spherically interpolate between the current rotation and the target rotation.
            vehicleCamera.transform.rotation = Quaternion.Slerp(transform.rotation, vehicleCamera.SelectedCameraViewTarget.transform.rotation,
                                                    vehicleCamera.SelectedCameraViewTarget.RotationFollowStrength);        
            
        }

    
        // Called every frame
        protected override void CameraControllerUpdate()
        {

            // Update boost field of view effects. 
            if (vehicleEngines != null  && vehicleEngines.EnginesActivated == true)
            {
                float targetFieldOfView = vehicleEngines.BoostInputs.z * boostFieldOfView + 
                                    (1 - vehicleEngines.BoostInputs.z) * vehicleCamera.DefaultFieldOfView;

                // Set the new field of view
                vehicleCamera.SetFieldOfView(Mathf.Lerp(vehicleCamera.MainCamera.fieldOfView, targetFieldOfView, boostFieldOfViewLerpSpeed));
                
            }

            // If position and/or rotation are locked for the selected camera view target, the position and rotation must be updated in 
            // late update to make sure that there is no lag.
            if (vehicleCamera.SelectedCameraViewTarget != null)
            {
                if (vehicleCamera.SelectedCameraViewTarget.LockPosition)
                {
                    vehicleCamera.transform.position = vehicleCamera.SelectedCameraViewTarget.transform.position;
                }
                if (vehicleCamera.SelectedCameraViewTarget.LockRotation)
                {
                    vehicleCamera.transform.rotation = vehicleCamera.SelectedCameraViewTarget.transform.rotation;
                }

                if (vehicleCamera.SelectedCameraViewTarget.LockCameraForwardVector)
                {
                    // Always point the camera directly forward 
                    vehicleCamera.transform.rotation = Quaternion.LookRotation(vehicleCamera.SelectedCameraViewTarget.transform.forward, vehicleCamera.transform.up);
                }
            }
        }
    }
}
