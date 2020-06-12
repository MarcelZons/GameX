using UnityEngine;
using System.Collections;
using VSX.UniversalVehicleCombat;
using System.Collections.Generic;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// This class provides a floating origin for managing large-scale distances. When the player gets too far from the scene center,
    /// the error in floating point position calculations increases and things start to jitter and shake. With this component, when the player gets a 
    /// specified distance from the origin, the player is returned to the center and the floating origin (which carries all of the scene objects) is 
    /// shifted to maintain the illusion that the player is still at the same position.
    /// </summary>
    public class FloatingOriginManager : MonoBehaviour
    {
        [Tooltip("A reference to the player.")]
        [SerializeField]
        protected GameAgent player = null;

        [Tooltip("The maximum distance the player can reach from the center before the origin is shifted.")]
        [SerializeField]
        protected float maxDistanceFromCenter = 5000;

        [Tooltip("A reference to the vehicle camera.")]
        [SerializeField]
        protected Transform vehicleCamera;              
        
        // A list of all the floating origin children in the scene.
        protected List<FloatingOriginChild> floatingOriginChildren = new List<FloatingOriginChild>();

        // The player's position in the scene relative to the floating origin.
        protected Vector3 playerFloatingPosition;
        public Vector3 PlayerFloatingPosition { get { return playerFloatingPosition; } }    

        // Singleton reference to the floating origin manager
        public static FloatingOriginManager Instance;



        // Called when scene starts
        void Awake()
        {
            // Create the singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Register a new floating origin child.
        /// </summary>
        /// <param name="floatingOriginChild">The new floating origin child.</param>
        public void Register(FloatingOriginChild floatingOriginChild)
        {
            
            // Parent to floating origin manager if not already.
            if (floatingOriginChild.transform.root != transform)
            {
                floatingOriginChild.transform.SetParent(transform);

                // Make sure that the floating origin child starts off with the correct position offset.
                floatingOriginChild.transform.localPosition = transform.position;
            }

            // Add the new floating origin child to the list.
            floatingOriginChildren.Add(floatingOriginChild);

        }


        /// <summary>
        /// Shift the floating origin.
        /// </summary>
        protected void ShiftScene()
        {

            // Call the pre-shift event on scene origin children so they can prepare anything that needs preparing
            for (int i = 0; i < floatingOriginChildren.Count; ++i)
            {
                floatingOriginChildren[i].OnPreOriginShift();
            }

            // Move the scene such that the player can be put back at (0,0,0) without anything changing
            Vector3 playerRelativePosition = player.Vehicle.transform.position - transform.position;
            transform.position = -playerRelativePosition;

            // Store the relative camera position before moving the player
            Vector3 cameraOffset = vehicleCamera.position - player.Vehicle.transform.position;

            // Put the player in the center
            player.Vehicle.transform.position = Vector3.zero;

            // Put the camera back at the correct position
            vehicleCamera.position = player.Vehicle.transform.position + cameraOffset;

            // Call the post-shift event on scene origin children
            for (int i = 0; i < floatingOriginChildren.Count; ++i)
            {
                floatingOriginChildren[i].OnPostOriginShift();
            }
        }


        // Called every frame
        void Update()
        {
            if (!player.IsInVehicle) return;

            // Get the player's actual location in the scene
            Vector3 playerRealPos = player.Vehicle.transform.position;

            // If player is too far from center, shift the scene and place the player at (0,0,0)
            if (playerRealPos.magnitude > maxDistanceFromCenter)
            {
                ShiftScene();
            }

            // Update the player's floating position in the scene
            playerFloatingPosition = playerRealPos - transform.position;

        }
    }
}