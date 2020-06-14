using UnityEngine;
using UnityEngine.Events;
using WG.GameX.Util;

namespace WG.GameX.Player
{
    public class PlayerInputController: MonoBehaviour
    {
        [Space(20)]
        [Header("Damping Values: (Low to High) = (Slower to Agile)")]
        [Range(1f,5f)]
        [SerializeField] private float _horizontalDamping;
        [Range(1f,10f)]
        [SerializeField] private float _verticalDamping;
        
        private float _horizontal;
        private float _vertical;
        private Vector2 _mouseDragHorizontal;
        private bool _isShooting;
        

        public float Horizontal => _horizontal;
        public float Vertical => _vertical;
        public Vector2 MouseDragHorizontal => _mouseDragHorizontal;
        public float MouseScroll { get;private set; }

        public UnityEvent PrimaryWeaponFiredEvent => _primaryWeaponFiredEvent;
        private UnityEvent _primaryWeaponFiredEvent;
        
        public UnityEvent SecondaryWeaponFiredEvent => _secondaryWeaponFiredEvent;
        private UnityEvent _secondaryWeaponFiredEvent;
        
        private MouseClickDrag mouseClickDrag;
        
        private void Awake()
        {
            _primaryWeaponFiredEvent = new UnityEvent();
            _secondaryWeaponFiredEvent = new UnityEvent();
        }
        
        public void SetUp(Camera cameraComponent)
        {
            mouseClickDrag = new MouseClickDrag(cameraComponent);
        }

        private void Update()
        {
            MouseScroll = Input.mouseScrollDelta.y;
            _horizontal = Horizontal.GetSmoothDamping(Input.GetAxis("Horizontal"),_horizontalDamping);
            _vertical = Vertical.GetSmoothDamping(Input.GetAxis("Vertical"), _verticalDamping);
            _mouseDragHorizontal = _mouseDragHorizontal.GetSmoothDamping(mouseClickDrag.GetMouseDrag(),5f);

            // Primary Weapon Input
            if (Input.GetMouseButtonDown(0) && _isShooting == false)
            {
                _isShooting = true;
                InvokeRepeating(nameof(FireShot),0f,.1f);
            }

            if (Input.GetMouseButtonUp(0) && _isShooting == true)
            {
                CancelInvoke(nameof(FireShot));
                _isShooting = false;
            }
            
            // Secondary Weapon Input

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _secondaryWeaponFiredEvent.Invoke();
            }
        }
        
        private void FireShot()
        {
            _primaryWeaponFiredEvent.Invoke();
        }
    }
}