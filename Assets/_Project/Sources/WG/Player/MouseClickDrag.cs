using UnityEngine;

namespace WG.GameX.Player
{
    public class MouseClickDrag
    {
        private readonly Camera _camera;
        private Vector2 _mouseCurrentPos;
        private Vector2 _firstClickPos;

        private Vector2 _distance;
        private bool _panning;

        public MouseClickDrag(Camera camera)
        {
            _camera = camera;
            _distance = Vector2.zero;
        }

        public Vector2 GetMouseDrag()
        {
            // if (Input.GetKeyDown(KeyCode.Mouse1) && !_panning)
            // {
            //     _firstClickPos = _camera.ScreenToViewportPoint(Input.mousePosition);
            //     _panning = true;
            // }
            //
            // if (_panning)
            // {
            //     _mouseCurrentPos = _camera.ScreenToViewportPoint(Input.mousePosition);
            //     distance = _mouseCurrentPos.x - _firstClickPos.x;
            // }
            //
            // if (Input.GetKeyUp(KeyCode.Mouse1))
            // {
            //     _mouseCurrentPos = Vector2.zero;
            //     _firstClickPos = Vector2.zero;
            //     _panning = false;
            //     distance = 0;
            // }
            
            _mouseCurrentPos = _camera.ScreenToViewportPoint(Input.mousePosition);
            _distance.x = (_mouseCurrentPos.x - .5f) * 180f;//_mouseCurrentPos.x - _firstClickPos.x;
            _distance.y = _mouseCurrentPos.y;
            return _distance;
        }
    }
}