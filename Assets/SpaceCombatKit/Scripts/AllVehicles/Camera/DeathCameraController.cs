using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Controls the camera when the target vehicle of the camera has been destroyed.
    /// </summary>
    public class DeathCameraController : MonoBehaviour
    {
        [Tooltip("The vehicle camera that this death camera controller animates.")]
        [SerializeField]
        protected VehicleCamera vehicleCamera;

        [Tooltip("The position offset to maintain relative to the point where the vehicle was destroyed.")]
        [SerializeField]
        protected Vector3 positionOffset;

        [Tooltip("The rotation speed of the death camera.")]
        [SerializeField]
        protected float rotationSpeed = 20;

        [Tooltip("The next target position for the death camera.")]
        protected Vector3 targetPosition;

        [Tooltip("Whether the death camera is currently animating.")]
        protected bool animating = false;

        protected Vehicle targetVehicle;


        private void Start()
        {
            // Listen for when the vehicle camera target vehicle changes 
            vehicleCamera.onTargetVehicleSet.AddListener(OnTargetVehicleSet);
        }

        /// <summary>
        /// Called when the vehicle camera's target vehicle changes.
        /// </summary>
        /// <param name="newVehicle">The new target vehicle for the vehicle camera.</param>
        public void OnTargetVehicleSet(Vehicle newVehicle)
        {
            // Disconnect from previous vehicle events
            if (targetVehicle != null)
            {
                targetVehicle.onDestroyed.RemoveListener(OnTargetVehicleDestroyed);
            }

            // Update the target vehicle
            targetVehicle = newVehicle;

            // Listen for destroyed event
            if (targetVehicle != null)
            {
                targetVehicle.onDestroyed.AddListener(OnTargetVehicleDestroyed);
            }
        }


        /// <summary>
        /// Called when the vehicle camera's target vehicle is destroyed.
        /// </summary>
        public void OnTargetVehicleDestroyed()
        {
            vehicleCamera.SetControllersDisabled(true);
            vehicleCamera.transform.position = vehicleCamera.TargetVehicle.transform.position + positionOffset;
            targetPosition = vehicleCamera.TargetVehicle.transform.position;
            animating = true;
        }

        // Called every frame
        protected void Update()
        {
            if (animating)
            {
                vehicleCamera.transform.RotateAround(targetPosition, Vector3.up, rotationSpeed * Time.deltaTime);
                vehicleCamera.transform.LookAt(targetPosition, Vector3.up);
            }
        }
    }
}