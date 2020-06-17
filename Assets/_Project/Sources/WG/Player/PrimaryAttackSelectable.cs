using System.Linq;
using UnityEngine;
using WG.GameX.Enemy;
using WG.GameX.Util;

namespace WG.GameX.Player
{ 
    public class PrimaryAttackSelectable : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private Camera _mainCamera;

        [SerializeField] private Transform _leftOrigin;
        [SerializeField] private Transform _rightOrigin;
        
        public Texture2D cursorTexture;
        public CursorMode cursorMode = CursorMode.Auto;
        public Vector2 hotSpot = Vector2.zero;

        private AttackController _attackController;

        private bool _isEnemySelected;

        private EnemyShipController _enemyShipController;

        private void Awake()
        {
            _attackController = GetComponent<AttackController>();
        }

        private void Update()
        {
            if (_isEnemySelected)
            {
                if (Input.GetMouseButton(0) && _enemyShipController != null)
                {
                    if (_enemyShipController.EnemyWeakPoints.Count == 0)
                    {
                        _attackController.SelectableFireCommand(_enemyShipController.transform.ToList(), 1f,
                            _enemyShipController.LayerMask, _leftOrigin, _rightOrigin);
                    }
                    else
                    {
                        var weakPointTransforms = _enemyShipController.EnemyWeakPoints.Select(point => point.transform)
                            .ToList();
                        _attackController.SelectableFireCommand(weakPointTransforms,1f,
                            _enemyShipController.LayerMask, _leftOrigin, _rightOrigin);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            RaycastHit hit;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 5000, _enemyLayerMask))
            {
                _isEnemySelected = true;
                _enemyShipController = hit.collider.gameObject.GetComponent<EnemyShipController>();
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            }
            else
            {
                _isEnemySelected = false;
                Cursor.SetCursor(null, hotSpot, cursorMode);
            }
        }
    }
}