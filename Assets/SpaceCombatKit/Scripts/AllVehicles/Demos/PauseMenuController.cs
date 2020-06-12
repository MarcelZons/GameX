using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSX.UniversalVehicleCombat
{
    public class PauseMenuController : SimpleMenuManager
    {

        [Header("Pause Menu")]

        [Tooltip("The index of the scene to go to when the player exits the game.")]
        [SerializeField]
        protected int quitSceneIndex;

        [Tooltip("The game state to go to when the player resumes the game.")]
        [SerializeField]
        protected GameState gameplayGameState;
      
        
        /// <summary>
        /// Called when the player presses the 'Continue' button.
        /// </summary>
        public void Continue()
        {
            GameStateManager.Instance.EnterGameState(gameplayGameState);
        }

        /// <summary>
        /// Called when the player presses the 'Restart' button.
        /// </summary>
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Called when the player presses the 'Controls' button.
        /// </summary>
        public void Controls()
        {
            GameStateManager.Instance.EnterGameState(GameState.ControlsMenu);
        }

        /// <summary>
        /// Called when the player presses the 'Exit' button.
        /// </summary>
        public void Quit()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(quitSceneIndex);
        }
    }
}