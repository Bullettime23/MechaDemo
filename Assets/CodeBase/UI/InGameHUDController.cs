using Infrastructure;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Mecha
{
    public class InGameHUDController : Singleton<InGameHUDController>
    {
        [SerializeField] private Color m_EnemyColor;
        [SerializeField] private Color m_FriendlyColor;
        [SerializeField] private UIUnitStatusPanel m_StatusPanelPrefab;


        private List<UIUnitStatusPanel> m_StatusPanels;


        public void DisableInGameInterface()
        {
            UIActionsPanel.Instance.gameObject.SetActive(false);

            foreach (UIUnitStatusPanel panel in m_StatusPanels)
            {
                panel.gameObject.SetActive(false);
            }
        }

        public void CreateStatusPanel(Unit unit)
        {

            UIUnitStatusPanel panel = Instantiate(m_StatusPanelPrefab);
            panel.SetUnit(unit);

            m_StatusPanels.Add(panel);
        }

        public void RemovePanelOfList(UIUnitStatusPanel panel)
        {
            m_StatusPanels.Remove(panel);
        }

        #region Unity Actions
        private void Start()
        {
            m_StatusPanels = new List<UIUnitStatusPanel>();
            GameController.OnTeamTurnChange += ChangePanelsColor;

            IEnumerator UpdateColorTimeout()
            {
                yield return new WaitForSeconds(1);
                ChangePanelsColor(GameController.Instance.TeamIndex);
            }

            StartCoroutine(UpdateColorTimeout());
        }

        private void OnDestroy()
        {
            GameController.OnTeamTurnChange -= ChangePanelsColor;
        }

        #endregion

        private void ChangePanelsColor(int currentTeamIndex)
        {
            foreach (UIUnitStatusPanel panel in m_StatusPanels)
            {
                if (panel.AttachedUnit.TeamNumber != currentTeamIndex)
                {
                    panel.SetColor(m_EnemyColor);
                }
                else
                {
                    panel.SetColor(m_FriendlyColor);
                }
            }
        }
    }
}