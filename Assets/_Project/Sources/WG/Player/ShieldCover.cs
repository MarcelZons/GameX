using UnityEngine;
using UnityEngine.Events;
using WG.GameX.Common;

namespace WG.GameX.Player
{
    public class ShieldCoverHitEvent : UnityEvent<Bullet>
    {
        
    }
    public class ShieldCover: MonoBehaviour
    {
        private ShieldCoverHitEvent _shieldCoverTakingHit;

        public ShieldCoverHitEvent ShieldCoverTakingHit => _shieldCoverTakingHit;

        private void Awake()
        {
            _shieldCoverTakingHit = new ShieldCoverHitEvent();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                _shieldCoverTakingHit.Invoke(other.gameObject.GetComponent<Bullet>());
                
            }
        }
    }
}