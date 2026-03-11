using Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace Mecha
{
    public class UIActionsPanel : Singleton<UIActionsPanel>
    {
        #region Public API

        [SerializeField] private Button m_MoveButton;
        [SerializeField] private Button m_AttackButton;

        public void PreviousUnit()
        {
            print("Previous unit");
            GameController.Instance.SelectPreviousUnit();
        }

        public void NextUnit()
        {
            print("Next unit");
            GameController.Instance.SelectNextUnit();
        }

        public void Attack()
        {
            GameController.Instance.Attack();
        }

        public void Move()
        {
            GameController.Instance.Move();
        }

        public void Skip()
        {
            GameController.Instance.Skip();
        }

        public void Overwatch()
        {
            print("Overwatch");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

        }
        #endregion

        #region ButtonsControls
        public void DisableMoveButton()
        {
            m_MoveButton.interactable = false;
        }

        public void EnableMoveButton()
        {
            m_MoveButton.interactable = true;
        }
        public void DisableAttackButton()
        {
            m_AttackButton.interactable = false;
        }

        public void EnableAttackButton()
        {
            m_AttackButton.interactable = true;
        }
        #endregion
    }
}