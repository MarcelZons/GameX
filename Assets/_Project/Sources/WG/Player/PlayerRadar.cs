using System.Collections.Generic;
using System.Linq;
using WG.GameX.Util;
using UnityEngine;
using WG.GameX.Enemy;

namespace WG.GameX.Player
{
    public class PlayerRadar : MonoBehaviour
    {
        private List<EnemyWeakPoint> _enemyWeakPoints;
        [SerializeField] private List<EnemyShipController> _enemyShipControllers;
        private Camera _cameraComponent;
        private void Start()
        {
            _enemyWeakPoints = new List<EnemyWeakPoint>();
            _enemyShipControllers = new List<EnemyShipController>();
        }

        public bool HasEnemyWeakPoints => _enemyWeakPoints.Count > 0;
        public bool HasEnemy => _enemyShipControllers.Count > 0;

        public EnemyWeakPoint GetNearestWeakPoint()
        {
            var min = _enemyWeakPoints.Min(p => DistanceWithPlayer(p.transform));
            var first = _enemyWeakPoints.FirstOrDefault(p => Mathf.Approximately(DistanceWithPlayer(p.transform), min));
            return first;
        }
        public EnemyShipController GetNearestEnemy()
        {
            var min = _enemyShipControllers.Min(p => DistanceWithPlayer(p.transform));
            var first = _enemyShipControllers.FirstOrDefault(p => Mathf.Approximately(DistanceWithPlayer(p.transform), min));
            return first;
        }

        private float DistanceWithPlayer(Transform target)
        {
            return Vector3.Distance(target.position, transform.position);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.EnemyWeakPointTag))
            {
                _enemyWeakPoints.Add(other.GetComponent<EnemyWeakPoint>());
            }

            if (other.CompareTag(Constants.EnemyTag))
            {
                _enemyShipControllers.Add(other.GetComponent<EnemyShipController>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.EnemyWeakPointTag))
            {
                _enemyWeakPoints.Remove(other.GetComponent<EnemyWeakPoint>());
            }

            if (other.CompareTag(Constants.EnemyTag))
            {
                _enemyShipControllers.Remove(other.GetComponent<EnemyShipController>());
            }
        }
    }
}