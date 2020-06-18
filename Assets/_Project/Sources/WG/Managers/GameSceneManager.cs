using System;
using UnityEngine;

namespace WG.GameX.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField] private DependencyMediator _dependency;
        public DependencyMediator DependencyMediator => _dependency;

        [SerializeField] private int _enemyCount;
        
        private void Start()
        {
            _dependency.PlayerShipController.SecondaryWeaponReady.AddListener(
                (message) => _dependency.UiController.SetInformationText(message, MessageType.Neutral)
            );

            _enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
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

        public void ReduceEnemy()
        {
            _enemyCount--;
            if (_enemyCount == 0)
            {
                GameOver(true);
            }
        }
    }
}