using UnityEngine;

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
        
        private bool _isFired;
        public bool IsReady { get; set; }
        public void Start()
        {
            _beamLineRenderer = _beamLineObject.GetComponent<LineRenderer>();
            _beamMuzzleObject.SetActive(false);
            _beamLineObject.SetActive(false);
            _beamHitEffect.SetActive(false);
        }

        public void FireAtTarget(Transform origin, Transform target, LayerMask layerMask, float duration)
        {
            _origin = origin;
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
                return raycastHit.point;
            }

            return Vector3.zero;
        }
    }
}