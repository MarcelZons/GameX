using System;
using UnityEngine;

namespace WG.GameX.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField] private DependencyMediator _dependency;
        public DependencyMediator DependencyMediator => _dependency;
        private void Start()
        {
            _dependency.PlayerShipController.SecondaryWeaponReady.AddListener(
                (message) => _dependency.UiController.SetInformationText(message)
            );
        }

        private void Update()
        {
            UiUpdate();
        }

        private void UiUpdate()
        {
            var speedFactor = _dependency.PlayerShipController.SpeedFactor;
            _dependency.UiController.SetSpeedHud(speedFactor);

            var secondaryWeaponsReadiness = _dependency.PlayerShipController.SecondaryWeaponFilledStatus;
            _dependency.UiController.SetEnergyHud(secondaryWeaponsReadiness);
        }

        public void GameOver(bool gameOwn)
        {
            Debug.Log($"Player won {gameOwn}");
        }
    }
}