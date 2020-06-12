using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{

    [System.Serializable]
    public class OnLoadoutVehicleSpawnedEventHandler : UnityEvent<Vehicle> { }

    public class LoadoutVehicleSpawner : MonoBehaviour
    {

        [SerializeField]
        protected PlayerItemManager itemManager;

        [SerializeField]
        protected Transform spawnPoint;

        [SerializeField]
        protected bool spawnAtStart = true;

        public OnLoadoutVehicleSpawnedEventHandler onLoadoutVehicleSpawned;


        protected void Start()
        {
            if (spawnAtStart)
            {
                Spawn();
            }
        }


        public void Spawn()
        {
            int playerVehicleIndex = PlayerData.GetSelectedVehicleIndex(itemManager);
            if (playerVehicleIndex == -1)
            {
                if (itemManager != null && itemManager.vehicles.Count > 0)
                {
                    playerVehicleIndex = 0;
                }
            }
            
            // Get the player ship
            Vehicle playerVehicle = null;

            if (playerVehicleIndex != -1)
            {

                Transform vehicleTransform = ((GameObject)Instantiate(itemManager.vehicles[playerVehicleIndex].gameObject, spawnPoint.position, spawnPoint.rotation)).transform;
                playerVehicle = vehicleTransform.GetComponent<Vehicle>();
                playerVehicle.name = "PlayerVehicle";

                List<int> selectedModuleIndexesByMount = PlayerData.GetModuleLoadout(playerVehicleIndex, itemManager);

                bool hasLoadout = false;
                for (int i = 0; i < selectedModuleIndexesByMount.Count; ++i)
                {
                    if (selectedModuleIndexesByMount[i] != -1)
                    {
                        hasLoadout = true;
                    }
                }

                // Update the vehicle loadout
                if (hasLoadout)
                {
                    for (int i = 0; i < selectedModuleIndexesByMount.Count; ++i)
                    {

                        if (selectedModuleIndexesByMount[i] == -1) continue;

                        Module module = GameObject.Instantiate(itemManager.modulePrefabs[selectedModuleIndexesByMount[i]], null);

                        playerVehicle.ModuleMounts[i].AddMountableModule(module, itemManager.modulePrefabs[selectedModuleIndexesByMount[i]], true);

                    }
                }
                else
                {
                    for (int i = 0; i < playerVehicle.ModuleMounts.Count; ++i)
                    {
                        playerVehicle.ModuleMounts[i].createDefaultModulesAtStart = true;
                    }
                }
                
                onLoadoutVehicleSpawned.Invoke(playerVehicle);

            }   
        }
    }
}