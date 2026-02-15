using Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mecha
{
    public class LevelController : Singleton<LevelController>
    {

        private const string MainMenu = "MainMenu";
        private const string Levels = "Levels";
        private const string Options = "Options";

        private string m_CurrentLevel = MainMenu;
        private int m_LevelIndex = 1;
        public void LoadMenu()
        {
            SceneManager.LoadScene(MainMenu);
        }

        public void LevelSelect()
        {
            SceneManager.LoadScene(Levels);
        }

        public void LoadOptions()
        {
            SceneManager.LoadScene(Options);
        }

        public void LoadLevel(int number)
        {
            SceneManager.LoadScene($"Level_{number}");
            m_CurrentLevel = $"Level_{number}";
            m_LevelIndex = number;
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(m_CurrentLevel);
        }
        public void NextLevel()
        {
            SceneManager.LoadScene($"Level_{m_LevelIndex + 1}");
            m_CurrentLevel = $"Level_{m_LevelIndex + 1}";
            m_LevelIndex += 1;
        }

        public void Quit()
        {
            Application.Quit();
        }

    }
}