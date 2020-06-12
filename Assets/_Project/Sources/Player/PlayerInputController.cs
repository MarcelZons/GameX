using gamex.util;
using UnityEngine;

namespace gamex.player
{
    public class PlayerInputController: MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        private float _horizontal;
        private float _vertical;
        private float _mouseDragHorizontal;
        public float MouseScroll { get;private set; }
        
        [Space(20)]
        [Header("Damping Values: (Low to High) = (Slower to Agile)")]
        [Range(1f,5f)]
        [SerializeField] private float _horizontalDamping;
        [Range(1f,10f)]
        [SerializeField] private float _verticalDamping;

        public float Horizontal => _horizontal;
        public float Vertical => _vertical;
        public float MouseDragHorizontal => _mouseDragHorizontal;

        private MouseClickDrag mouseClickDrag;


        private void Awake()
        {
            mouseClickDrag = new MouseClickDrag(_camera);
        }

        private void Update()
        {
            MouseScroll = Input.mouseScrollDelta.y;
            _horizontal = Horizontal.GetSmoothDamping(Input.GetAxis("Horizontal"),_horizontalDamping);
            _vertical = Vertical.GetSmoothDamping(Input.GetAxis("Vertical"), _verticalDamping);
            _mouseDragHorizontal = _mouseDragHorizontal.GetSmoothDamping(mouseClickDrag.GetMouseDrag(),1f);
        }
    }
}