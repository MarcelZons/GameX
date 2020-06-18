using UnityEngine;
using UnityEngine.UI;

namespace WG.GameX.Ui
{
    public class FilledBar : MonoBehaviour
    {
        [SerializeField] private Transform _barsParent;
        [SerializeField] private Image _emptyPowerLogo;
        [SerializeField] private Image _filledPowerLogo;
        private Image[] _bars;

        private void Awake()
        {
            _bars = new Image[_barsParent.childCount];
            for (int i = 0; i < _barsParent.childCount; i++)
            {
                _bars[i] = _barsParent.GetChild(i).GetComponent<Image>();
            }
        }

        public void UpdateValue(float fillAmount)
        {
            _filledPowerLogo.enabled = (fillAmount == 1);
            _emptyPowerLogo.enabled = (fillAmount < 1);
            
            var index = (int) (fillAmount * _bars.Length);
            for (int i = 0; i < index; i++)
            {
                _bars[i].enabled = true;
            }

            for (int i = index; i < _bars.Length; i++)
            {
                _bars[i].enabled = false;
            }
        }
    }
}