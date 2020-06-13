using gamex.util;
using UnityEngine;
using UnityEngine.Events;

namespace gamex.player
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
        private float _mouseDragHorizontal;
        private bool _isShooting;
        

        public float Horizontal => _horizontal;
        public float Vertical => _vertical;
        public float MouseDragHorizontal => _mouseDragHorizontal;
        public float MouseScroll { get;private set; }

        public UnityEvent BulletFiredEvent => _bulletFiredEvent;

        private MouseClickDrag mouseClickDrag;

        private UnityEvent _bulletFiredEvent;

        private void Awake()
        {
            _bulletFiredEvent = new UnityEvent();
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
            _mouseDragHorizontal = _mouseDragHorizontal.GetSmoothDamping(mouseClickDrag.GetMouseDrag(),1f);

            // shot input
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
        }
        
        private void FireShot()
        {
            _bulletFiredEvent.Invoke();
        }
    }
}