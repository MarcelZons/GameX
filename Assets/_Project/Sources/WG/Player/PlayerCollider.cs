using UnityEngine;
using UnityEngine.Events;

namespace WG.GameX.Player
{
    public class PlayerCollider: MonoBehaviour
    {
        private UnityEvent _playerHitEvent;

        public UnityEvent PlayerHitEvent => _playerHitEvent;
        
        private void Awake()
        {
            _playerHitEvent = new UnityEvent();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                PlayerHitEvent.Invoke();
            }
        }
    }
}