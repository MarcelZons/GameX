using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WG.GameX.Enemy;
using WG.GameX.Util;

namespace WG.GameX.Player
{
    public class AttackController : MonoBehaviour
    {
        [Header("Primary Weapons ------------------------")] [SerializeField]
        private float _primaryBulletVelocity;

        [SerializeField] private List<Transform> _gunPoints;
        [SerializeField] private GameObject _bulletPrefab;
        private PrimaryWeapon _primaryWeapon;

        [Space(10)] [Header("Secondary Weapons ------------------------")] [SerializeField]
        private Transform _originTransform;

        [SerializeField] private float _secondaryWeapnFireFrequency;
        [SerializeField] private GameObject _beamGlowObject;
        [SerializeField] private GameObject _beamSpawnObject;
        [SerializeField] private GameObject _beamHeatEffect;
        [SerializeField] private PlayerRadar _playerRadar;
        private SecondaryWeapon _secondaryWeapon;
        private float _timeSinceSecondaryWeaponFired;
        private float _secondaryWeaponFillStatus;

        private SecondaryWeaponStatusEvent _secondaryWeaponStatus;

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

        private void Awake()
        {
            _secondaryWeaponStatus = new SecondaryWeaponStatusEvent();
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

            if (_playerRadar.HasEnemyWeakPoints == false && _playerRadar.HasEnemy == false)
            {
                _secondaryWeaponStatus.Invoke("No Enemy in the Radar");
                return;
            }

            if (_playerRadar.HasEnemyWeakPoints)
            {
                var nearestWeakPoint = _playerRadar.GetNearestWeakPoint();
                _secondaryWeapon.FireAtTarget(_originTransform, nearestWeakPoint.transform,
                    nearestWeakPoint.LayerMask);
                Invoke(nameof(StopSecondaryFire), 1f);
            }

            else if (_playerRadar.HasEnemy)
            {
                var nearestEnemy = _playerRadar.GetNearestEnemy();
                _secondaryWeapon.FireAtTarget(_originTransform, nearestEnemy.transform, nearestEnemy.LayerMask);
                Invoke(nameof(StopSecondaryFire), 1f);
            }

            _secondaryWeapon.IsReady = false;
            _timeSinceSecondaryWeaponFired = 0;
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

        public void SelectableFireCommand(List<Transform> _enemyWeakPoints, float hitDuration, LayerMask layerMask)
        {
            StartCoroutine(IterateWeapoints(_enemyWeakPoints, hitDuration, layerMask));
        }

        private IEnumerator IterateWeapoints(List<Transform> _enemyWeakPoints, float hitDuration, LayerMask layerMask)
        {
            foreach (var weakPoint in _enemyWeakPoints)
            {
                _secondaryWeapon.FireAtTarget(_originTransform, weakPoint, layerMask);
                yield return new WaitForSeconds(hitDuration);
                StopSecondaryFire();
            }
        }

        private void StopSecondaryFire()
        {
            _secondaryWeapon.StopFire();
        }
    }
}