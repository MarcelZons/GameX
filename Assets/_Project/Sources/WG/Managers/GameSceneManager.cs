using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WG.GameX.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField] private int _enemyCount;

        private bool _isGameOver;
        
        private void Start()
        {
            _isGameOver = false;
            DependencyMediator.Instance.PlayerShipController.SecondaryWeaponReady.AddListener(
                (message) => DependencyMediator.Instance.UiController.SetInformationText(message, MessageType.Neutral)
            );

            _enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }

        private void Update()
        {
            UiUpdate();
        }

        private void UiUpdate()
        {
            var speedFactor = DependencyMediator.Instance.PlayerShipController.SpeedFactor;
            DependencyMediator.Instance.UiController.SetSpeedHud(speedFactor);

            var secondaryWeaponsReadiness = DependencyMediator.Instance.PlayerShipController.SecondaryWeaponFilledStatus;
            DependencyMediator.Instance.UiController.SetEnergyHud(secondaryWeaponsReadiness);
        }

        public void GameOver(bool gameOwn)
        {
            if (_isGameOver == false)
            {
                _isGameOver = true;
                DependencyMediator.Instance.UiController.ShowGameOver(gameOwn);
                StartCoroutine(LoadMenuScene(gameOwn));
                LoadMenuScene(gameOwn);
            }
        }

        private IEnumerator LoadMenuScene(bool gameOwn)
        {
            yield return new WaitForSeconds(5);
            if (gameOwn)
            {
                SceneManager.LoadScene("MenuSceneWin");
            }
            else
            {
                SceneManager.LoadScene("MenuSceneDefeated");
            }
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