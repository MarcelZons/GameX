using UnityEngine;
using WG.GameX.Managers;

namespace WG.GameX.Enemy
{
    public class BigEnemyMover : MonoBehaviour
    {
        [Header("######## Tuning Component #############")] [Range(0, 100f)] [SerializeField]
        private float _moveMentSpeed;

        [Range(.01f, 1f)] [SerializeField] private float _turnSpeed;
        private Transform _playerTransform;
        private Transform _transform;

        private bool _isCloseWithOtherEnemy;

        private void Start()
        {
            _isCloseWithOtherEnemy = false;
            _transform = transform;
            _playerTransform = DependencyMediator.Instance.PlayerShipController.transform;
            InvokeRepeating(nameof(CheckDistanceFromOtherEnemy), 0f, 2f);
        }

        private void Update()
        {
            if (_isCloseWithOtherEnemy == false)
            {
                var target = _playerTransform.position;
                target.y -= 50;
                var direction = (target - _transform.position).normalized;

                _transform.position += (direction * (Time.deltaTime * _moveMentSpeed));

                direction = (_playerTransform.position - _transform.position).normalized;

                direction.y = 0;
                var horizontalLookRotation = Quaternion.LookRotation(direction);
                _transform.rotation = Quaternion.Slerp(_transform.rotation, horizontalLookRotation,
                    Time.deltaTime * _turnSpeed);
            }
        }

        private void CheckDistanceFromOtherEnemy()
        {
            _isCloseWithOtherEnemy = false;

            var otherEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var other in otherEnemies)
            {
                if (other.gameObject != gameObject)
                {
                    if (Vector3.Distance(other.transform.position, _transform.position) < 300)
                    {
                        _isCloseWithOtherEnemy = true;
                    }
                }
            }
        }
    }
}