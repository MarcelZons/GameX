using UnityEngine;

namespace gamex.player
{
    public class MouseClickDrag
    {
        private readonly Camera _camera;
        private Vector2 _mouseCurrentPos;
        private Vector2 _firstClickPos;
        private bool _panning;

        public MouseClickDrag(Camera camera)
        {
            _camera = camera;
        }

        public float GetMouseDrag()
        {
            var distance = 0f;
            
            if (Input.GetKeyDown(KeyCode.Mouse0) && !_panning)
            {
                _firstClickPos = _camera.ScreenToViewportPoint(Input.mousePosition);
                _panning = true;
            }
            
            if (_panning)
            {
                _mouseCurrentPos = _camera.ScreenToViewportPoint(Input.mousePosition);
                distance = _mouseCurrentPos.x - _firstClickPos.x;
            }
 
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _mouseCurrentPos = Vector2.zero;
                _firstClickPos = Vector2.zero;
                _panning = false;
                distance = 0;
            }
            
            return distance * 180;
        }
    }
}