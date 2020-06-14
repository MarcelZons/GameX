using System.Collections.Generic;
using UnityEngine;
using VSX.Pooling;
using WG.GameX.Common;

namespace WG.GameX.Player
{
    public class PrimaryWeapon
    {
        private float _primaryBulletVelocity;
        private List<Transform> _gunPoints;
        private GameObject _bulletPrefab;
        private int _primaryWeaponsIndex;

        public PrimaryWeapon(float primaryBulletVelocity, List<Transform> gunPoints, GameObject bulletPrefab)
        {
            _primaryBulletVelocity = primaryBulletVelocity;
            _gunPoints = gunPoints;
            _bulletPrefab = bulletPrefab;
        }

        public void FireShot(Quaternion rotation, Vector3 direction, float speedFactor)
        {
            var position = _gunPoints[(_primaryWeaponsIndex++) % _gunPoints.Count].transform.position;
            var bullet = PoolManager.Instance.Get(_bulletPrefab, position, rotation);
            var bulletVelocity = _primaryBulletVelocity + (_primaryBulletVelocity * speedFactor * 3);
            bullet.GetComponent<Bullet>().Fire(direction * bulletVelocity);
        }
    }
}