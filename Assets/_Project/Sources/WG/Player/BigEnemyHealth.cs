using UnityEngine;

namespace WG.GameX.Player
{
    public class BigEnemyHealth : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float _health;

        private EnemyShipUiController _uiController;
        private EnemyShipUiTargeter _targeterUiController;

        private void Awake()
        {
             _uiController = GetComponent<EnemyShipUiController>();
            _targeterUiController = GetComponentInChildren<EnemyShipUiTargeter>();
        }

        private void Update()
        {
            _uiController.SetHealthBarValue(_health);
            _targeterUiController.SetHealthBarValue(_health);
        }
    }
}