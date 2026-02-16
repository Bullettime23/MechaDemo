using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mecha
{
    public class UIStatusPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform m_HealthContainer;
        [SerializeField] private Image m_CoverIndicator;
        [SerializeField] private Image m_HealthIndicatorPrefab;
        [SerializeField] private TextMeshProUGUI m_MoveTokens;
        [SerializeField] private TextMeshProUGUI m_AttackTokens;

        [SerializeField] private Unit m_Unit;

        [Header("Assets")]
        [SerializeField] private Sprite m_HeathbarFull;
        [SerializeField] private Sprite m_HeathbarEmpty;
        [SerializeField] private Sprite m_CoverFull;
        [SerializeField] private Sprite m_CoverHalf;

        private Camera m_Camera;
        private Image[] m_HealthIndicators;

        private void Start()
        {
            m_Camera = Camera.main;

            // Init health
            m_HealthIndicators = new Image[m_Unit.InitialHitPoints];

            for (int i = 0; i < m_Unit.InitialHitPoints; i++)
            {
                m_HealthIndicators[i] = Instantiate(m_HealthIndicatorPrefab, m_HealthContainer);
                m_HealthIndicators[i].sprite = m_HeathbarFull;
            }


            m_Unit.OnStatusChange += UpdateInterface;
            m_Unit.EventOnDeath.AddListener(OnDestroy);
            GameController.OnTeamTurnChange += ChangeInterfaceColor;
            m_EnemyColor = GameController.Instance.EnemyColor;

            UpdateInterface();
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - m_Camera.transform.position);
        }

        private void OnDestroy()
        {
            m_Unit.OnStatusChange -= UpdateInterface;
            m_Unit.EventOnDeath.RemoveListener(OnDestroy);
        }

        private void UpdateInterface()
        {
            UpdateTokensText();
            UpdateHealth();
            UpdateCover();
        }

        private void UpdateTokensText()
        {
            m_MoveTokens.text = m_Unit.MoveTokens.ToString();
            m_AttackTokens.text = m_Unit.AttackTokens.ToString();
        }

        private void UpdateHealth()
        {
            for (int i = m_Unit.InitialHitPoints - 1; i > -1 && i > m_Unit.CurrentHitPoints - 1; i--)
            {
                m_HealthIndicators[i].sprite = m_HeathbarEmpty;
                if (m_Unit.TeamNumber != GameController.Instance.TeamIndex)
                {
                    m_HealthIndicators[i].color = m_EnemyColor;
                }
            }
        }

        /// <summary>
        /// Если укрытие есть, показать максимальное
        /// </summary>
        private void UpdateCover()
        {
            if (m_Unit.CurrentGrid != null && m_Unit.CurrentGrid.Covers.Count > 0)
            {
                Cover maxCover = null;
                for (int i = 1; i < 5; i++)
                {
                    Cover cover;
                    m_Unit.CurrentGrid.Covers.TryGetValue((CoverDirection)i, out cover);
                    if (cover != null && cover.Type == CoverType.FullCover)
                    {
                        maxCover = cover;
                        break;
                    }

                    // Что делать с полу-укрытием
                    if (cover != null)
                    {
                        maxCover = cover;
                    }

                }

                m_CoverIndicator.sprite = maxCover.Type == CoverType.FullCover ? m_CoverFull : m_CoverHalf;
                return;
            }

            m_CoverIndicator.color = GridBehaviour.Instance.DefaultColor;
        }

        #region ColorSwitch
        private Color m_EnemyColor;

        private void ChangeInterfaceColor(int teamIndex)
        {
            if (m_Unit.TeamNumber != teamIndex)
            {
                foreach (Image image in GetComponentsInChildren<Image>())
                {
                    image.color = new Color(m_EnemyColor.r, m_EnemyColor.g, m_EnemyColor.b, image.color.a);
                }

                m_MoveTokens.color = m_EnemyColor;
                m_AttackTokens.color = m_EnemyColor;
            }
            else
            {
                foreach (Image image in GetComponentsInChildren<Image>())
                {
                    image.color = new Color(1f, 1f, 1f, image.color.a);
                }

                m_MoveTokens.color = Color.white;
                m_AttackTokens.color = Color.white;
            }
        }

        #endregion
    }
}