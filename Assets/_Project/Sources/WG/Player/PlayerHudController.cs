using UnityEngine;

namespace WG.GameX.Ui
{
    public class PlayerHudController : MonoBehaviour
    {
        [SerializeField] private CenterImageController _centerImageController;
        [SerializeField] private FilledBar _energyFillbar;
        [SerializeField] private FilledBar _speedFillBar;
        
        public void SetHealth(float health)
        {
            _centerImageController.UpdateValue(health);
        }

        public void SetSpeed(float value)
        {
            _speedFillBar.UpdateValue(value);
        }

        public void SetEnergyBar(float value)
        {
            _energyFillbar.UpdateValue(value);
        }
        
    }
}