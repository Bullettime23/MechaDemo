using System.Collections;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mecha
{
    public class UIDamagePanel : Singleton<UIDamagePanel>
    {
        [SerializeField] private Image m_DamagePanel, m_CriticalPanel;
        [SerializeField] private TextMeshProUGUI m_DamageText, m_CriticalText;
        [SerializeField] private float m_DisplayDuration;
        [SerializeField] private float m_OffsetY = 1;

        private Camera m_Camera;

        public void ShowDamage(Vector3 positoin, int damage)
        {
            m_DamageText.text = $"Damage: {damage}";
            m_DamagePanel.GetComponent<RectTransform>().anchoredPosition = m_Camera.WorldToScreenPoint(positoin + new Vector3(0, m_OffsetY));
            ActivateWithDuration(m_DamagePanel.gameObject);
        }

        public void ShowCritical(Vector3 positoin, int damage)
        {
            m_CriticalText.text = $"Critical: {damage}";
            m_CriticalPanel.GetComponent<RectTransform>().anchoredPosition = m_Camera.WorldToScreenPoint(positoin + new Vector3(0, m_OffsetY));

            ActivateWithDuration(m_CriticalPanel.gameObject);
        }

        public void ShowMiss(Vector3 positoin)
        {
            m_DamageText.text = $"Miss!";
            m_DamagePanel.GetComponent<RectTransform>().anchoredPosition = m_Camera.WorldToScreenPoint(positoin + new Vector3(0, m_OffsetY));

            ActivateWithDuration(m_DamagePanel.gameObject);
        }

        private void Start()
        {
            m_DamagePanel.gameObject.SetActive(false);
            m_CriticalPanel.gameObject.SetActive(false);

            m_Camera = Camera.main;
        }

        private void ActivateWithDuration(GameObject go)
        {
            go.SetActive(true);

            IEnumerator DisableTimeout()
            {
                yield return new WaitForSeconds(m_DisplayDuration);
                go.SetActive(false);
            }

            StartCoroutine(DisableTimeout());
        }
    }
}