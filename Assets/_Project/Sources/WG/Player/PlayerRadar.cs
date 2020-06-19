using System;
using System.Collections.Generic;
using System.Linq;
using WG.GameX.Util;
using UnityEngine;
using WG.GameX.Enemy;

namespace WG.GameX.Player
{
    public class PlayerRadar : MonoBehaviour
    {
        private readonly object _lock = new object();
        
        private List<EnemyWeakPoint> _enemyWeakPoints;
        private Camera _cameraComponent;

        private void Start()
        {
            _enemyWeakPoints = new List<EnemyWeakPoint>();
        }

        public bool HasEnemyWeakPoints => _enemyWeakPoints.Count > 0;

        public EnemyWeakPoint GetNearestWeakPoint()
        {
            if (_enemyWeakPoints.Count == 0)
                return null;

            var activeWeakPoints = new List<EnemyWeakPoint>();
            try
            {
                lock (_lock)
                {
                    activeWeakPoints = _enemyWeakPoints.Where(p => p!= null).ToList();
                    var min = activeWeakPoints.Min(p => DistanceWithPlayer(p.transform));
                    var first = activeWeakPoints.FirstOrDefault(p => Mathf.Approximately(DistanceWithPlayer(p.transform), min));
                    return first;                    
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                return null;
            }
            
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
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.EnemyWeakPointTag))
            {
                _enemyWeakPoints.Remove(other.GetComponent<EnemyWeakPoint>());
            }
        }

        public void RemoveFromList(EnemyWeakPoint enemyWeakPoint)
        {
            lock (_lock)
            {
                _enemyWeakPoints.Remove(enemyWeakPoint);                
            }
            
        }
    }
}