using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WG.GameX.Enemy
{
    public class EnemyShipController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        public LayerMask LayerMask => _layer;
    
        private List<EnemyWeakPoint> _enemyWeakPoints;
    
        public Vector3 Position => transform.position;
        public LayerMask Layer => gameObject.layer;
        void Start()
        {
            _enemyWeakPoints = GetComponentsInChildren<EnemyWeakPoint>().ToList();
        }
    }
}

