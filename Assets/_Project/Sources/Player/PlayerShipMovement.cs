using gamex.util;
using UnityEngine;

namespace gamex.player
{
    public class PlayerShipMovement
    {
        private readonly Transform _shipTransform;
        private readonly Transform _pivotTransform;
        private readonly float _minSpeed;
        private readonly float _maxSpeed;
        private readonly float _accelaration;
        private float _forwardSpeed;
        private float _lerpedForwardMovementSpeed;

        public float SpeedFactor => _forwardSpeed / _maxSpeed;
        
        public PlayerShipMovement(Transform shipTransform, Transform pivotTransform, float minSpeed, float maxSpeed, float accleration)
        {
            _shipTransform = shipTransform;
            _minSpeed = minSpeed;
            _maxSpeed = maxSpeed;
            _accelaration = accleration;
            _pivotTransform = pivotTransform;
            _lerpedForwardMovementSpeed = minSpeed;
        }

        public void Update(float mouseScroll, float horizontalValue)
        {
            // Rotation
            _shipTransform.Rotate(Vector3.up * horizontalValue);
            _lerpedForwardMovementSpeed += mouseScroll;
            _lerpedForwardMovementSpeed = _lerpedForwardMovementSpeed.Clamp(_minSpeed, _maxSpeed);
            _forwardSpeed = _forwardSpeed.GetSmoothDamping(_lerpedForwardMovementSpeed, _accelaration);
            var position = Vector3.Normalize(_pivotTransform.forward);
            _shipTransform.position += position * _forwardSpeed;
        }
    }
}