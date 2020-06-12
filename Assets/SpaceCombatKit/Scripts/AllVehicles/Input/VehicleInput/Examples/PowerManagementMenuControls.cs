using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class PowerManagementMenuControls : GeneralInput
    {

        [Header("Settings")]

        [SerializeField]
        protected PowerManagementMenuController powerManagementMenuController;

        [SerializeField]
        protected float powerBallMoveSpeed = 1;

        [SerializeField]
        protected CustomInput powerBallMoveHorizontalInput;

        [SerializeField]
        protected CustomInput powerBallMoveVerticalInput;


        protected override bool Initialize()
        {
            return (powerManagementMenuController != null);
        }

        protected override void InputUpdate()
        {
          
            // Move power ball horizontally
            powerManagementMenuController.MovePowerBallHorizontally(powerBallMoveHorizontalInput.FloatValue() * powerBallMoveSpeed * Time.unscaledDeltaTime);

            // Move power ball vertically
            powerManagementMenuController.MovePowerBallVertically(powerBallMoveVerticalInput.FloatValue() * powerBallMoveSpeed * Time.unscaledDeltaTime);

        }
    }
}

