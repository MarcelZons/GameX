using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VSX.UniversalVehicleCombat
{
    public class PlayerVehicleEnterExitInput : VehicleInput
    {

        [Header("Enter/Exit Settings")]

        [SerializeField]
        protected GameAgent gameAgent;

        // The dependencies on the current vehicle
        protected VehicleEnterExitManager vehicleEnterExitManager;
        protected HUDVehicleEnterExit hudController;

        [SerializeField]
        protected CustomInput enterExitInput = new CustomInput("General Vehicle Controls", "Enter/Exit Vehicle", KeyCode.F);

      
        /// <summary>
        /// Stop input.
        /// </summary>
        public override void StopInput()
        {
            base.StopInput();
        }

        /// <summary>
        /// Initialize the vehicle input component.
        /// </summary>
        /// <param name="vehicle">The vehicle to intialize for.</param>
        /// <returns>Whether initialization succeeded.</returns>
        protected override bool Initialize(Vehicle vehicle)
        {          
            // Update the dependencies
            vehicleEnterExitManager = vehicle.GetComponent<VehicleEnterExitManager>();
            if (vehicleEnterExitManager == null)
            {
                return false;
            }

            hudController = vehicle.GetComponentInChildren<HUDVehicleEnterExit>();

            return true;

        }

        // Called every frame
        protected override void InputUpdate()
        {
            if (hudController != null)
            {
                hudController.SetPrompts("PRESS " + enterExitInput.GetInputAsString() + " TO ENTER", 
                                            "PRESS " + enterExitInput.GetInputAsString() + " TO EXIT"); 
            }

            if (vehicleEnterExitManager.CanEnterParent())
            {
                // Check for input
                if (enterExitInput.Down())
                {
                    Vehicle parent = vehicleEnterExitManager.EnterableVehicles[0].Vehicle;
                    vehicleEnterExitManager.EnterParent(0);
                    gameAgent.EnterVehicle(parent);
                }
            }
            else if (vehicleEnterExitManager.CanExitToChild())
            {
                // Check for input
                if (enterExitInput.Down())
                {
                    Vehicle child = vehicleEnterExitManager.Child.Vehicle;
                    vehicleEnterExitManager.ExitToChild();
                    gameAgent.EnterVehicle(child);
                }
            }
        }
    }
}
