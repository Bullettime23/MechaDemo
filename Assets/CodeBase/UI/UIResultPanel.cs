using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mehca {
    public class UIResultPanel : Singleton<UIResultPanel>
    {
        [SerializeField] private TextMeshProUGUI m_HeaderText;
        [SerializeField] private Image m_Panel;
        void Start()
        {
            m_Panel.gameObject.SetActive(false);
        }

        public void Victory()
        {
            m_HeaderText.text = "Victory";
            m_Panel.gameObject.SetActive(true);
        }

        public void Defeat()
        {
            m_HeaderText.text = "Defeat";
            m_Panel.gameObject.SetActive(true);
            // Restart level ?
        }
    }
}