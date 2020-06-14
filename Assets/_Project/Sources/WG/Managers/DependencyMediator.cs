using UnityEngine;
using UnityEngine.Serialization;
using WG.GameX.Player;

namespace WG.GameX.Managers
{
    public class DependencyMediator : MonoBehaviour
    {
        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private GameUiController _gameUiController;
        [FormerlySerializedAs("_playerShipController")] [SerializeField] private PlayerShipController playerPlayerShipController;

        public GameUiController UiController => _gameUiController;

        public PlayerShipController PlayerShipController => playerPlayerShipController;

        public GameSceneManager SceneManager => _gameSceneManager;
    }
}