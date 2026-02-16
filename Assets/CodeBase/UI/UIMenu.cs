using UnityEngine;

namespace Mecha
{
    public class UIMenu : MonoBehaviour
    {
        public void LoadMenu()
        {
            LevelController.Instance.LoadMenu();
        }

        public void LevelSelect()
        {
            LevelController.Instance.LevelSelect();
        }

        public void LoadOptions()
        {
            LevelController.Instance.LoadOptions();
        }


        public void RestartLevel()
        {
            LevelController.Instance.RestartLevel();
        }

        public void NextLevel()
        {
            LevelController.Instance.NextLevel();
        }

        public void Quit()
        {
            LevelController.Instance.Quit();
        }
    }
}