using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{

    [System.Serializable]
    public class CameraViewInput
    {
        public VehicleCameraView view;
        public CustomInput input;       

        public CameraViewInput (VehicleCameraView view, CustomInput input)
        {
            this.view = view;
            this.input = input;
        }
    }


    /// <summary>
    /// Vehicle camera control script.
    /// </summary>
    public class PlayerVehicleCameraControls : GeneralInput
    {

        [Header("Settings")] 

        [SerializeField]
        protected VehicleCamera vehicleCamera;

        [Header("Cycle Camera View")]

        [SerializeField]
        protected CustomInput cycleViewForwardInput = new CustomInput("Camera Controls", "Cycle camera view forward.", KeyCode.RightBracket);

        [SerializeField]
        protected CustomInput cycleViewBackwardInput = new CustomInput("Camera Controls", "Cycle camera view backward.", KeyCode.LeftBracket);

        [Header("Select Camera View")]

        [SerializeField]
        protected List<CameraViewInput> cameraViewInputs = new List<CameraViewInput>();

        [Header("Free Look Mode")]

        [SerializeField]
        protected float freeLookSpeed = 1;

        [SerializeField]
        protected CustomInput freeLookModeInput = new CustomInput("Camera Controls", "Free look mode.", KeyCode.LeftShift);

        [SerializeField]
        protected CustomInput lookHorizontalInput = new CustomInput("Camera Controls", "Free look horizontal.", "Mouse X");

        [SerializeField]
        protected CustomInput lookVerticalInput = new CustomInput("Camera Controls", "Free look vertical.", "Mouse Y");

        protected GimbalController vehicleCameraGimbalController;


        protected void Reset()
        {
            cameraViewInputs.Clear();
            cameraViewInputs.Add(new CameraViewInput(VehicleCameraView.Interior, new CustomInput("Camera Controls", "Interior view.", KeyCode.Alpha1)));
            cameraViewInputs.Add(new CameraViewInput(VehicleCameraView.Interior, new CustomInput("Camera Controls", "Chase view.", KeyCode.Alpha2)));
        }

        protected override bool Initialize()
        {
            if (vehicleCamera != null)
            {
                vehicleCameraGimbalController = vehicleCamera.GetComponent<GimbalController>();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void InputUpdate()
        {
            // Cycle camera view
            if (vehicleCamera != null)
            {
                if (cycleViewForwardInput.Down())
                {
                    vehicleCamera.CycleCameraView(true);
                    if (vehicleCameraGimbalController != null) vehicleCameraGimbalController.ResetGimbal();
                }
                else if (cycleViewBackwardInput.Down())
                {
                    vehicleCamera.CycleCameraView(false);
                    if (vehicleCameraGimbalController != null) vehicleCameraGimbalController.ResetGimbal();
                }
            }

            // Select vehicle camera view
            for (int i = 0; i < cameraViewInputs.Count; ++i)
            {
                if (cameraViewInputs[i].input.Down())
                {
                    vehicleCamera.SetView(cameraViewInputs[i].view);
                }
            }

            // Free look mode
            if (vehicleCameraGimbalController != null)
            {
                if (freeLookModeInput.Pressed())
                {
                    vehicleCameraGimbalController.Rotate(lookHorizontalInput.FloatValue() * freeLookSpeed * Time.deltaTime,
                                                            lookVerticalInput.FloatValue() * freeLookSpeed * Time.deltaTime);
                }
            }
        }
    }
}