using UnityEngine;
using VSX.Pooling;
using VSX.UniversalVehicleCombat;
using WG.GameX.Common;
using Random = UnityEngine.Random;

namespace WG.GameX.Enemy
{
    public class EnemyTurret : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _verticalArm;
        [SerializeField] private Transform _horizontalArm;
        [SerializeField] private Transform _originPoint;

        private Transform _playerTransform;
        private FlashController _flashController;

        private float _activationDistance;
        private float _bulletVelocity;
        private float _turretRobustness;

        public float ActivationDistance => _activationDistance;


        private void Awake()
        {
            _flashController = GetComponentInChildren<FlashController>();
        }

        //enemyTurret.Setup(DependencyMediator.Instance.PlayerShipController.transform, _turretActivationDistance, _turretFrequency, _bulletVelocity);
        public void Setup(Transform playerTransform, float turretActivationDistance, float turretFrequency,
            float bulletVelocity, float turretRobustness)
        {
            _playerTransform = playerTransform;
            InvokeRepeating(nameof(ShootAtPlayer), 0, Random.Range(.15f, turretFrequency));
            _activationDistance = turretActivationDistance;
            _bulletVelocity = bulletVelocity;
            _turretRobustness = turretRobustness;
        }

        private void Update()
        {
            if (Vector3.Distance(_playerTransform.position, transform.position) > ActivationDistance)
                return;

            var direction = (_playerTransform.position - _horizontalArm.position).normalized;
            var horizontalDirection = direction;
            horizontalDirection.y = 0;
            var horizontalLookRotation = Quaternion.LookRotation(horizontalDirection);
            _horizontalArm.rotation = Quaternion.Slerp(_horizontalArm.rotation, horizontalLookRotation,
                Time.deltaTime * _turretRobustness);

            var verticalDirection = direction;
            var verticalLookRotation = Quaternion.LookRotation(verticalDirection);
            _verticalArm.rotation = Quaternion.Slerp(_verticalArm.rotation, verticalLookRotation,
                Time.deltaTime * _turretRobustness);
        }

        private void ShootAtPlayer()
        {
            if (Vector3.Distance(_playerTransform.position, transform.position) > ActivationDistance)
                return;

            if (_playerTransform.position.y < transform.position.y)
                return;

            _flashController.Flash();
            var bullet = PoolManager.Instance.Get(_bulletPrefab, _originPoint.position, transform.rotation);
            var offset = new Vector3(0, Random.Range(-10, 10), 0);
            var direction = (_playerTransform.position + offset - _originPoint.position).normalized;
            bullet.GetComponent<Bullet>().Fire(direction * _bulletVelocity);
        }
    }
}