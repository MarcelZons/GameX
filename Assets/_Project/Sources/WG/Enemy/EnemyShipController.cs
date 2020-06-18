using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using WG.GameX.Managers;

namespace WG.GameX.Enemy
{
    public class EnemyShipController : MonoBehaviour
    {
        [Header("##### Tuning Parameter - Turret")]
        [SerializeField] private float _turretActivationDistance = 300;
        [SerializeField] private float _turretFrequency;
        [Range(10,1000)]
        [SerializeField] private float _bulletVelocity;

        [Range(1,100)]
        [SerializeField] private float _turretRobustness;
        
        [Header("###############################")]
        [SerializeField] private LayerMask _weakpointLayer;
        

        public LayerMask WeakpointLayerMask => _weakpointLayer;
        private List<EnemyWeakPoint> _enemyWeakPoints;
        private List<EnemyTurret> _enemyTurrets;

        private void Awake()
        {
            _enemyWeakPoints = GetComponentsInChildren<EnemyWeakPoint>().ToList();
            _enemyTurrets = GetComponentsInChildren<EnemyTurret>().ToList();
        }

        private void Start()
        {
            foreach (var enemyTurret in _enemyTurrets)
            {
                enemyTurret.Setup(DependencyMediator.Instance.PlayerShipController.transform, _turretActivationDistance, _turretFrequency, _bulletVelocity, _turretRobustness);
            }
        }

        public List<Transform> GetEnemyWeakpoints()
        {
            if (_enemyWeakPoints.Count == 0)
            {
                DependencyMediator.Instance.ExplosionFx.PlayMediumExplosion(transform.position);
                DestroyShip();
                return null;
            }
            
            return _enemyWeakPoints.Select(point => point.transform).ToList();
        }

        private void DestroyShip()
        {
            DependencyMediator.Instance.GameSceneManager.ReduceEnemy();
            Destroy(gameObject);
        }

        public void RemoveWeakPoint(EnemyWeakPoint enemyWeakPoint)
        {
            _enemyWeakPoints.Remove(enemyWeakPoint);
        }
    }
}