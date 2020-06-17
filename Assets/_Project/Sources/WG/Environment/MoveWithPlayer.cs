using UnityEngine;
using WG.GameX.Managers;

namespace WG.GameX.Environment
{
    public class MoveWithPlayer: MonoBehaviour
    {
        private Transform _playerTransform;
        
        private Vector3 _offset;
        private void Start()
        {
            _playerTransform =  DependencyMediator.Instance.PlayerShipController.transform;
            _offset = transform.position - _playerTransform.position;
        }

        private void Update()
        {
            var position = transform.position;
            position.z = _playerTransform.position.z + _offset.z;
            transform.position = position;
        }
    }
}