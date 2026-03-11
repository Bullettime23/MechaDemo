using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mecha
{
    public static class LevelController
    {
        private const string MainMenu = "MainMenu";
        private const string Levels = "Levels";
        private const string Options = "Options";

        private static string m_CurrentLevel = MainMenu;
        private static int m_LevelIndex = 1;
        public static void LoadMenu()
        {
            SceneManager.LoadScene(MainMenu);
        }

        public static void LevelSelect()
        {
            SceneManager.LoadScene(Levels);
        }

        public static void LoadOptions()
        {
            SceneManager.LoadScene(Options);
        }

        public static void LoadLevel(int number)
        {
            SceneManager.LoadScene($"Level_{number}");
            m_CurrentLevel = $"Level_{number}";
            m_LevelIndex = number;
        }

        public static void RestartLevel()
        {
            SceneManager.LoadScene(m_CurrentLevel);
        }
        public static void NextLevel()
        {
            SceneManager.LoadScene($"Level_{m_LevelIndex + 1}");
            m_CurrentLevel = $"Level_{m_LevelIndex + 1}";
            m_LevelIndex += 1;
        }

        public static void Quit()
        {
            Application.Quit();
        }

    }
}