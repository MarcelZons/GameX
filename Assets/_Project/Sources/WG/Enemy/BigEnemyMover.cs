using UnityEngine;
using WG.GameX.Managers;

namespace WG.GameX.Enemy
{
    public class BigEnemyMover : MonoBehaviour
    {
        [Header("######## Tuning Component #############")]
        [Range(0, 100f)] [SerializeField] private float _moveMentSpeed;
        [Range(.01f, 1f)] [SerializeField] private float _turnSpeed;
        private Transform _playerTransform;
        private Transform _transform;

        private void Start()
        {
            _transform = transform;
            _playerTransform = DependencyMediator.Instance.PlayerShipController.transform;
        }

        private void Update()
        {
            var target = _playerTransform.position;
            target.y -= 50;
            var direction = (target - _transform.position).normalized;

            _transform.Translate(direction * (Time.deltaTime * _moveMentSpeed));
            
            direction = (_playerTransform.position - _transform.position).normalized;
            
            direction.y = 0;
            var horizontalLookRotation = Quaternion.LookRotation(direction);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, horizontalLookRotation, Time.deltaTime * _turnSpeed);
            
        }
    }
}