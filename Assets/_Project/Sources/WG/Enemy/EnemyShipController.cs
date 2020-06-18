using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        
        [Header("###############################")]
        [SerializeField] private LayerMask _weakpointLayer;
        

        public LayerMask WeakpointLayerMask => _weakpointLayer;
        private List<EnemyWeakPoint> _enemyWeakPoints;
        private List<EnemyTurret> _enemyTurrets;

        public Vector3 Position => transform.position;
        public LayerMask Layer => gameObject.layer;

        public List<EnemyWeakPoint> EnemyWeakPoints => _enemyWeakPoints;

        private void Awake()
        {
            _enemyWeakPoints = GetComponentsInChildren<EnemyWeakPoint>().ToList();
            _enemyTurrets = GetComponentsInChildren<EnemyTurret>().ToList();
        }

        private void Start()
        {
            foreach (var enemyTurret in _enemyTurrets)
            {
                enemyTurret.Setup(DependencyMediator.Instance.PlayerShipController.transform, _turretActivationDistance, _turretFrequency, _bulletVelocity);
            }
        }
    }
}