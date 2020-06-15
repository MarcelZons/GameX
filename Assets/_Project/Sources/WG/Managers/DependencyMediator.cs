using UnityEngine;
using UnityEngine.Serialization;
using WG.GameX.Player;

namespace WG.GameX.Managers
{
    public class DependencyMediator : MonoBehaviour
    {
        private static DependencyMediator _instance;

        public static DependencyMediator Instance { get { return _instance; } }


        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            } else {
                _instance = this;
            }
        }
        
        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private GameUiController _gameUiController;
        [SerializeField] private PlayerShipController _playerShipController;

        public GameUiController UiController => _gameUiController;

        public PlayerShipController PlayerShipController => _playerShipController;

        public GameSceneManager SceneManager => _gameSceneManager;
        public Camera MainCamera => _playerShipController.MainCamera;
    }
}