using System.Collections;
using UnityEngine;
using WG.GameX.Common;
using WG.GameX.Managers;
using WG.GameX.Util;

namespace WG.GameX.Player
{
    public class Shield : MonoBehaviour
    {
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Active = Animator.StringToHash("Active");
        
        private ColorPropertySetter _shieldColorSetter;
        private int _shieldHealth;
        private int _initialHealth;
        private Gradient _colorGradiant;
        private MeshCollider [] _meshColliders;
        private ShieldCover [] _shieldCovers;
        private ShieldCoverHitEvent[] _shieldCoverHitEvents;
        private bool _isShieldActive;
        private void Awake()
        {
            _shieldColorSetter = GetComponentInChildren<ColorPropertySetter>();
            _meshColliders = GetComponentsInChildren<MeshCollider>();
            _shieldCovers = GetComponentsInChildren<ShieldCover>();
        }

        private void Start()
        {
            _shieldCoverHitEvents = new ShieldCoverHitEvent[_shieldCovers.Length];
            for (int i = 0; i < _shieldCovers.Length; i++)
            {
                _shieldCoverHitEvents[i] = _shieldCovers[i].ShieldCoverTakingHit;
            }

            foreach (var shieldCoverHitEvent in _shieldCoverHitEvents)
            {
                shieldCoverHitEvent.AddListener((bullet) =>
                {
                    ReduceShieldHealth(bullet.DamageAmount);
                });
            }
        }

        public void Setup(int initialHealth, Gradient colorGradiant)
        {
            _initialHealth = initialHealth;
            _shieldHealth = initialHealth;
            _colorGradiant = colorGradiant;
            _shieldColorSetter.SetVisibility(1f, _colorGradiant.Evaluate(1f));
            _isShieldActive = true;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                ReduceShieldHealth(other.gameObject.GetComponent<Bullet>().DamageAmount);
            }
        }


        private void ReduceShieldHealth(int subtractionAmount)
        {
            if (_isShieldActive == false)
            {
                return;
            }
            _shieldHealth -= subtractionAmount;
            if (_shieldHealth > 0)
            {
                var healthFactor = (_shieldHealth * 1.0f / _initialHealth);
                _shieldColorSetter.SetVisibility(healthFactor, _colorGradiant.Evaluate(healthFactor));
            }

            if (_shieldHealth <= 0)
            {
                ShieldDown();
            }
        }

        private void ShieldDown()
        {
            _isShieldActive = false;
            _shieldColorSetter.SetVisibility(0f,_colorGradiant.Evaluate(0f));
            _shieldHealth = 0;
            foreach (var meshCollider in _meshColliders)
            {
                meshCollider.enabled = false;
            }
            DependencyMediator.Instance.UiController.SetInformationText($"{gameObject.name} Down!", MessageType.Negative);
            StartCoroutine(ReplenishShield());
        }

        private IEnumerator ReplenishShield()
        {
            yield return new WaitForSeconds(5f);
            for (var i = 0f; i <= 1; i+= Time.deltaTime/5f)
            {
                _shieldColorSetter.SetVisibility(i, _colorGradiant.Evaluate(i));
                yield return new WaitForEndOfFrame();
            }
            
            foreach (var meshCollider in _meshColliders)
            {
                meshCollider.enabled = true;
            }
            DependencyMediator.Instance.UiController.SetInformationText($"{gameObject.name} Recharged!", MessageType.Positive);
            _shieldHealth = _initialHealth;
            _isShieldActive = true;
        }
    }
}