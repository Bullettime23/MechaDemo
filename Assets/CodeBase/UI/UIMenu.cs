using UnityEngine;

namespace Mecha
{
    public class UIMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform m_OptionsScreen;
        [SerializeField] private RectTransform m_MenuScreen;
        public void LoadMenu()
        {
            LevelController.LoadMenu();
        }

        public void LevelSelect()
        {
            LevelController.LevelSelect();
        }

        public void LoadOptions()
        {
            LevelController.LoadOptions();
        }

        public void DisplayOptions()
        {
            m_MenuScreen.gameObject.SetActive(false);
            m_OptionsScreen.gameObject.SetActive(true);
        }

        public void DisplayMainMenu()
        {
            m_OptionsScreen.gameObject.SetActive(false);
            m_MenuScreen.gameObject.SetActive(true);
        }


        public void RestartLevel()
        {
            LevelController.RestartLevel();
        }

        public void NextLevel()
        {
            LevelController.NextLevel();
        }

        public void Quit()
        {
            LevelController.Quit();
        }
    }
}