using UnityEngine;
using WG.GameX.Player;
using WG.GameX.Ui;

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
        
        [SerializeField] private GameSceneManager gameGameSceneManager;
        [SerializeField] private GameUiController _gameUiController;
        [SerializeField] private PlayerShipController _playerShipController;
        [SerializeField] private PlayerHudController _playerHudController;
        public GameUiController UiController => _gameUiController;
        public PlayerShipController PlayerShipController => _playerShipController;
        public GameSceneManager GameSceneManager => gameGameSceneManager;
        public Camera MainCamera => _playerShipController.MainCamera;
        public PlayerHudController PlayerHudController => _playerHudController;
    }
}