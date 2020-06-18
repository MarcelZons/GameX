using UnityEngine;
using UnityEngine.Serialization;
using WG.GameX.Enemy;

namespace WG.GameX.Player
{
    public class BigEnemyHealth : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float _health;

        [Header("Number of milliseconds the beam needs to hit the weak point")]
        [Range(.01f, 100)]
        [SerializeField] private float _initialWeakPointHealth;
            
        private EnemyShipUiController _uiController;
        private EnemyShipUiTargeter _targeterUiController;

        private EnemyWeakPoint[] _enemyWeakPoints;

        private float _maxHealtValue;
        private void Awake()
        {
             _uiController = GetComponent<EnemyShipUiController>();
            _targeterUiController = GetComponentInChildren<EnemyShipUiTargeter>();
            _enemyWeakPoints = GetComponentsInChildren<EnemyWeakPoint>();
            foreach (var enemyWeakPoint in _enemyWeakPoints)
            {
                enemyWeakPoint.WeakPointHealth = _initialWeakPointHealth;
            }

            _maxHealtValue = _enemyWeakPoints.Length * _initialWeakPointHealth;
        }

        private void Update()
        {
            var currentHealth = 0f;
            foreach (var enemyWeakPoint in _enemyWeakPoints)
            {
                currentHealth += enemyWeakPoint.WeakPointHealth;
            }

            _health = currentHealth / _maxHealtValue;
            
            _uiController.SetHealthBarValue(_health);
            _targeterUiController.SetHealthBarValue(_health);
        }
    }
}