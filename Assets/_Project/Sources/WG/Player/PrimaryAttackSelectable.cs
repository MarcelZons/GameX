using UnityEngine;
using UnityEngine.UI;
using WG.GameX.Enemy;

namespace WG.GameX.Player
{ 
    public class PrimaryAttackSelectable : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private Camera _mainCamera;

        [SerializeField] private Transform _leftOrigin;
        [SerializeField] private Transform _rightOrigin;
        
        [SerializeField] private RectTransform _aimCursor;
        [SerializeField] private Color _aimCursorMarked, _aimCursorNormal;
        
        private AttackController _attackController;
        private bool _isEnemySelected;
        private EnemyShipController _enemyShipController;
        private Image _aimCursorImage;
        
        private void Awake()
        {
            _attackController = GetComponent<AttackController>();
            _aimCursorImage = _aimCursor.GetComponent<Image>();
            Cursor.visible = false;
        }

        private void Update()
        {
            _aimCursor.position = Input.mousePosition;
            if (_isEnemySelected)
            {
                if (Input.GetMouseButton(0) && _enemyShipController != null)
                {
                    var weakPointTransforms = _enemyShipController.GetEnemyWeakpoints();
                    
                    if(weakPointTransforms!= null)
                        _attackController.SelectableFireCommand(weakPointTransforms,1f, _attackController.EnemyWeakPointLayerMask, _leftOrigin, _rightOrigin);
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
                _aimCursorImage.color = _aimCursorMarked;
            }
            else
            {
                _isEnemySelected = false;
                _aimCursorImage.color = _aimCursorNormal;
            }
        }
    }
}