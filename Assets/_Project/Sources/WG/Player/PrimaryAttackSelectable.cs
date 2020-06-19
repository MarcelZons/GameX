using System.Collections.Generic;
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

        // AutoShoot variables
        private float _autoShootFrequencey;
        private float _autoShootDuration;
        private float _range;
        
        private PlayerRadar _leftRadar;
        private float _autoShootLeftTime;
        private PlayerRadar _rightRadar;
        private float _autoShootRightTime;

        private void Awake()
        {
            _attackController = GetComponent<AttackController>();
            _aimCursorImage = _aimCursor.GetComponent<Image>();
            Cursor.visible = false;
        }

        private void Start()
        {
            _range = GetComponent<PlayerShipController>().PrimaryAttackRange;
            _autoShootFrequencey = GetComponent<PlayerShipController>().PrimaryWeaponShootingFrequency;
            _autoShootDuration = GetComponent<PlayerShipController>().PrimaryWeaponShootingDuration;
            _leftRadar = GetComponent<PlayerShipController>().PlayerLeftRadar;
            _rightRadar = GetComponent<PlayerShipController>().PlayerRightRadar;
            _autoShootLeftTime = 0;
            _autoShootRightTime = 0;
        }

        private void Update()
        {
            // Shot with Mosue Click
            _aimCursor.position = Input.mousePosition;
            if (_isEnemySelected)
            {
                if (Input.GetMouseButton(0) && _enemyShipController != null)
                {
                    var weakPointTransforms = _enemyShipController.GetEnemyWeakpoints();

                    if (weakPointTransforms != null)
                        _attackController.SelectableFireCommand(weakPointTransforms, 1f,
                            _attackController.EnemyWeakPointLayerMask, _leftOrigin, _rightOrigin);
                }
            }

            // auto shot based on Radar
            AutoShootFromRadar(_leftRadar, ref _autoShootLeftTime, _leftOrigin, true);
            AutoShootFromRadar(_rightRadar, ref _autoShootRightTime, _rightOrigin, false);

            _autoShootLeftTime += Time.deltaTime;
            _autoShootRightTime += Time.deltaTime;
        }

        private void AutoShootFromRadar(PlayerRadar radar, ref float autoShootTime, Transform origin, bool isLeftWeapon)
        {
            if (autoShootTime >= _autoShootFrequencey)
            {
                var nearestWeakPoint = radar.GetNearestWeakPoint();
                if (nearestWeakPoint == null)
                    return;

                _attackController.SelectableFireCommand(nearestWeakPoint.transform, _autoShootDuration,
                    _attackController.EnemyWeakPointLayerMask, origin, isLeftWeapon);
                
                autoShootTime = 0;
            }
        }

        private void FixedUpdate()
        {
            RaycastHit hit;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, _range, _enemyLayerMask))
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