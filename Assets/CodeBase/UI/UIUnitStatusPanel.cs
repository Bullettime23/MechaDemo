using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mecha
{
    /// <summary>
    /// Отображает количество здоровья и токенов действия юнита
    /// Панель отображается над юнитом, исчезает во время действий
    /// </summary>
    public class UIUnitStatusPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Panel;
        [SerializeField] private RectTransform m_HealthContainer;
        [SerializeField] private Image m_CoverIndicator;
        [SerializeField] private Image m_HealthIndicatorPrefab;
        [SerializeField] private TextMeshProUGUI m_MoveTokens;
        [SerializeField] private TextMeshProUGUI m_AttackTokens;
        [SerializeField] private float m_OffsetUp = 9;

        [Header("Assets")]
        [SerializeField] private Sprite m_HeathbarFull;
        [SerializeField] private Sprite m_HeathbarEmpty;
        [SerializeField] private Sprite m_CoverFull;
        [SerializeField] private Sprite m_CoverHalf;
        [SerializeField] private float m_UpdateRate = 0.06f;

        private float m_UpdateTimer;
        private Camera m_Camera;
        public Unit AttachedUnit;
        private Image[] m_HealthIndicators;

        public void SetUnit(Unit unit)
        {
            AttachedUnit = unit;

            AttachedUnit.OnStatusChange += UpdateInterface;
            AttachedUnit.EventOnDeath.AddListener(OnUnitDestroy);

            UpdateInterface();
        }

        #region Unity Events

        private void Start()
        {
            m_Camera = Camera.main;
            m_UpdateTimer = m_UpdateRate;

            // Init health
            m_HealthIndicators = new Image[AttachedUnit.InitialHitPoints];

            for (int i = 0; i < AttachedUnit.InitialHitPoints; i++)
            {
                m_HealthIndicators[i] = Instantiate(m_HealthIndicatorPrefab, m_HealthContainer);
                m_HealthIndicators[i].sprite = m_HeathbarFull;
            }

            UpdateInterface();
        }

        private void Update()
        {
            if (AttachedUnit == null) return;
            m_UpdateTimer -= Time.deltaTime;
            if (m_UpdateTimer <= 0)
            {
                m_Panel.anchoredPosition = m_Camera.WorldToScreenPoint(AttachedUnit.transform.position + Vector3.up * m_OffsetUp);
                m_UpdateTimer = m_UpdateRate;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeAll();
        }
        #endregion

        #region Subscriptions management
        private void OnUnitDestroy(DestructibleBase destructible)
        {
            UnsubscribeAll();
            InGameHUDController.Instance.RemovePanelOfList(this);
            Destroy(gameObject);
        }

        private void UnsubscribeAll()
        {
            AttachedUnit.OnStatusChange -= UpdateInterface;
            AttachedUnit.EventOnDeath.RemoveListener(OnUnitDestroy);
        }

        #endregion

        private void UpdateInterface()
        {
            UpdateTokensText();
            UpdateHealth();
            UpdateCover();
        }

        private void UpdateTokensText()
        {
            m_MoveTokens.text = AttachedUnit.MoveTokens.ToString();
            m_AttackTokens.text = AttachedUnit.AttackTokens.ToString();
        }

        private void UpdateHealth()
        {
            for (int i = AttachedUnit.InitialHitPoints - 1; i > -1 && i > AttachedUnit.CurrentHitPoints - 1; i--)
            {
                m_HealthIndicators[i].sprite = m_HeathbarEmpty;
                if (AttachedUnit.TeamNumber != GameController.Instance.TeamIndex)
                {
                    m_HealthIndicators[i].color = m_Color;
                }
            }
        }

        /// <summary>
        /// Если укрытие есть, показать максимальное
        /// </summary>
        private void UpdateCover()
        {
            if (AttachedUnit.CoverTook != null)
            {
                m_CoverIndicator.sprite = AttachedUnit.CoverTook.Type == CoverType.FullCover ? m_CoverFull : m_CoverHalf;
                m_CoverIndicator.color = m_Color;

                // Иконка прозрачная?
                return;
            }

            m_CoverIndicator.color = m_Color;
        }

        #region ColorSwitch
        //private Color m_EnemyColor;
        private Color m_Color;

        public void SetColor(Color color)
        {
            m_Color = color;
            foreach (Image image in GetComponentsInChildren<Image>())
            {
                image.color = new Color(color.r, color.g, color.b, image.color.a);
            }

            m_MoveTokens.color = color;
            m_AttackTokens.color = color;
            m_CoverIndicator.color = m_Color;
        }


        #endregion
    }
}