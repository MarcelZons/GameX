using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity event for running functions when a module is mounted at a module mount.
    /// </summary>
    [System.Serializable]
    public class OnModuleMountedEventHandler : UnityEvent<ModuleMount> { };

    /// <summary>
    /// Unity event for running functions when a module is unmounted from a module mount.
    /// </summary>
    [System.Serializable]
    public class OnModuleUnmountedEventHandler : UnityEvent { };

    /// <summary>
    /// Unity event for running functions when a module's activation state changes.
    /// </summary>
    [System.Serializable]
    public class OnModuleActivationStateChangedEventHandler : UnityEvent<ModuleActivationState> { };

    /// <summary>
    /// Unity event for running functions when a module is reset.
    /// </summary>
    [System.Serializable]
    public class OnModuleResetEventHandler : UnityEvent { };

    /// <summary>
    /// Unity event for running functions when the owner's root gameobject is set.
    /// </summary>
    [System.Serializable]
    public class OnSetRootTransformEventHandler : UnityEvent <Transform> { };


    /// <summary>
    /// This class represents a module that can be loaded or unloaded on a vehicle's module mount.
    /// </summary>
    public class Module : MonoBehaviour
	{

        [Header("General")]

        [SerializeField]
        protected string label;
		public string Label { get { return label; } }

        [SerializeField]
        protected string description;
        public string Description { get { return description; } }

        [SerializeField]
        protected Sprite menuSprite;
        public Sprite MenuSprite { get { return menuSprite; } }

        [SerializeField]
        protected ModuleType moduleType;
		public ModuleType ModuleType { get { return moduleType; } }

        protected GameObject cachedGameObject;
		public GameObject CachedGameObject { get { return cachedGameObject; } }

        protected ModuleActivationState moduleActivationState;
		public ModuleActivationState ModuleActivationState { get { return moduleActivationState; } }

        [Header("Events")]

        // Module mounted event
        public OnModuleMountedEventHandler onModuleMounted;

        // Module unmounted event
        public OnModuleUnmountedEventHandler onModuleUnmounted;

        // Module activation state changed event
        public OnModuleActivationStateChangedEventHandler onModuleActivationStateChanged;

        // Module reset event
        public OnModuleResetEventHandler onModuleReset;

        // Module owner root gameobject set event
        public OnSetRootTransformEventHandler onSetRootTransform;

        protected List<IRootTransformUser> rootTransformUsers = new List<IRootTransformUser>();
        

        protected virtual void Awake()
        {
            rootTransformUsers = new List<IRootTransformUser>(GetComponentsInChildren<IRootTransformUser>());
        }

        /// <summary>
        /// Called when this module is mounted at a module mount.
        /// </summary>
        /// <param name="moduleMount">The module mount this module is to be mounted at.</param>
		public virtual void Mount(ModuleMount moduleMount)
        {
            onModuleMounted.Invoke(moduleMount);
        }

        /// <summary>
        /// Called when this module is unmounted from a module mount.
        /// </summary>
		public virtual void Unmount()
        {
            onModuleUnmounted.Invoke();
        }

        /// <summary>
        /// Called to change the activation state of this module.
        /// </summary>
        /// <param name="newModuleActivationState">The new activation state for the module.</param>
		public virtual void SetModuleActivationState(ModuleActivationState newModuleActivationState)
        {
            moduleActivationState = newModuleActivationState;
            onModuleActivationStateChanged.Invoke(newModuleActivationState);
        }

        /// <summary>
        /// Reset the module.
        /// </summary>
		public virtual void ResetModule()
        {
            onModuleReset.Invoke();
        }

        /// <summary>
        /// Pass the module owner's root gameobject to relevant components via event.
        /// </summary>
        /// <param name="ownerRootGameObject">The owner's root gameobject.</param>
        public virtual void SetRootTransform(Transform rootTransform)
        {

            for (int i = 0; i < rootTransformUsers.Count; ++i)
            {
                rootTransformUsers[i].SetRootTransform(rootTransform);
            }

            onSetRootTransform.Invoke(rootTransform);
        }
    }
}
