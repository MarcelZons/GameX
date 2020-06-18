using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WG.GameX.Player
{
    public class EnemyShipUiController : MonoBehaviour
    {
        [Header("Top Ui Images")]
        [SerializeField] private Image _topUiHealthBarImageFg;
        [SerializeField] private Image _topUiHealthBarImageBg;
        [SerializeField] private TextMeshProUGUI _shipNameText;
        private EnemyShipUiTargeter _enemyShipUiTargeter;

        private void Awake()
        {
            _enemyShipUiTargeter = GetComponentInChildren<EnemyShipUiTargeter>();
            _enemyShipUiTargeter.SetEnemyName(gameObject.name);
        }

        private void Start()
        {
            _enemyShipUiTargeter.TopUiDisplayEvent.AddListener(ShowTopUi);   
        }

        private void ShowTopUi(bool state)
        {
            _topUiHealthBarImageBg.enabled = state;
            _topUiHealthBarImageFg.enabled = state;
            _shipNameText.enabled = state;
        }

        public void SetHealthBarValue(float health)
        {
            _topUiHealthBarImageFg.fillAmount = health;
        }
    }
}