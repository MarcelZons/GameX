using UnityEngine;
using WG.GameX.Enemy;

namespace WG.GameX.Player
{
    public class SecondaryWeapon
    {
        private GameObject _beamMuzzleObject;
        private GameObject _beamLineObject;
        private GameObject _beamHitEffect;

        private LineRenderer _beamLineRenderer;
        private Transform _origin;
        private Transform _target;
        private LayerMask _layerMask;
        private bool _isFired;
        public bool IsReady { get; set; }
        public SecondaryWeapon(GameObject beamMuzzleObject, GameObject beamLineObject, GameObject beamHitEffect)
        {
            _beamMuzzleObject = beamMuzzleObject;
            _beamLineObject = beamLineObject;
            _beamHitEffect = beamHitEffect;
            _beamLineRenderer = beamLineObject.GetComponent<LineRenderer>();
            _beamMuzzleObject.SetActive(false);
            _beamLineObject.SetActive(false);
            _beamHitEffect.SetActive(false);
        }

        public void FireAtTarget(Transform origin, Transform target, LayerMask layerMask)
        {
            _origin = origin;
            _target = target;
            _layerMask = layerMask;
            SetFxStatus(true);
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
                
                if(_target == null)
                    return;
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
                 weakPoint.ReduceHealth(20);
                return raycastHit.point;
            }

            return Vector3.zero;
        }
    }
}