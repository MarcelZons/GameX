using KoganeUnityLib;
using TMPro;
using UnityEngine;

namespace WG.GameX.Managers
{
    public class GameUiController : MonoBehaviour
    {
        [SerializeField] private PlayerHud _speedHud;
        [SerializeField] private PlayerHud _energyHud;
        [SerializeField] private TextMeshProUGUI _informationText;
        private TMP_Typewriter _inforamtionTextTypewriter;

        private void Awake()
        {
            _inforamtionTextTypewriter = _informationText.gameObject.GetComponent<TMP_Typewriter>();
        }

        public void SetSpeedHud(float value)
        {
            DependencyMediator.Instance.PlayerHudController.SetSpeed(value);
        }

        public void SetEnergyHud(float value)
        {
            DependencyMediator.Instance.PlayerHudController.SetEnergyBar(value);
        }

        public void SetInformationText(string info)
        {
            CancelInvoke(nameof(HideInformationText));
            _inforamtionTextTypewriter.Play(info, 50, null);
            Invoke(nameof(HideInformationText), 3f);
        }

        private void HideInformationText()
        {
            _informationText.text = "";
        }

        public void SetHealth(float healthFactor)
        {
            DependencyMediator.Instance.PlayerHudController.SetHealth(healthFactor);
        }
    }
}