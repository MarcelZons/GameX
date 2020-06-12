using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// This class provides a simple way to enable and disable a set of UI objects, and set the first UI element selection,
    /// when the game enters a specified game state.
    /// </summary>
    public class SimpleMenuManager : MonoBehaviour
    {

        [Header("Menu Manager")]

        [SerializeField]
        protected GameState activationGameState;

        [Tooltip("The game state to exit to, for example when the back button is pressed.")]
        [SerializeField]
        protected GameState exitGameState;

        [SerializeField]
        protected bool deactivateMenuOnAwake = true;

        [SerializeField]
        protected List<GameObject> UIObjects = new List<GameObject>();

        [SerializeField]
        protected GameObject firstSelected;
        protected bool waitingForHighlight = false;

        [SerializeField]
        protected float pauseBeforeActivation;

        protected bool menuActivated = false;

        

        protected virtual void Awake()
        {
            if (GameStateManager.Instance != null) GameStateManager.Instance.onEnteredGameState.AddListener(OnEnteredGameState);
            if (deactivateMenuOnAwake) DeactivateMenu();
        }

        protected virtual void Start()
        {
            if (GameAgentManager.Instance != null) GameAgentManager.Instance.onFocusedVehicleChanged.AddListener(SetVehicle);
        }


        public virtual void SetVehicle(Vehicle vehicle) { }
        

        // Event called when the game enters a new game state
        protected virtual void OnEnteredGameState(GameState newGameState)
        {  
            // If the game enters the game state this manager refers to, activate all UI
            if (newGameState == activationGameState)
            {
                StartCoroutine(WaitForActivation(pauseBeforeActivation));
            }
            // If the game state is not the one this manager refers to, disable all UI
            else
            {
                DeactivateMenu();

                waitingForHighlight = false;
            }
        }

        protected virtual IEnumerator WaitForActivation(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (GameStateManager.Instance.CurrentGameState == activationGameState)
            {
                ActivateMenu();
            }
        }

        public virtual bool CanOpenMenu()
        {
            return true;
        }

        public virtual void OpenMenu()
        {
            if (CanOpenMenu())
            {
                GameStateManager.Instance.EnterGameState(activationGameState);
            }
        }

        public virtual void ActivateMenu()
        {
            for (int i = 0; i < UIObjects.Count; ++i)
            {
                UIObjects[i].SetActive(true);
            }

            if (firstSelected != null)
            {
                // When the menu activates, flag the first item to be selected, and clear the currently selected item.
                // The new selected gameobject must be selected in OnGUI.
                EventSystem.current.SetSelectedGameObject(null);
                waitingForHighlight = true;
            }
            
            menuActivated = true;
        }


        public virtual void DeactivateMenu()
        {
            for (int i = 0; i < UIObjects.Count; ++i)
            {
                UIObjects[i].SetActive(false);
            }

            menuActivated = false;
        }

        public virtual void ExitMenu()
        {
            GameStateManager.Instance.EnterGameState(exitGameState);
        }

        // Called when the UI is updated
        protected virtual void OnGUI()
        {
            // If the flag is still up, highlight the first button
            if (waitingForHighlight)
            {
                // Highlight the first button
                EventSystem.current.SetSelectedGameObject(firstSelected);
                
                // Reset the flag
                waitingForHighlight = false;
            }
        }
    }
}