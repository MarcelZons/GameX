using System.Collections;
using UnityEngine;
using WG.GameX.Common;
using WG.GameX.Managers;
using WG.GameX.Util;

namespace WG.GameX.Player
{
    public class Shield : MonoBehaviour
    {
        private ColorPropertySetter _shieldColorSetter;
        private int _shieldHealth;
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Active = Animator.StringToHash("Active");
        private int _initialHealth;
        private Gradient _colorGradiant;
        [SerializeField] private MeshCollider [] _meshColliders;

        private bool _isShieldActive;
        private void Awake()
        {
            _shieldColorSetter = GetComponentInChildren<ColorPropertySetter>();
            _meshColliders = GetComponentsInChildren<MeshCollider>();
        }

        public void Setup(int initialHealth, Gradient colorGradiant)
        {
            _initialHealth = initialHealth;
            _shieldHealth = initialHealth;
            _colorGradiant = colorGradiant;
            _shieldColorSetter.SetVisibility(1f, _colorGradiant.Evaluate(1f));
            _isShieldActive = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($" {gameObject.name} Was hit by {other.gameObject} with tag {other.tag}");
            if (other.CompareTag("Bullet"))
            {
                ReduceShieldHealth(other.GetComponent<Bullet>().DamageAmount);
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
            DependencyMediator.Instance.UiController.SetInformationText($"{gameObject.name} Down!");
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
            DependencyMediator.Instance.UiController.SetInformationText($"{gameObject.name} Recharged!");
            _shieldHealth = _initialHealth;
            _isShieldActive = true;
        }
    }
}