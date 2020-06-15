using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using WG.GameX.Managers;

namespace WG.GameX.Enemy
{
    public class EnemyShipController : MonoBehaviour
    {
        [SerializeField] private float _turretActivationDistance = 300;
        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private LayerMask _layer;
        public LayerMask LayerMask => _layer;

        private List<EnemyWeakPoint> _enemyWeakPoints;
        private List<EnemyTurret> _enemyTurrets;

        public Vector3 Position => transform.position;
        public LayerMask Layer => gameObject.layer;

        private void Awake()
        {
            _enemyWeakPoints = GetComponentsInChildren<EnemyWeakPoint>().ToList();
            _enemyTurrets = GetComponentsInChildren<EnemyTurret>().ToList();
            
            foreach (var enemyTurret in _enemyTurrets)
            {
                enemyTurret.Setup(_gameSceneManager.DependencyMediator.PlayerShipController.transform, _turretActivationDistance);
            }
        }
    }
}