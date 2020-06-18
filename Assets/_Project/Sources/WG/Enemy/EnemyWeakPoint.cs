using UnityEngine;
using WG.GameX.Managers;
using WG.GameX.Player;

namespace WG.GameX.Enemy
{
    [RequireComponent(typeof(BoxCollider))]
    public class EnemyWeakPoint : MonoBehaviour
    {
        public float WeakPointHealth { get; set; }

        private EnemyShipController _enemyShipController;
        private PlayerShipController _playerShipController;
        
        private void Start()
        {
            _enemyShipController = GetComponentInParent<EnemyShipController>();
            _playerShipController = DependencyMediator.Instance.PlayerShipController;
        }

        public void ReduceHealth(float multiplier = 1.0f)
        {
            WeakPointHealth -= Time.deltaTime * multiplier;
            if (WeakPointHealth <= 0)
            {
                DependencyMediator.Instance.ExplosionFx.PlaySmallExplosion(transform.position);
                _enemyShipController.RemoveWeakPoint(this);
                _playerShipController.RemoveFromRadar(this);
                //Debug.LogError($"Weakpoint {gameObject.name} can be destroyed");
                Destroy(this.gameObject);
            }
        }

    }
}