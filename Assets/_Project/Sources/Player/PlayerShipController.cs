using gamex.util;
using UnityEngine;


namespace gamex.player
{
    [RequireComponent(typeof(PlayerInputController) , typeof(PlayerShipAnimationController) )]
    public class PlayerShipController : MonoBehaviour
    {
        [Header("Transforms")]
        [SerializeField] private Transform _shipTransform;

        [Space(10)] 
        [Header("Movement")] 
        [Range(1,100f)]
        [SerializeField] private float _minSpeed;
        [Range(1,100f)]
        [SerializeField] private float _maxSpeed;
        [Range(1,10f)]
        [SerializeField] private float _accelaration;
        
        [Space(20)]
        [Header("Damping Values: (Low to High) = (Slower to Agile)")]
        [Range(1f,5f)]
        [SerializeField] private float _horizontalDamping;
        [Range(1f,10f)]
        [SerializeField] private float _verticalDamping;


        private PlayerInputController _playerInputController;
        private PlayerShipAnimationController _animationController;

        private float _horizontalValue;
        private float _verticalValue;
        private float _forwardSpeed;
        private float _lerpedForwardMovementSpeed;

        private void Awake()
        {
            _playerInputController = GetComponent<PlayerInputController>();
            _animationController = GetComponent<PlayerShipAnimationController>();
        }

        private void Start()
        {
            _lerpedForwardMovementSpeed = _minSpeed;
        }
        
        private void Update()
        {
            _horizontalValue = _horizontalValue.GetSmoothDamping(_playerInputController.Horizontal,_horizontalDamping);
            _verticalValue = _verticalValue.GetSmoothDamping(_playerInputController.Vertical, _verticalDamping);
            
            // Rotation
            _shipTransform.Rotate(Vector3.up * _horizontalValue);
            
            // Forward Movement
            _lerpedForwardMovementSpeed += _playerInputController.MouseScroll;
            _lerpedForwardMovementSpeed = _lerpedForwardMovementSpeed.Clamp(_minSpeed, _maxSpeed);
            _forwardSpeed = _forwardSpeed.GetSmoothDamping(_lerpedForwardMovementSpeed, _accelaration);
            _shipTransform.Translate((Vector3.forward * _forwardSpeed) +  (Vector3.up * _verticalValue));
            
            // Animation Update
            _animationController.UpdateAnimator(_horizontalValue, _verticalValue);   
        }
    }
}
