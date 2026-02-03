using UnityEngine;
using Infrastructure;
using System.Collections.Generic;

namespace Mecha
{
    /// <summary>
    /// Две команды ходят по очереди, пока не будет выполнено условие победы
    /// В начале совоего хода каждый юнит в команде получает 2 токена действия. Когда юнит гибнет, токены обнуляются
    /// Перемещение снимает один токен. Можно перемещаться один раз за ход.
    /// Атака снимает оба токена. После этого юнит считается походившим: у него становится 0 токенов
    /// </summary>
    public class GameController : Singleton<GameController>
    {
        private GameStateMachine m_StateMachine;
        //private List<Unit> m_Team1Units;
        //private List<Unit> m_Team2Units;
        [SerializeField] private Unit[] m_Team1Units;
        [SerializeField] private Unit[] m_Team2Units;
        private Unit[] m_CurrentTeam;
        private int m_SelectedUnitIndex;

        private void Start()
        {
            m_StateMachine = new GameStateMachine();

            m_CurrentTeam = m_Team1Units;
            m_SelectedUnitIndex = 0;

            // focus camera
        }

        #region Interface Actions
        public Unit SelectedUnit => m_CurrentTeam.Length > 0 ? m_CurrentTeam[m_SelectedUnitIndex] : null;

        public void SelectAction()
        {
            m_StateMachine.ChangeState(new StateUnitActionSelect(m_StateMachine));
        }
        public void Move()
        {
            m_StateMachine.ChangeState(new StateMoveUnit(m_StateMachine));
        }

        public void Skip()
        {
            SelectedUnit.OnSkip();
            // Debug
            NextTeamTurn();
        }

        //Service Methods

        public void SelectNextUnit()
        {
            m_SelectedUnitIndex++;
            if (m_SelectedUnitIndex >= m_CurrentTeam.Length)
                m_SelectedUnitIndex = 0;
            // Move camera to unit
        }

        public void SelectPreviousUnit()
        {
            m_SelectedUnitIndex--;
            if (m_SelectedUnitIndex < 0)
                m_SelectedUnitIndex = m_CurrentTeam.Length - 1;
            // Move camera to unit
        }

        public void NextTeamTurn()
        {
            if (m_CurrentTeam == m_Team1Units)
            {
                m_CurrentTeam = m_Team2Units;
            }
            else
            {
                NextRound();
                m_CurrentTeam = m_Team1Units;
            }

            m_SelectedUnitIndex = 0;
            m_StateMachine.ChangeState(new StateUnitActionSelect(m_StateMachine));

            // Если не осталось врагов, объявить победу
        }

        public void NextRound()
        {
            foreach (Unit unit in m_Team1Units)
                unit.OnTurnStart();

            foreach (Unit unit in m_Team2Units)
                unit.OnTurnStart();
        }


        #endregion
    }

    public class GameStateMachine : StateMachine
    {
        // Знает о текущем состоянии
    }

    public class StateMoveUnit : State
    {
        public StateMoveUnit(StateMachine stateMachine) : base(stateMachine)
        {
            UIActionsPanel.Instance.gameObject.SetActive(false);
            Unit unitToMove = GameController.Instance.SelectedUnit;
            //Активировать отображение поля для хода
            GridBehaviour.Instance.SetStartCoordinatesOfUnit(unitToMove);
            GridBehaviour.Instance.EnableGridForClick();

            //Дождаться, когда игрок выберет клетку для хода
            GridBehaviour.Instance.OnPathChoosen += OnPathCreated;

            //Дождаться, когда юнит дойдет, вернуться к фазе выбора действий

        }

        private void OnPathCreated(List<GameObject> path)
        {
            GridBehaviour.Instance.DisableGridForClick();
            GameController.Instance.SelectedUnit.OnMoveEnd += FinishMovement;
            GameController.Instance.SelectedUnit.MoveByPath(path);
            GridBehaviour.Instance.OnPathChoosen -= OnPathCreated;
        }

        private void FinishMovement()
        {
            GameController.Instance.SelectedUnit.OnMoveEnd -= FinishMovement;
            GameController.Instance.SelectAction();
        }

        // Чтобы щелкнуть правой кнопкой мыши и сбросить передвижение
        private void OnAbortMovement()
        {
            GridBehaviour.Instance.OnPathChoosen -= OnPathCreated;
            GameController.Instance.SelectAction();
        }
    }

    public class StateUnitActionSelect : State
    {
        public StateUnitActionSelect(StateMachine stateMachine) : base(stateMachine)
        {
            UIActionsPanel.Instance.gameObject.SetActive(true);

            Unit unitToMove = GameController.Instance.SelectedUnit;

            if (unitToMove == null)
            {
                GameController.Instance.NextTeamTurn();
            }


            if (unitToMove.IsMovedThisTurn)
            {
                UIActionsPanel.Instance.DisableMoveButton();
            }
            else
            {
                UIActionsPanel.Instance.EnableMoveButton();
            }
        }
    }

    /// <summary>
    /// In the turn end
    /// </summary>
    public class StateMapEvents : State
    {
        public StateMapEvents(StateMachine stateMachine) : base(stateMachine) { }

        // Нанести урон горящим зданиям, например
    }
}