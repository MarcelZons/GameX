using UnityEngine;
using UnityEngine.Serialization;


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
        [Space(10)]
        [Header("___________Movement___________________")] 
        [Range(1,100f)]
        [SerializeField] private float _minSpeed;
        [Range(1,100f)]
        [SerializeField] private float _maxSpeed;
        [Range(1,10f)]
        [SerializeField] private float _accelaration;
        
        [Space(10)]
        [Header("-------- Components-------------")]
        [SerializeField] private PlayerCameraController _playerCameraController;
        
        private PlayerShipMovement _playerShipMovement;
        private PlayerInputController _playerInputController;
        private PlayerShipAnimationController _animationController;

        private void Awake()
        {
            _playerInputController = GetComponent<PlayerInputController>();
            _animationController = new PlayerShipAnimationController(_pivotTransform.GetComponent<Animator>());
            _playerShipMovement = new PlayerShipMovement(_shipTransform,_pivotTransform,_minSpeed, _maxSpeed, _accelaration);
            _playerCameraController.Setup(_lookAtReferenceMiddle, _lookAtReferenceBottom);
        }
        
        private void Update()
        {
            _playerShipMovement.Update(_playerInputController.MouseScroll, _playerInputController.Horizontal);
            _animationController.Update(_playerInputController.Horizontal, _playerInputController.Vertical);
            _playerCameraController.UpdateCamera(_playerInputController.MouseDragHorizontal,_playerInputController.Vertical,_playerShipMovement.SpeedFactor);
        }

       
    }
}
