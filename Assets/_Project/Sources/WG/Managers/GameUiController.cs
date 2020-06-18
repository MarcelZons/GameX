using KoganeUnityLib;
using TMPro;
using UnityEngine;

namespace WG.GameX.Managers
{
    public class GameUiController : MonoBehaviour
    {
        [SerializeField] private MessageBoxController _messageBoxController;
        public void SetSpeedHud(float value)
        {
            DependencyMediator.Instance.PlayerHudController.SetSpeed(value);
        }

        public void SetEnergyHud(float value)
        {
            DependencyMediator.Instance.PlayerHudController.SetEnergyBar(value);
        }

        public void SetInformationText(string info, MessageType messageType)
        {
            _messageBoxController.SetMessage(info, messageType);
        }

        public void SetHealth(float healthFactor)
        {
            DependencyMediator.Instance.PlayerHudController.SetHealth(healthFactor);
        }
    }
}