using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WG.GameX.Enemy;
using WG.GameX.Util;

namespace WG.GameX.Player
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyWeakPointLayerMask;
        
        [Header("Primary Weapons ------------------------")] [SerializeField]
        private float _primaryBulletVelocity;

        [SerializeField] private List<Transform> _gunPoints;
        [SerializeField] private GameObject _bulletPrefab;
        private PrimaryWeapon _primaryWeapon;

        [Space(10)] [Header("Secondary Weapons ------------------------")] 
        [SerializeField] private Transform _originTransform;

        [SerializeField] private float _secondaryWeapnFireFrequency;
        [SerializeField] private GameObject _beamGlowObject;
        [SerializeField] private GameObject _beamSpawnObject;
        [SerializeField] private GameObject _beamHeatEffect;
        [SerializeField] private PlayerRadar _playerRadar;

        [Space(10)] [Header("Selectable Primary Weapons ------------------------")] 
        [SerializeField] private SelectablePrimaryWeapon _selectableWeaponPrefab;
        [SerializeField] private int _numberOfSelectableWeapons;
        private SelectablePrimaryWeapon[] _selectablePrimaryWeapons;
        
        private SecondaryWeapon _secondaryWeapon;
        private float _timeSinceSecondaryWeaponFired;
        private float _secondaryWeaponFillStatus;
        private List<Transform> _selectableEnemyWeakPointsTransform;
        private SecondaryWeaponStatusEvent _secondaryWeaponStatus;

        private SelectablePrimaryWeapon _leftPrimaryWeapon;
        private SelectablePrimaryWeapon _rightPrimaryWeapon;

        public float SecondaryWeaponFilledStatus
        {
            get
            {
                var currentReadiness = (_timeSinceSecondaryWeaponFired / _secondaryWeapnFireFrequency).Clamp(0, 1f);
                _secondaryWeaponFillStatus =
                    Mathf.MoveTowards(_secondaryWeaponFillStatus, currentReadiness, Time.deltaTime * 10);
                return _secondaryWeaponFillStatus;
            }
        }

        public SecondaryWeaponStatusEvent SecondaryWeaponStatus => _secondaryWeaponStatus;

        public LayerMask EnemyWeakPointLayerMask => _enemyWeakPointLayerMask;

        private void Awake()
        {
            _secondaryWeaponStatus = new SecondaryWeaponStatusEvent();
            _selectablePrimaryWeapons = new SelectablePrimaryWeapon[_numberOfSelectableWeapons];
            
            for (int i = 0; i < _numberOfSelectableWeapons; i++)
            {
                _selectablePrimaryWeapons[i] = Instantiate(_selectableWeaponPrefab, transform);
            }

            _leftPrimaryWeapon = Instantiate(_selectableWeaponPrefab, transform);
            _rightPrimaryWeapon = Instantiate(_selectableWeaponPrefab, transform);
            _selectableEnemyWeakPointsTransform = new List<Transform>();
        }


        public void Setup(List<Transform> gunPoints)
        {
            _gunPoints = gunPoints;
            _primaryWeapon = new PrimaryWeapon(_primaryBulletVelocity, _gunPoints, _bulletPrefab);
            _secondaryWeapon = new SecondaryWeapon(_beamGlowObject, _beamSpawnObject, _beamHeatEffect);
        }

        public void PrimaryWeaponFired(Quaternion rotation, Vector3 direction, float speedFactor)
        {
            _primaryWeapon.FireShot(rotation, direction, speedFactor);
        }

        public void SecondaryWeaponFired()
        {
            if (_secondaryWeapon.IsReady == false)
            {
                _secondaryWeaponStatus.Invoke("Beam is not fully charged");
                return;
            }

            if (_playerRadar.HasEnemyWeakPoints == false)
            {
                _secondaryWeaponStatus.Invoke("No Enemy in the Radar");
                return;
            }

            if (_playerRadar.HasEnemyWeakPoints)
            {
                var nearestWeakPoint = _playerRadar.GetNearestWeakPoint();
                if (nearestWeakPoint != null)
                {
                    _secondaryWeapon.FireAtTarget(_originTransform, nearestWeakPoint.transform, EnemyWeakPointLayerMask);
                    Invoke(nameof(StopSecondaryFire), 1f);  
                    _secondaryWeapon.IsReady = false;
                    _timeSinceSecondaryWeaponFired = 0;
                }   
            }
        }

        private void Update()
        {
            if (_timeSinceSecondaryWeaponFired > _secondaryWeapnFireFrequency)
            {
                if (_secondaryWeapon.IsReady == false)
                {
                    _secondaryWeapon.IsReady = true;
                    _secondaryWeaponStatus.Invoke("Beam is fully Charged!\nPress Space to Fire");
                }
            }

            _timeSinceSecondaryWeaponFired += Time.deltaTime;
            _secondaryWeapon.Update();
        }

        public void SelectableFireCommand(Transform enemyWeakPoint, float hitDuration, LayerMask layerMask, Transform origin, bool isLeftWeapon)
        {
            //StartCoroutine(IterateWeapons(enemyWeakPoint, hitDuration, layerMask,origin, isLeftWeapon));
            if(isLeftWeapon)
                _leftPrimaryWeapon.FireAtTarget(enemyWeakPoint, layerMask, hitDuration, origin);
            else
                _rightPrimaryWeapon.FireAtTarget(enemyWeakPoint, layerMask, hitDuration, origin);
        }
        
        public void SelectableFireCommand(List<Transform> enemyWeakPoints, float hitDuration, LayerMask layerMask, Transform origin, Transform rightOrigin)
        {
            StartCoroutine(IterateWeapoints(enemyWeakPoints, hitDuration, layerMask,origin, rightOrigin));
        }

        private IEnumerator IterateWeapoints(List<Transform> _enemyWeakPoints, float hitDuration, LayerMask layerMask, Transform leftOrigin, Transform rightOrigin)
        {
            var weakPointIndex = 0;
            
            while (weakPointIndex < _enemyWeakPoints.Count)
            {
                for (int i = 0; i < _numberOfSelectableWeapons; i++)
                {
                    if (weakPointIndex == _enemyWeakPoints.Count)
                    {
                        break;
                    }
                    
                    _selectablePrimaryWeapons[i].FireAtTarget( _enemyWeakPoints[weakPointIndex], layerMask, hitDuration, leftOrigin, rightOrigin);
                    weakPointIndex++;
                }
                yield return new WaitForSeconds(hitDuration);
            }
        }
        

        private void StopSecondaryFire()
        {
            _secondaryWeapon.StopFire();
        }

        
    }
}