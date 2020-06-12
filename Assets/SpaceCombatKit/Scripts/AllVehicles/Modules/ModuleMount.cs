using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VSX.UniversalVehicleCombat;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Used to store a module that has been created and attached, but not necessarily mounted yet on the module mount.
    /// </summary>
    public class MountableModule
	{

		public Module modulePrefab;

		public Module createdModule;
	
		public MountableModule(Module modulePrefab, Module createdModule)
		{
			this.modulePrefab = modulePrefab;
			this.createdModule = createdModule;
		}
	}

    /// <summary>
    /// Unity event for running functions when a module is mounted at a module mount.
    /// </summary>
    [System.Serializable]
    public class OnModuleMountModuleMountedEventHandler : UnityEvent<Module> { }

    /// <summary>
    /// Unity event for running functions when a module is unmounted from a module mount 
    /// </summary>
    [System.Serializable]
    public class OnModuleMountModuleUnmountedEventHandler : UnityEvent<Module> { }

    /// <summary>
    /// Provides a physical location as well as a reference point for a module that adds functionality to a vehicle.
    /// </summary>
    public class ModuleMount : MonoBehaviour 
	{

        [Header("General")]
	
        // The label shown in the loadout menu for this module mount
		[SerializeField]
		protected string label = "Module Mount";
		public string Label { get { return label; } }

		// All the module types that can be mounted on this module mount
		[SerializeField]
		protected List<ModuleType> mountableTypes = new List<ModuleType>();
		public List<ModuleType> MountableTypes { get { return mountableTypes; } }

        /// <summary>
        /// A list of all the attachment points for mounting multi-unit modules
        /// </summary>
		[SerializeField]
        protected List<Transform> attachmentPoints = new List<Transform>();
        public List<Transform> AttachmentPoints { get { return attachmentPoints; } }

        // Whether to look for and mount the first available module at the start
        public bool mountFirstAvailableModuleAtStart = true;

        // Reference to the root object of the owner of this module (e.g. a vehicle)
        [SerializeField]
        private Transform rootTransform;

        [SerializeField]
        protected int sortingIndex = 0;
        public int SortingIndex { get { return sortingIndex; } }

        [Header("Child Modules")]

        // Whether to load modules that already exist in the hierarchy
        public bool loadExistingChildModules = true;
        
        [Header("Default Modules")]

        // All the module prefabs that will be instantiated by default for this mount
        [SerializeField]
		protected List<Module> defaultModulePrefabs = new List<Module>();	
		public List<Module> DefaultModulePrefabs { get { return defaultModulePrefabs; } }

        // Whether or not to create (instantiate) default modules at the start
        public bool createDefaultModulesAtStart = true;
		
		// List of all the modules that have been created at this mount and which can be mounted here
		[SerializeField]
		protected List<MountableModule> mountableModules = new List<MountableModule>();	
		public List<MountableModule> MountableModules { get { return mountableModules; } }

		// The index of the currently selected module
		protected int mountedModuleIndex = -1;
		public int MountedModuleIndex { get { return mountedModuleIndex; } }
	
        [Header("Events")]

        // Module mounted event
        public OnModuleMountModuleMountedEventHandler onModuleMounted;

        // Module unmounted event
        public OnModuleMountModuleUnmountedEventHandler onModuleUnmounted;

        
        // Called when the component is first added to a gameobject or reset in the inspector
        protected virtual void Reset()
        {
            // Default to allowing all module types to be mounted here 
            mountableTypes = new List<ModuleType>((ModuleType[])System.Enum.GetValues(typeof(ModuleType)));

            rootTransform = transform.root;
        }

        // Called when scene starts
        protected virtual void Start()
		{
            // Load all of the modules already existing as children of this module mount
            if (loadExistingChildModules)
            {
                Module[] modules = transform.GetComponentsInChildren<Module>();
                foreach (Module module in modules)
                {

                    // If this type of module is not allowed here, ignore it
                    if (!mountableTypes.Contains(module.ModuleType))
                    {
                        Debug.LogWarning("Skipping loading of child module as it is an incompatible type for this module mount.");
                        continue;
                    }

                    // Check if this module is already loaded
                    bool found = false;
                    for (int i = 0; i < mountableModules.Count; ++i)
                    {
                        if (module.CachedGameObject == mountableModules[i].createdModule.CachedGameObject) found = true;
                    }
                    if (found) continue;

                    // Center and orient the module at the module mount
                    module.transform.localPosition = Vector3.zero;
                    module.transform.localRotation = Quaternion.identity;

                    // Add the module as a mountable module
                    AddMountableModule(module, null, (mountFirstAvailableModuleAtStart && mountedModuleIndex == -1));
                    
                }
            }
            
			// Create all of the modules in the default list
			if (createDefaultModulesAtStart)
			{
				for (int i = 0; i < defaultModulePrefabs.Count; ++i)
				{
                    // If this listing is null, ignore it
                    if (defaultModulePrefabs[i] == null)
						continue;

                    // If this type of module is not allowed at the module mount, ignore it
					if (!mountableTypes.Contains(defaultModulePrefabs[i].ModuleType))
                    {
                        Debug.LogWarning("Skipping instantiation and loading of default module prefab as it is an incompatible type for this module mount.");
                        continue;
                    }
						
                    Module createdModule = Instantiate(defaultModulePrefabs[i], transform);
                    
					createdModule.transform.localPosition = Vector3.zero;
					createdModule.transform.localRotation = Quaternion.identity;
                    
					AddMountableModule(createdModule, defaultModulePrefabs[i], (mountFirstAvailableModuleAtStart && mountedModuleIndex == -1));
					
				}
			}
		}



        /// <summary>
        /// Add a new mountable module to this module mount.
        /// </summary>
        /// <param name="createdModule">The module to be added (must be already created in the scene).</param>
        /// <param name="prefabReference">The prefab the module was made from.</param>
        /// <param name="mountImmediately">Whether the module should be mounted immediately. </param>
        /// <returns>The MountableModule class instance for the newly stored module.</returns>
        public MountableModule AddMountableModule(Module createdModule, Module prefabReference = null, bool mountImmediately = false)
		{

            if (!mountableTypes.Contains(createdModule.ModuleType))
				return null;

            // Update the module
            createdModule.SetRootTransform(rootTransform);
            createdModule.transform.SetParent(transform);
			createdModule.transform.localPosition = Vector3.zero;
			createdModule.transform.localRotation = Quaternion.identity;
            createdModule.gameObject.SetActive(false);

            // Add to mountable modules list
			MountableModule newMountableModule = new MountableModule(prefabReference, createdModule);
			mountableModules.Add(newMountableModule);			
			
            // Mount
			if (mountImmediately)
			{ 
				MountModule(mountableModules.Count - 1);
			}
			else
			{
				newMountableModule.createdModule.Unmount();
			}

			return newMountableModule;
		}
		

		/// <summary>
        /// Cycle the mounted module at this module mount.
        /// </summary>
        /// <param name="forward">Whether to cycle forward or backward.</param>
		public void Cycle(bool forward)
		{

			if (mountableModules.Count <= 1) return;

			// Increment or decrement the module index
			int newMountedModuleIndex = forward ? mountedModuleIndex + 1 : mountedModuleIndex - 1;
		
			// If exceeds highest index, return to zero index
			newMountedModuleIndex = newMountedModuleIndex >= mountableModules.Count ? 0 : newMountedModuleIndex;

			// If exceeds lowest index, return to last index
			newMountedModuleIndex = newMountedModuleIndex < 0 ? mountableModules.Count - 1 : newMountedModuleIndex;

			// Mount the new Module
			MountModule(newMountedModuleIndex);

		}
		
		
		/// <summary>
        /// Mount a new module at the module mount. Module must be already added as a MountableModule instance.
        /// </summary>
        /// <param name="newMountedModuleIndex">The new module's index within the list of Mountable Modules.</param>
        /// <returns>Whether the mounting of the module occurred.</returns>
		public void MountModule(int newMountedModuleIndex)
		{
            // Deactivate the last one
			if (mountedModuleIndex >= 0)
			{ 
				mountableModules[mountedModuleIndex].createdModule.Unmount();
                mountableModules[mountedModuleIndex].createdModule.gameObject.SetActive(false);
                onModuleUnmounted.Invoke(mountableModules[mountedModuleIndex].createdModule);
                mountedModuleIndex = -1; 
			}

			// If new index is valid, activate the module
			if (newMountedModuleIndex >= 0 && newMountedModuleIndex < mountableModules.Count)
			{
                mountableModules[newMountedModuleIndex].createdModule.gameObject.SetActive(true);
                mountableModules[newMountedModuleIndex].createdModule.Mount(this);
				mountedModuleIndex = newMountedModuleIndex;
                onModuleMounted.Invoke(mountableModules[newMountedModuleIndex].createdModule);
            }
		}

	
		/// <summary>
        /// Clear all of the modules stored at this module mount
        /// </summary>
		public void ClearMountableModules()
		{
			for (int i = 0; i < mountableModules.Count; ++i){
				mountableModules[i].createdModule.Unmount();
			}
			mountableModules.Clear();
			mountedModuleIndex = -1;
		}


		/// <summary>
        /// Get a reference to the Module component of the module currently mounted at this module mount.
        /// </summary>
        /// <returns>The module's Module component</returns>
		public Module Module()
		{
			if (mountedModuleIndex == -1)
			{
				return null;
			}
			else
			{
				return mountableModules[mountedModuleIndex].createdModule;
			}
		}
	}
}
