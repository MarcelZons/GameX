using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VSX.UniversalVehicleCombat
{
    public class ShipLanderInput : VehicleInput
    {

        [Header("Settings")]
        
        [SerializeField]
        protected CustomInput landingInput = new CustomInput("General Controls", "Land/Take Off", KeyCode.L);

        protected ShipLander shipLander;
        protected HUDShipLander hudShipLander;



        protected override void Start()
        {
            base.Start();
        }

        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;
            
            shipLander = vehicle.GetComponentInChildren<ShipLander>();

            hudShipLander = vehicle.GetComponentInChildren<HUDShipLander>();

            return (shipLander != null);

        }

       
        protected override void InputUpdate()
        {

            if (hudShipLander != null)
            {
                hudShipLander.SetPrompts("PRESS " + landingInput.GetInputAsString() + " TO LAUNCH", "PRESS " + landingInput.GetInputAsString() + " TO LAND");
            }

            switch (shipLander.CurrentState)
            {
                case (ShipLander.ShipLanderState.Launched):

                    if (landingInput.Down())
                    {
                        shipLander.Land();
                    }

                    break;

                case (ShipLander.ShipLanderState.Landed):

                    if (landingInput.Down())
                    {
                        shipLander.Launch();
                    }

                    break;
            }
        }

    }
}