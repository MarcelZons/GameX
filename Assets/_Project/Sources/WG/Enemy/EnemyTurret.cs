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
        [SerializeField] private FlashController _flashController;

        private float _activationDistance;

        public float ActivationDistance => _activationDistance;


        private void Awake()
        {
            _flashController = GetComponentInChildren<FlashController>();
        }

        public void Setup(Transform playerTransform, float turretActivationDistance)
        {
            _playerTransform = playerTransform;
            InvokeRepeating(nameof(ShootAtPlayer), 0, Random.Range(.25f, 1f));
            _activationDistance = turretActivationDistance;
        }

        private void Update()
        {
            if(Vector3.Distance(_playerTransform.position, transform.position) > ActivationDistance)
                return;
            
            var direction = (_playerTransform.position - _horizontalArm.position).normalized;
            var horizontalDirection = direction;
            horizontalDirection.y = 0;
            var horizontalLookRotation = Quaternion.LookRotation(horizontalDirection);
            _horizontalArm.rotation = Quaternion.Slerp(_horizontalArm.rotation, horizontalLookRotation, Time.deltaTime);

            var verticalDirection = direction;
            var verticalLookRotation = Quaternion.LookRotation(verticalDirection);
            _verticalArm.rotation = Quaternion.Slerp(_verticalArm.rotation, verticalLookRotation, Time.deltaTime);
        }

        private void ShootAtPlayer()
        {
            if(Vector3.Distance(_playerTransform.position, transform.position) > ActivationDistance)
                return;
            
            if (_playerTransform.position.y < transform.position.y)
                return;

            _flashController.Flash();
            var bullet = PoolManager.Instance.Get(_bulletPrefab, _originPoint.position, transform.rotation);
            var direction = (_playerTransform.position - transform.position).normalized;
            bullet.GetComponent<Bullet>().Fire(direction * 100);
        }
    }
}