using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Manages a demo game over menu.
    /// </summary>
    public class GameOverMenuController : SimpleMenuManager
    {

        [Header("Game Over Menu")]

        [Tooltip("The index of the scene to go to when the player exits the game.")]
        [SerializeField]
        protected int quitSceneIndex;

        /// <summary>
        /// Restart the scene.
        /// </summary>
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Quit to another scene.
        /// </summary>
        public void Quit()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(quitSceneIndex);
        }
    }
}