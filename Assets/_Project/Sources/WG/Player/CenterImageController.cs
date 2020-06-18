using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WG.GameX.Ui
{
    public class CenterImageController : MonoBehaviour
    {
        [SerializeField] private Image _green, _orange, _red;
        [SerializeField] private TextMeshProUGUI _healthText;

        public void UpdateValue(float health)
        {
            if (health < .03f)
            {
                _red.enabled = true;
                _green.enabled = false;
                _orange.enabled = false;
            }else if (health >= .03f && health <= .5f)
            {
                _red.enabled = false;
                _green.enabled = false;
                _orange.enabled = true;
            }
            else if(health >=.5f)
            {
                _red.enabled = false;
                _green.enabled = true;
                _orange.enabled = false;
            }

            _healthText.text = $"{((int) (health * 100))}%";
        }
    }
}