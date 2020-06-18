using UnityEngine;
using UnityEngine.Events;
using WG.GameX.Managers;

namespace WG.GameX.Player
{
    public class PlayerHealthController : MonoBehaviour
    {

        private int _playerMaxHealth;
        
        private int _currentHealth;
        
        public float HealthFactor => (float) (_currentHealth * 1.0 / _playerMaxHealth);

        private PlayerCollider [] _playerColliders;
        private UnityEvent[] _playerHitEvents;
        private void Awake()
        {
            _playerColliders = GetComponentsInChildren<PlayerCollider>();
            _playerHitEvents = new UnityEvent[_playerColliders.Length];
        }

        private void Start()
        {
            _playerMaxHealth = DependencyMediator.Instance.PlayerShipController.Health;
            
            _currentHealth = _playerMaxHealth;
            DependencyMediator.Instance.UiController.SetHealth(HealthFactor);
            
            for (var i = 0; i < _playerColliders.Length; i++)
            {
                var playerCollider = _playerColliders[i];
                _playerHitEvents[i] = playerCollider.PlayerHitEvent;
            }
            
            foreach (var playerHitEvent in _playerHitEvents)
            {
                playerHitEvent.AddListener(() =>
                {
                    _currentHealth--;
                    if (_currentHealth <= 0)
                    {
                        _currentHealth = 0;
                        DependencyMediator.Instance.GameSceneManager.GameOver(false);
                    }
                    DependencyMediator.Instance.UiController.SetHealth(HealthFactor);
                });
            }
        }
    }
}