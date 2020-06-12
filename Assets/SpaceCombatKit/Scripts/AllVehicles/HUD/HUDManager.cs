using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Manages the different components of the HUD for a vehicle.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {

        protected VehicleCamera vehicleCamera;

        protected List<HUDComponent> hudComponents = new List<HUDComponent>();

        protected bool activated = false;

        [Tooltip("Whether to activate the HUD when the scene starts.")]
        [SerializeField]
        protected bool activateOnStart = false;

        [Tooltip("Whether to activate the HUD when the vehicle is entered by any game agent, and deactivate it when exited.")]
        [SerializeField]
        protected bool activateOnPlayerEnterVehicle = true;


        protected virtual void Awake()
        {

            hudComponents = new List<HUDComponent>(transform.GetComponentsInChildren<HUDComponent>());

            Vehicle vehicle = transform.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                vehicle.onEntered.AddListener(OnGameAgentEnteredVehicle);
                vehicle.onExited.AddListener(OnGameAgentExitedVehicle);
            }

            vehicleCamera = GameObject.FindObjectOfType<VehicleCamera>();
            if (vehicleCamera != null)
            {
                vehicleCamera.onCameraViewTargetChanged.AddListener(OnCameraViewChanged);
                for (int i = 0; i < hudComponents.Count; ++i)
                {
                    hudComponents[i].SetCamera(vehicleCamera.MainCamera);
                }
            }
        }
      
        // Called when the scene starts
        protected void Start()
        {
            if (!activated)
            {
                if (activateOnStart)
                {
                    ActivateHUD();
                }
                else
                {
                    DeactivateHUD();
                }
            }  
        }

        /// <summary>
        /// Activate the HUD.
        /// </summary>
        public void ActivateHUD()
        {
            activated = true;

            if (vehicleCamera != null)
            {
                // Check for activation
                OnCameraViewChanged(vehicleCamera.SelectedCameraViewTarget);
               
            }
        }


        /// <summary>
        /// Deactivate the HUD.
        /// </summary>
        public void DeactivateHUD()
        {
            for (int i = 0; i < hudComponents.Count; ++i)
            {
                hudComponents[i].Deactivate();
            }
            activated = false;
        }

     
        /// <summary>
        /// Called when the camera view target changes.
        /// </summary>
        /// <param name="newCameraViewTarget">The new camera view target.</param>
        public void OnCameraViewChanged(CameraViewTarget newCameraViewTarget)
        {
        
            if (!activated) return;

            for (int i = 0; i < hudComponents.Count; ++i)
            {
                int settingsIndex = -1;
                if (newCameraViewTarget != null)
                {
                    for (int j = 0; j < hudComponents[i].CameraViewSettings.Count; ++j)
                    {
                        // Check that the HUDComponent is set to be shown in the current camera view
                        if (hudComponents[i].CameraViewSettings[j].cameraView == newCameraViewTarget.CameraView)
                        {
                            settingsIndex = j;
                            break;
                        }
                    }
                }

                if (settingsIndex == -1)
                {
                    hudComponents[i].Deactivate();
                }
                else
                {
                    switch (hudComponents[i].CameraViewSettings[settingsIndex].anchorTypeForView)
                    {
                        case HUDAnchorType.AnchorTransform:

                            hudComponents[i].transform.SetParent(hudComponents[i].CameraViewSettings[settingsIndex].anchorTransform);
                            hudComponents[i].transform.localPosition = hudComponents[i].CameraViewSettings[settingsIndex].positionOffset;
                            hudComponents[i].transform.localRotation = Quaternion.identity;
                            break;

                        case HUDAnchorType.Camera:

                            hudComponents[i].transform.SetParent(vehicleCamera.transform);
                            hudComponents[i].transform.localPosition = hudComponents[i].CameraViewSettings[settingsIndex].positionOffset;
                            hudComponents[i].transform.localRotation = Quaternion.identity;
                            break;

                        default: // None

                            transform.SetParent(null);
                            break;

                    }

                    if (activated && hudComponents[i].CameraViewSettings[settingsIndex].shownInView)
                    {
                        hudComponents[i].Activate();
                    }
                    else
                    {
                        hudComponents[i].Deactivate();
                    }
                }

                hudComponents[i].OnCameraViewChanged(newCameraViewTarget);
            }
        }
        


        protected virtual void OnGameAgentEnteredVehicle(GameAgent agent)
        {
            if (activateOnPlayerEnterVehicle)
            {
                if (agent.IsPlayer) ActivateHUD();
            }
        }


        protected virtual void OnGameAgentExitedVehicle(GameAgent agent)
        {
            if (activateOnPlayerEnterVehicle)
            {
                if (agent.IsPlayer) DeactivateHUD();
            }
        }


        public void LateUpdate()
        {
            if (activated)
            {
                for (int i = 0; i < hudComponents.Count; ++i)
                {
                    if (hudComponents[i].Activated)
                    {
                        hudComponents[i].OnUpdateHUD();
                    }
                }
            }
        }
    }
}
