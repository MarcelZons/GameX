using System;
using KoganeUnityLib;
using TMPro;
using UnityEngine;

namespace WG.GameX.Managers
{
    public class GameUiController : MonoBehaviour
    {
        [SerializeField] private MessageBoxController _messageBoxController;
        [SerializeField] private Animator _defatedAnimator;
        [SerializeField] private Animator _victoryAnimator;
        private static readonly int Appear = Animator.StringToHash("Appear");
        private bool _gameOverShowing;

        private void Awake()
        {
            _gameOverShowing = false;
        }

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
            if(!_gameOverShowing)
                _messageBoxController.SetMessage(info, messageType);
        }

        public void SetHealth(float healthFactor)
        {
            DependencyMediator.Instance.PlayerHudController.SetHealth(healthFactor);
        }

        public void ShowGameOver(bool hasWon)
        {
            if (hasWon)
            {
                _victoryAnimator.SetBool(Appear, true);
            }
            else
            {
                _defatedAnimator.SetBool(Appear, true);
            }
            
            _gameOverShowing = true;
        }
    }
}