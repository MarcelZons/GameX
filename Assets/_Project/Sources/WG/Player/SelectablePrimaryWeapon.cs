using UnityEngine;
using WG.GameX.Enemy;
using WG.GameX.Managers;

namespace WG.GameX.Player
{
    public class SelectablePrimaryWeapon: MonoBehaviour
    {
        [SerializeField] private GameObject _beamMuzzleObject;
        [SerializeField] private GameObject _beamLineObject;
        [SerializeField] private GameObject _beamHitEffect;

        private LineRenderer _beamLineRenderer;
        private Transform _origin;
        private Transform _target;
        private LayerMask _layerMask;
        private Transform _playerTransform;
        
        private bool _isFired;
        public bool IsReady { get; set; }
        public void Start()
        {
            _playerTransform = DependencyMediator.Instance.PlayerShipController.transform;
            _beamLineRenderer = _beamLineObject.GetComponent<LineRenderer>();
            _beamMuzzleObject.SetActive(false);
            _beamLineObject.SetActive(false);
            _beamHitEffect.SetActive(false);
        }

        public void FireAtTarget(Transform target, LayerMask layerMask, float duration, Transform leftOrigin, Transform rightOrigin)
        {
            if (_playerTransform.InverseTransformPoint(target.position).x < 0)
                _origin = leftOrigin;
            else
                _origin = rightOrigin;
            
            _target = target;
            _layerMask = layerMask;
            SetFxStatus(true);
            Invoke(nameof(StopFire),duration);
        }

        public void StopFire()
        {
            SetFxStatus(false);
        }

        private void SetFxStatus(bool status)
        {
            _isFired = status;
            _beamMuzzleObject.SetActive(status);
            _beamLineObject.SetActive(status);
            _beamHitEffect.SetActive(status);
        }

        public void Update()
        {
            if (_isFired)
            {
                _beamMuzzleObject.transform.position = _origin.position;
                var hitPoint = GetHitPoint(_origin, _target);

                if (hitPoint == Vector3.zero)
                    return;

                _beamLineRenderer.SetPosition(0, _origin.position);
                _beamLineRenderer.SetPosition(1, hitPoint);
                _beamHitEffect.transform.position = hitPoint;
            }
        }

        private Vector3 GetHitPoint(Transform origin, Transform target)
        {
            RaycastHit raycastHit;
            var direction = (target.position - origin.position).normalized;
            
            if (Physics.Raycast(origin.position, direction, out raycastHit, 3000, _layerMask))
            {
                var weakPoint = raycastHit.collider.GetComponent<EnemyWeakPoint>();
                weakPoint.ReduceHealth();
                return raycastHit.point;
            }

            return Vector3.zero;
        }
    }
}