using System.Collections.Generic;
using UnityEngine;
using VSX.Pooling;
using VSX.UniversalVehicleCombat;

namespace gamex.player
{
    public class AttackController: MonoBehaviour
    {
        [SerializeField] private float _primaryBulletVelocity; 
        [SerializeField] private List<Transform> _gunPoints;
        //[SerializeField] private ObjectPool _bulletPool;
        [SerializeField] private GameObject _bulletPrefab;
        private int _gunPointIndex;

        public void Setup(List<Transform> gunPoints)
        {
            _gunPoints = gunPoints;
        }
        
        public void FireShot(Quaternion rotation, float speedFactor)
        {
            var position = _gunPoints[(_gunPointIndex++) % _gunPoints.Count].transform.position;
            var bullet = PoolManager.Instance.Get(_bulletPrefab,position,rotation);
            var bulletVelocity = _primaryBulletVelocity + (_primaryBulletVelocity * speedFactor * 3);
            bullet.GetComponent<RigidbodyMover>().Fire(Vector3.forward * bulletVelocity);
        }
    }
}