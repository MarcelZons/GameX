using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity event for running functions when the vehicle is destroyed.
    /// </summary>
    [System.Serializable]
    public class OnVehicleDestroyedEventHandler : UnityEvent { };

    /// <summary>
    /// Unity event for running functions when the vehicle is restored from being destroyed.
    /// </summary>
    [System.Serializable]
    public class OnVehicleRestoredEventHandler : UnityEvent { };

    /// <summary>
    /// Unity event for running functions when the vehicle is entered by a game agent.
    /// </summary>
    [System.Serializable]
    public class OnVehicleEnteredEventHandler : UnityEvent <GameAgent> { };

    /// <summary>
    /// Unity event for running functions when the vehicle is exited by a game agent.
    /// </summary>
    [System.Serializable]
    public class OnVehicleExitedEventHandler : UnityEvent <GameAgent> { };


    /// <summary> 
    /// This class is a base class for all kinds of vehicles. It exposes a function for entering and exiting 
    /// the vehicle, and deals with all kinds of vehicle events.
    /// </summary>
    public class Vehicle : MonoBehaviour
	{

		[Header("General")]

        // The identifying label for this vehicle, used by the loadout menu etc. 
        [SerializeField]
		protected string label;
		public virtual string Label { get { return label; } }

        // The class of vehicle
        [SerializeField]
        protected VehicleClass vehicleClass;
        public VehicleClass VehicleClass { get { return vehicleClass; } }

        // A list of all the camera views on the vehicle
        protected List<CameraViewTarget> cameraViewTargets = new List<CameraViewTarget>();
        public List<CameraViewTarget> CameraViewTargets
        {
            get
            {
                // If prefab, search the hierarchy
                if (gameObject.scene.rootCount == 0)
                {
                    return GetCameraViewTargetsOnVehicle();
                }
                // If not prefab, use cached list
                else
                {
                    return cameraViewTargets;
                }
            }
        }

        // A list of all the occupants currently in the vehicle
        protected List<GameAgent> occupants = new List<GameAgent>();
        public List<GameAgent> Occupants { get { return occupants; } }

        // Efficiently get the game object
        protected GameObject cachedGameObject;
        public GameObject CachedGameObject { get { return cachedGameObject; } }

        // Efficiently get the rigidbody
        protected Rigidbody cachedRigidbody;
        public Rigidbody CachedRigidbody { get { return cachedRigidbody; } }

        protected List<ModuleMount> moduleMounts = new List<ModuleMount>();
        public List<ModuleMount> ModuleMounts
        {
            get
            {
                // If prefab, search the hierarchy
                if (gameObject.scene.rootCount == 0)
                {
                    return GetModuleMountsOnVehicle();
                }
                // If not prefab, use cached list
                else
                {
                    return moduleMounts;
                }
            }
        }

        protected List<ModuleManager> moduleManagers = new List<ModuleManager>();

        protected bool destroyed;
        public bool Destroyed { get { return destroyed; } }

        [Header("Events")]
        
        // Vehicle destroyed event
        public OnVehicleDestroyedEventHandler onDestroyed;

        // Vehicle restored event
        public OnVehicleRestoredEventHandler onRestored;

        // Game agent entered event
        public OnVehicleEnteredEventHandler onEntered;

        // Game Agent exited event
        public OnVehicleEnteredEventHandler onExited;



        // Called when the component is first added to a gameobject or is reset in the inspector
        protected virtual void Reset()
        {
            label = "Vehicle";
        }


        protected virtual void Awake()
		{		
            cachedGameObject = gameObject;
            cachedRigidbody = GetComponent<Rigidbody>();

            cameraViewTargets = GetCameraViewTargetsOnVehicle();

            moduleManagers = new List<ModuleManager>(GetComponentsInChildren<ModuleManager>());

            moduleMounts = GetModuleMountsOnVehicle();

        }


        protected List<ModuleMount> GetModuleMountsOnVehicle()
        {
            List<ModuleMount> moduleMountsList = new List<ModuleMount>();
            ModuleMount[] moduleMountsArray = transform.GetComponentsInChildren<ModuleMount>();
            foreach (ModuleMount moduleMount in moduleMountsArray)
            {
                // Find the right index to insert the module mount according to its sorting index
                int insertIndex = 0;
                for (int i = 0; i < moduleMountsList.Count; ++i)
                {
                    if (moduleMountsList[i].SortingIndex < moduleMount.SortingIndex)
                    {
                        insertIndex = i + 1;
                    }
                }

                // Insert the camera view target into the list
                moduleMountsList.Insert(insertIndex, moduleMount);
            }

            return moduleMountsList;
        }


        /// <summary>
        /// Add a module mount to the vehicle.
        /// </summary>
        /// <param name="moduleMount">The module mount to be added.</param>
        public virtual void AddModuleMount(ModuleMount moduleMount)
        {
            // Find the right index to insert the module mount according to its sorting index
            int insertIndex = 0;
            for (int i = 0; i < moduleMounts.Count; ++i)
            {
                if (moduleMounts[i].SortingIndex < moduleMount.SortingIndex)
                {
                    insertIndex = i + 1;
                }
            }

            // Insert the camera view target into the list
            moduleMounts.Insert(insertIndex, moduleMount);

            for (int i = 0; i < moduleManagers.Count; ++i)
            {
                moduleManagers[i].OnModuleMountAdded(moduleMount);
            }
        }


        /// <summary>
        /// Remove a module mount from the vehicle.
        /// </summary>
        /// <param name="moduleMount">The module mount to be removed.</param>
        public virtual void RemoveModuleMount(ModuleMount moduleMount)
        {

            int index = moduleMounts.IndexOf(moduleMount);
            if (index == -1) return;

            moduleMounts.Remove(moduleMount);

            for (int i = 0; i < moduleManagers.Count; ++i)
            {
                moduleManagers[i].OnModuleMountRemoved(moduleMount);
            }
        }


        protected List<CameraViewTarget> GetCameraViewTargetsOnVehicle()
        {
            List<CameraViewTarget> cameraViewTargetsList = new List<CameraViewTarget>();
            CameraViewTarget[] cameraViewTargetsArray = transform.GetComponentsInChildren<CameraViewTarget>();
            foreach (CameraViewTarget cameraViewTarget in cameraViewTargetsArray)
            {
                // Find the right index to insert the module mount according to its sorting index
                int insertIndex = 0;
                for (int i = 0; i < cameraViewTargetsList.Count; ++i)
                {
                    if (cameraViewTargetsList[i].SortingIndex < cameraViewTarget.SortingIndex)
                    {
                        insertIndex = i + 1;
                    }
                }

                // Insert the camera view target into the list
                cameraViewTargetsList.Insert(insertIndex, cameraViewTarget);
            }

            return cameraViewTargetsList;
        }


        /// <summary>
        /// Called when a game agent enters the vehicle.
        /// </summary>
        /// <param name="newOccupant">The game agent that entered the vehicle.</param>
        public virtual void OnEntered (GameAgent newOccupant)
        {
            // Check if the game agent is already in the vehicle
            for (int i = 0; i < occupants.Count; ++i)
            {
                if (occupants[i] == newOccupant)
                {
                    return;
                }
            }

            // Add the new occupant
            occupants.Add(newOccupant);

            // Call the event
            onEntered.Invoke(newOccupant);

        }


        /// <summary>
        /// Called when a game agent exits a vehicle.
        /// </summary>
        /// <param name="exitingOccupant">The game agent exiting.</param>
        public virtual void OnExited (GameAgent exitingOccupant)
        {
            // Find the occupant in the list and remove
            for (int i = 0; i < occupants.Count; ++i)
            {
                if (occupants[i] == exitingOccupant)
                {
                    // Remove the occupant
                    occupants.RemoveAt(i);

                    // Call the event
                    onExited.Invoke(exitingOccupant);
                    
                    break;
                }
            }
        }

        /// <summary>
        /// Called to destroy the vehicle (e.g. when health reaches zero).
        /// </summary>
        public virtual void Destroy()
        {
            if (!destroyed)
            {
                destroyed = true;

                // Call event
                onDestroyed.Invoke();
            }          
        }

        /// <summary>
        /// Restore the vehicle after it has been destroyed.
        /// </summary>
        public virtual void Restore()
		{
            if (destroyed)
            {
                destroyed = false;

                // Call event
                onRestored.Invoke();
            }         
		}
    }
}
