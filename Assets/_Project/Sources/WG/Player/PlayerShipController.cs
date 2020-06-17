using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WG.GameX.Player
{
    [RequireComponent(typeof(PlayerInputController))]
    public class PlayerShipController : MonoBehaviour
    {
        [Header("------ Manager Component ------")] [Header("-------Transforms----------")] [SerializeField]
        private Transform _shipTransform;

        [SerializeField] private Transform _pivotTransform;
        [SerializeField] private Transform _lookAtReferenceMiddle;
        [SerializeField] private Transform _lookAtReferenceBottom;
        [SerializeField] private List<Transform> _gunPoints;

        [Space(10)] [Header("___________Movement___________________")]
        [Range(0, 5f)] [SerializeField] private float _minSpeed;

        [Range(0f, 10f)] [SerializeField] private float _maxSpeed;
        [Range(1, 10f)] [SerializeField] private float _accelaration;

        private PlayerCameraController _playerCameraController;
        private PlayerShipMovement _playerShipMovement;
        private PlayerInputController _playerInputController;
        private PlayerShipAnimationController _animationController;
        private AttackController _attackController;
        private SecondaryWeaponStatusEvent secondaryWeaponReady;
        public float SpeedFactor => _playerShipMovement.SpeedFactor;
        public float SecondaryWeaponFilledStatus => _attackController.SecondaryWeaponFilledStatus;
        public SecondaryWeaponStatusEvent SecondaryWeaponReady => secondaryWeaponReady;
        public Camera MainCamera => _playerCameraController.CameraComponent;
        
        
        private void Awake()
        {
            _playerInputController = GetComponent<PlayerInputController>();
            _attackController = GetComponent<AttackController>();
            _playerCameraController = GetComponentInChildren<PlayerCameraController>();
            _playerCameraController.Setup(_lookAtReferenceMiddle, _lookAtReferenceBottom);
            _animationController = new PlayerShipAnimationController(_pivotTransform.GetComponent<Animator>());
            _playerShipMovement = new PlayerShipMovement(_shipTransform, _pivotTransform, _minSpeed, _maxSpeed,
                _accelaration, GetComponent<Rigidbody>());

            _attackController.Setup(_gunPoints);
            _attackController.SecondaryWeaponStatus.AddListener(OnSecondaryWeaponStatusUpdated);
            _playerInputController.SetUp(_playerCameraController.CameraComponent);
            _playerInputController.PrimaryWeaponFiredEvent.AddListener(PrimaryShotFired);
            _playerInputController.SecondaryWeaponFiredEvent.AddListener(SecondaryShotFired);

            secondaryWeaponReady = new SecondaryWeaponStatusEvent();
        }

        private void OnSecondaryWeaponStatusUpdated(string message)
        {
            SecondaryWeaponReady.Invoke(message);
        }

        private void PrimaryShotFired()
        {
            //_attackController.PrimaryWeaponFired(_pivotTransform.rotation, _pivotTransform.forward,  _playerShipMovement.SpeedFactor);
        }

        private void SecondaryShotFired()
        {
            _attackController.SecondaryWeaponFired();
        }

        private void Update()
        {
            _playerShipMovement.Update(_playerInputController.MouseScroll, _playerInputController.Horizontal);
            _animationController.Update(_playerInputController.Horizontal, _playerInputController.Vertical);

            _playerCameraController.UpdateCamera(_playerInputController.MouseDragHorizontal.x,
                _playerInputController.Vertical,
                _playerInputController.MouseDragHorizontal.y
                , _playerShipMovement.SpeedFactor);
        }
    }
}