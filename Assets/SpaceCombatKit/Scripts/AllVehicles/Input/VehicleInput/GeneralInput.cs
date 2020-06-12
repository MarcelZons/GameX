using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Base class for vehicle input components.
    /// </summary>
    public class GeneralInput : MonoBehaviour
    {

        [Header("General Input")]

        [SerializeField]
        protected bool specifyCompatibleGameStates = false;

        [SerializeField]
        protected List<GameState> compatibleGameStates = new List<GameState>();

        // Whether this input component has everything it needs to run
        protected bool initialized = false;
        public bool Initialized { get { return initialized; } }
 
        // Whether this input component is currently activated
        protected bool inputActive;
        public virtual bool InputActive { get { return inputActive; } }



        protected virtual void Start()
        {
            if (Initialize())
            {
                initialized = true;
                inputActive = true;
            }
        }

        /// <summary>
        /// Start running this input script.
        /// </summary>
        public virtual void StartInput()
        {
            if (initialized) inputActive = true;
        }

        /// <summary>
        /// Stop running this input script.
        /// </summary>
        public virtual void StopInput()
        {
            inputActive = false;
        }


        /// <summary>
        /// Attempt to initialize the input component.
        /// </summary>
        /// <returns> Whether initialization was successful. </returns>
        protected virtual bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Put all your input code in an override of this method.
        /// </summary>
        protected virtual void InputUpdate() { }


        protected virtual void Update()
        {
            if (inputActive)
            {
                if (specifyCompatibleGameStates && GameStateManager.Instance != null)
                {
                    for (int i = 0; i < compatibleGameStates.Count; ++i)
                    {
                        if (compatibleGameStates[i] == GameStateManager.Instance.CurrentGameState)
                        {
                            InputUpdate();
                            break;
                        }
                    }
                }
                else
                {
                    InputUpdate();
                }
            }
        }
    }
}