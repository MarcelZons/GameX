using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity Event for running functions when the vehicle camera's vehicle target is set.
    /// </summary>
    [System.Serializable]
    public class OnVehicleCameraVehicleSetEventHandler : UnityEvent<Vehicle> { }

    /// <summary>
    /// Unity event for running functions when the camera is updated (position, rotation, field of view etc).
    /// </summary>
    [System.Serializable]
    public class OnVehicleCameraUpdatedEventHandler : UnityEvent { }

    /// <summary>
    /// Unity event for running functions when the camera view target changes.
    /// </summary>
    [System.Serializable]
    public class OnVehicleCameraViewTargetChangedEventHandler : UnityEvent <CameraViewTarget> { }

    /// <summary>
    /// Unity event for running functions when the field of view changes.
    /// </summary>
    [System.Serializable]
    public class OnFieldOfViewChangedEventHandler : UnityEvent<float> { }

    /// <summary>
    /// This class represents a vehicle camera, a camera which follows a vehicle and shows different views.
    /// </summary>
    public class VehicleCamera : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("The vehicle to follow when the scene starts.")]
        [SerializeField]
        protected Vehicle startingTargetVehicle;

        // Reference to the current target vehicle
        protected Vehicle targetVehicle;
        public Vehicle TargetVehicle { get { return targetVehicle; } }

        [Tooltip("Reference to the main camera.")]
        [SerializeField]
        protected Camera mainCamera;
        public Camera MainCamera { get { return mainCamera; } }

        [Tooltip("A list of all the auxiliary cameras which must conform to the vehicle camera.")]
        [SerializeField]
        protected List<AuxiliaryCameraController> auxiliaryCameras = new List<AuxiliaryCameraController>();
        public List<AuxiliaryCameraController> AuxiliaryCameras { get { return auxiliaryCameras; } }

        // Store the default field of view (without any effects)
        protected float defaultFieldOfView;
        public float DefaultFieldOfView { get { return defaultFieldOfView; } }
        
        // List of all the camera controllers in the hierarchy
        protected List<VehicleCameraController> cameraControllers = new List<VehicleCameraController>();

        // Flag for whether the camera is passive (controller disabled)
        protected bool controllersDisabled = false;
        public bool ControllersDisabled { get { return controllersDisabled; } }

        protected CameraViewTarget selectedCameraViewTarget;
        public CameraViewTarget SelectedCameraViewTarget { get { return selectedCameraViewTarget; } }

        protected bool hasCameraViewTarget;
        public bool HasCameraViewTarget { get { return hasCameraViewTarget; } }

        public VehicleCameraView CurrentView { get { return hasCameraViewTarget ? selectedCameraViewTarget.CameraView : (VehicleCameraView)0; } }

        [Header("Events")]

        // Target vehicle set event
        public OnVehicleCameraVehicleSetEventHandler onTargetVehicleSet;

        // Camera view changed event
        public OnVehicleCameraViewTargetChangedEventHandler onCameraViewTargetChanged;

        // Called when the field of view changes
        public OnFieldOfViewChangedEventHandler onFieldOfViewChanged;


        
        // Called when the component is first added to a gameobject or the component is reset
        protected virtual void Reset()
        {
            mainCamera = Camera.main;
        }


        protected virtual void Awake()
		{
            // Store the default field of view 
            defaultFieldOfView = mainCamera.fieldOfView;

            // Get all the vehicle camera controllers in the hierarchy
            cameraControllers = new List<VehicleCameraController>(transform.GetComponentsInChildren<VehicleCameraController>());            
            if (cameraControllers.Count == 0)
            {
                Debug.LogWarning("No camera controllers found in vehicle camera hierarchy, vehicle camera will not operate. Please add one or more.");
            }

            foreach(VehicleCameraController cameraController in cameraControllers)
            {
                cameraController.VehicleCamera = this;
            }

            foreach (AuxiliaryCameraController auxiliaryCameraController in auxiliaryCameras)
            {
                onCameraViewTargetChanged.AddListener(auxiliaryCameraController.OnCameraViewTargetChanged);
                onFieldOfViewChanged.AddListener(auxiliaryCameraController.OnFieldOfViewChanged);
            }
        }


        // Called at the start
        protected virtual void Start()
        {
            // Start targeting the starting target vehicle
            if (startingTargetVehicle != null)
            {
                SetVehicle(startingTargetVehicle);
            }
        }


        /// <summary>
        /// Called to set a new target vehicle for the Vehicle Camera.
        /// </summary>
        /// <param name="newVehicle">The new target vehicle.</param>
        public virtual void SetVehicle (Vehicle newVehicle)
		{
           
            // Clear parent
            transform.SetParent(null);
            
            // Deactivate all the camera controllers
            for (int i = 0; i < cameraControllers.Count; ++i)
            {
                cameraControllers[i].StopController();
            }

            if (GetComponent<GimbalController>() != null)
            {
                // Make sure the main camera is centered
                mainCamera.transform.localRotation = Quaternion.identity;
                mainCamera.transform.localPosition = Vector3.zero;
            }

            // Update the vehicle reference
            targetVehicle = newVehicle;

            // Activate the appropriate controller(s)
            if (targetVehicle != null)
            {
                // If no camera view targets on vehicle, issue a warning
                if (targetVehicle.CameraViewTargets.Count == 0)
                {
                    Debug.LogWarning("No Camera View Target components found in vehicle hierarchy, please add one or more.");
                }

                // Activate the appropriate camera controller(s) for the vehicle
                int numControllers = 0;
                for (int i = 0; i < cameraControllers.Count; ++i)
                {
                    cameraControllers[i].SetVehicle(targetVehicle, true);
                    if (cameraControllers[i].ControllerActive)
                    {
                        numControllers++;
                    }
                }
                if (numControllers == 0)
                {
                    Debug.LogWarning("No compatible camera controllers found for target vehicle, please add a compatible camera controller to the vehicle camera hierarchy.");
                }
            }

            // Call the event
            onTargetVehicleSet.Invoke(targetVehicle);

		}

        /// <summary>
        /// Called by an input script to cycle the camera view.
        /// </summary>
        /// <param name="forward">Whether to cycle forward.</param>
        public virtual void CycleCameraView(bool forward)
        {

            // If the target vehicle has no camera view targets, return.
            if (targetVehicle.CameraViewTargets.Count == 0) return;

            // Get the index of the current camera view target
            int index = targetVehicle.CameraViewTargets.IndexOf(selectedCameraViewTarget);
            index += forward ? 1 : -1;

            // Wrap the index between 0 and the number of camera view targets on the vehicle.
            if (index >= targetVehicle.CameraViewTargets.Count)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = targetVehicle.CameraViewTargets.Count - 1;
            }

            // Set the new camera view target
            SetView(targetVehicle.CameraViewTargets[index]);

        }

        /// <summary>
        /// Set a new camera view, specifying a camera view target.
        /// </summary>
        /// <param name="cameraViewTarget">The new camera view target.</param>
        public virtual void SetView(CameraViewTarget cameraViewTarget)
        {

            if (cameraViewTarget == null) return;

            // Update the current view target info
            selectedCameraViewTarget = cameraViewTarget;

            // Update the flag
            hasCameraViewTarget = selectedCameraViewTarget != null;

            if (cameraViewTarget.ParentCamera)
            {
                transform.SetParent(cameraViewTarget.transform);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
                transform.SetParent(null);
            }

            onCameraViewTargetChanged.Invoke(selectedCameraViewTarget);

        }


        /// <summary>
        /// Select a new Vehicle Camera View.
        /// </summary>
        /// <param name="newView">The new camera view.</param>
        public virtual void SetView(VehicleCameraView newView)
		{
            
            // If no target vehicle, set to null and exit.
            if (targetVehicle == null)
            {
                SetView(null);
                return;
            }
            
            // Search all camera views on vehicle for desired view
			for (int i = 0; i < targetVehicle.CameraViewTargets.Count; ++i)
			{
				if (targetVehicle.CameraViewTargets[i].CameraView == newView)
				{
                    SetView(targetVehicle.CameraViewTargets[i]);
                    return;
				}
			}

            // If none found, default to the first available
            if (targetVehicle.CameraViewTargets.Count > 0)
            {
                // Set the first available Camera View Target
                SetView(targetVehicle.CameraViewTargets[0]);

                // Issue a warning
                Debug.LogWarning("No CameraViewTarget found for VehicleCameraView " + newView.ToString() + ". Defaulting to " + 
                    selectedCameraViewTarget.CameraView.ToString());
            }
            else
            {
                SetView(null);

                // Issue a warning
                Debug.LogWarning("No CameraViewTarget found on vehicle, vehicle camera will not work. Please add one or more CameraViewTarget components to the vehicle hierarchy.");
            }	
		}


        /// <summary>
        /// Set the camera view using the integer value of the VehicleCameraView enum value.
        /// </summary>
        /// <param name="vehicleCameraView">The VehicleCameraView as an integer.</param>
        public virtual void SetView(int vehicleCameraView)
        {
            SetView((VehicleCameraView)vehicleCameraView);
        }


        /// <summary>
        /// Set the field of view for the camera
        /// </summary>
        /// <param name="newFieldOfView">The new field of view.</param>
        public void SetFieldOfView(float newFieldOfView)
        {
            mainCamera.fieldOfView = newFieldOfView;

            onFieldOfViewChanged.Invoke(newFieldOfView);
        }


        /// <summary>
        /// Called to set the activation of all the camera controllers (e.g. to control the camera manually).
        /// </summary>
        /// <param name="setDisabled">Whether to enable the controllers.</param>
        public virtual void SetControllersDisabled (bool setDisabled)
        {

            // If setting passive, deactivate all of the camera controllers 
            if (setDisabled)
            {
                for (int i = 0; i < cameraControllers.Count; ++i)
                {
                    cameraControllers[i].StopController();
                }
            }
            // If setting to not passive, and there is a target vehicle, activate all of the controller components that match the target vehicle.
            else
            {
                if (targetVehicle != null)
                {
                    for (int i = 0; i < cameraControllers.Count; ++i)
                    {
                        if (cameraControllers[i].Initialized)
                        {
                            cameraControllers[i].StartController();
                        }
                    }
                } 
            }
        }
    }
}
