using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WG.GameX.MenuScene
{
    public enum DifficultyLevelSelection
    {
        Easy, Normal, Hard
    }
    public class MenuSceneManager: MonoBehaviour
    {
        private DifficultyLevelSelection _difficultyLevel = DifficultyLevelSelection.Easy;
        [SerializeField] private Button _easy, _normal, _hard;
        [SerializeField] private Button _startButton;
        private void Start()
        {
            _easy.onClick.AddListener(() => { _difficultyLevel = DifficultyLevelSelection.Easy; });
            _normal.onClick.AddListener(() => { _difficultyLevel = DifficultyLevelSelection.Normal; });
            _hard.onClick.AddListener(() => { _difficultyLevel = DifficultyLevelSelection.Hard; });
            
            _startButton.onClick.AddListener(() =>
            {
                Debug.Log($"Current level: {_difficultyLevel}");
                SceneManager.LoadScene("GameScene");
            });
        }

    }
}