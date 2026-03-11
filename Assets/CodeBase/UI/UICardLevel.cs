using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mecha
{
    public class UICardLevel : MonoBehaviour
    {
        [SerializeField] private LevelProperties m_Props;
        [SerializeField] private Image m_Preview;
        [SerializeField] private TextMeshProUGUI m_Header;

        private void Start()
        {
            if (m_Props != null)
            {
                m_Preview.sprite = m_Props.preview;
                m_Header.text = m_Props.header;
            }
        }

        public void LoadLevel()
        {
            LevelController.LoadLevel(m_Props.levelNumber);
        }
    }
}