using System.Collections.Generic;
using UnityEngine;

namespace gamex.player
{
    [RequireComponent(typeof(PlayerInputController))]
    public class PlayerShipController : MonoBehaviour
    {
        [Header("-------Transforms----------")]
        [SerializeField] private Transform _shipTransform;
        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private Transform _lookAtReferenceMiddle;
        [SerializeField] private Transform _lookAtReferenceBottom;
        [SerializeField] private List<Transform> _gunPoints;
        
        [Space(10)]
        [Header("___________Movement___________________")] 
        [Range(.1f,5f)]
        [SerializeField] private float _minSpeed;
        [Range(.1f,5f)]
        [SerializeField] private float _maxSpeed;
        [Range(1,10f)]
        [SerializeField] private float _accelaration;

        private PlayerCameraController _playerCameraController;
        private PlayerShipMovement _playerShipMovement;
        private PlayerInputController _playerInputController;
        private PlayerShipAnimationController _animationController;
        private AttackController _attackController;

        private void Awake()
        {
            _playerInputController = GetComponent<PlayerInputController>();
            _attackController = GetComponent<AttackController>();
            _playerCameraController = GetComponentInChildren<PlayerCameraController>();
            _playerCameraController.Setup(_lookAtReferenceMiddle, _lookAtReferenceBottom);
            _animationController = new PlayerShipAnimationController(_pivotTransform.GetComponent<Animator>());
            _playerShipMovement = new PlayerShipMovement(_shipTransform,_pivotTransform,_minSpeed, _maxSpeed, _accelaration);
            _attackController.Setup(_gunPoints);
            _playerInputController.SetUp(_playerCameraController.CameraComponent);
            _playerInputController.BulletFiredEvent.AddListener(ShotFire);
        }

        private void ShotFire()
        {
            _attackController.FireShot(_pivotTransform.rotation, _playerShipMovement.SpeedFactor);
        }

        private void Update()
        {
            _playerShipMovement.Update(_playerInputController.MouseScroll, _playerInputController.Horizontal);
            _animationController.Update(_playerInputController.Horizontal, _playerInputController.Vertical);
            _playerCameraController.UpdateCamera(_playerInputController.MouseDragHorizontal,_playerInputController.Vertical,_playerShipMovement.SpeedFactor);
        }
    }
}
