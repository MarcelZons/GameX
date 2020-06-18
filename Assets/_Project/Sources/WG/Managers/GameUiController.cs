using KoganeUnityLib;
using TMPro;
using UnityEngine;

namespace WG.GameX.Managers
{
    public class GameUiController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _informationText;
        [SerializeField] private Animator _messageBoxAnimator;
        private TMP_Typewriter _inforamtionTextTypewriter;
        private static readonly int Appear = Animator.StringToHash("Appear");

        private void Awake()
        {
            _inforamtionTextTypewriter = _informationText.gameObject.GetComponent<TMP_Typewriter>();
            _informationText.text = "";
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
            _messageBoxAnimator.SetBool(Appear,true);
            _inforamtionTextTypewriter.Play(info.ToUpper(), 50, null);
            Invoke(nameof(HideInformationText), 3f);
        }

        private void HideInformationText()
        {
            _informationText.text = "";
            _messageBoxAnimator.SetBool(Appear,false);
        }

        public void SetHealth(float healthFactor)
        {
            DependencyMediator.Instance.PlayerHudController.SetHealth(healthFactor);
        }
    }
}