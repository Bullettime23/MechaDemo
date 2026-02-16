using UnityEngine;
using Infrastructure;
using System.Collections.Generic;
using Common;
using System;
using Mehca;

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
        public static Action<int> OnTeamTurnChange;
        private GameStateMachine m_StateMachine;
        [SerializeField] private Unit[] m_Team1Units, m_Team2Units;
        [SerializeField] private Color m_EnemyTeamColor;
        private Unit[] m_CurrentTeam;
        private int m_SelectedUnitIndex;
        private int m_CurrentTeamIndex;
        public int TeamIndex => m_CurrentTeamIndex;

        public Color EnemyColor => m_EnemyTeamColor;

        private void Start()
        {
            m_StateMachine = new GameStateMachine();

            m_CurrentTeam = m_Team1Units;
            m_CurrentTeamIndex = 1;
            OnTeamTurnChange?.Invoke(m_CurrentTeamIndex);
            m_SelectedUnitIndex = 0;
            ResetUnitsActionPoints();

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
            SelectedUnit.SetActionTokensToZero();
            UnitActionsEnd();
        }

        public void UnitActionsEnd()
        {
            if (FinishGameIfNoUnitsInAnyTeam())
            {
                m_StateMachine.ChangeState(new StateGameEnd(m_StateMachine));
                return;
            }
            if (SelectNextUnit())
            {
                return;
            }
            NextTeamTurn();
        }

        //Service Methods

        //Когда у текущего юнита заканчиваются токены действия,
        //проверить, есть ли у других юнитов в этой команде ещё токены. Если да, переключиться на этот юнит
        //если нет, передать ход следующей команде

        /// <summary>
        /// Выбрать следующий юнит с очками действия
        /// </summary>
        public bool SelectNextUnit()
        {
            int unitIndexWithActionPoints = -1;
            // Ищем справа
            for (int i = m_SelectedUnitIndex + 1; i < m_CurrentTeam.Length; i++)
            {
                if (m_CurrentTeam[i].HasActionTokens)
                {
                    unitIndexWithActionPoints = i;
                    break;
                }
            }
            if (unitIndexWithActionPoints > -1)
            {
                m_SelectedUnitIndex = unitIndexWithActionPoints;
                SelectAction();
                return true;
            }
            //ишем слева направо
            for (int i = 0; i < m_SelectedUnitIndex; i++)
            {
                if (m_CurrentTeam[i].HasActionTokens)
                {
                    unitIndexWithActionPoints = i;
                    break;
                }
            }

            if (unitIndexWithActionPoints > -1)
            {
                m_SelectedUnitIndex = unitIndexWithActionPoints;
                SelectAction();
                return true;
            }

            return false;
            // Move camera to unit
        }

        public void SelectPreviousUnit()
        {
            int unitIndexWithActionPoints = -1;
            //ишем слева
            for (int i = m_SelectedUnitIndex - 1; i >= 0; i--)
            {
                if (m_CurrentTeam[i].HasActionTokens)
                {
                    unitIndexWithActionPoints = i;
                    break;
                }
            }
            if (unitIndexWithActionPoints > -1)
            {
                m_SelectedUnitIndex = unitIndexWithActionPoints;
                SelectAction();
                return;
            }
            // Ищем справа налево
            for (int i = m_CurrentTeam.Length - 1; i > m_SelectedUnitIndex; i--)
            {
                if (m_CurrentTeam[i].HasActionTokens)
                {
                    unitIndexWithActionPoints = i;
                    break;
                }
            }
            if (unitIndexWithActionPoints > -1)
            {
                m_SelectedUnitIndex = unitIndexWithActionPoints;
                SelectAction();
                return;
            }
        }

        public void NextTeamTurn()
        {
            if (m_CurrentTeam == m_Team1Units)
            {
                m_CurrentTeam = m_Team2Units;
                m_CurrentTeamIndex = 2;
                OnTeamTurnChange?.Invoke(m_CurrentTeamIndex);

            }
            else
            {
                ResetUnitsActionPoints();
                m_CurrentTeam = m_Team1Units;
                m_CurrentTeamIndex = 1;
                OnTeamTurnChange?.Invoke(m_CurrentTeamIndex);
            }

            m_SelectedUnitIndex = 0;
            m_StateMachine.ChangeState(new StateUnitActionSelect(m_StateMachine));

            // Если не осталось врагов, объявить победу
        }

        public void ResetUnitsActionPoints()
        {
            foreach (Unit unit in m_Team1Units)
                unit.OnTurnStart();

            foreach (Unit unit in m_Team2Units)
                unit.OnTurnStart();
        }

        public void Attack()
        {
            m_StateMachine.ChangeState(new StateAttack(m_StateMachine));
        }

        public bool FinishGameIfNoUnitsInAnyTeam()
        {
            bool isVictory = !CheckIfUnitsLeft(m_Team2Units);
            bool isDefeat = !CheckIfUnitsLeft(m_Team1Units);

            if (isVictory)
            {
                UIResultPanel.Instance.Victory();
                return true;
            }

            if (isDefeat)
            {
                UIResultPanel.Instance.Defeat();
                return true;
            }
            return false;
        }


        #endregion

        private bool CheckIfUnitsLeft(Unit[] team)
        {
            bool isUnitsAlive = false;
            foreach (Unit unit in team)
            {
                if (unit != null)
                {
                    isUnitsAlive = true;
                    break;
                }
            }
            return isUnitsAlive;
        }
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
            GridBehaviour.Instance.SelectPathEnd();

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
            Unit unitToMove = GameController.Instance.SelectedUnit;
            CameraMovement.Instance.FocusOn(unitToMove.gameObject);

            UIActionsPanel.Instance.gameObject.SetActive(true);

            if (unitToMove == null)
            {
                GameController.Instance.NextTeamTurn();
            }


            if (unitToMove.MoveTokens == 0)
            {
                UIActionsPanel.Instance.DisableMoveButton();
            }
            else
            {
                UIActionsPanel.Instance.EnableMoveButton();
            }
        }
    }

    public class StateAttack : State
    {
        public StateAttack(StateMachine stateMachine) : base(stateMachine)
        {
            UIActionsPanel.Instance.gameObject.SetActive(false);
            GridBehaviour.Instance.SelectTarget();

            GridBehaviour.Instance.OnTargetChoosen += OnObjectChoosen;
        }

        private void OnObjectChoosen(GameObject clickedTarget)
        {
            if (clickedTarget == null)
            {
                GridBehaviour.Instance.OnTargetChoosen -= OnObjectChoosen;
                GameController.Instance.SelectAction();
                return;
            }
            Unit attacker = GameController.Instance.SelectedUnit;
            DestructibleBase firstHitTarget = AttackSystem.Instance.GetFirstObjectOfTrajectory(attacker, clickedTarget);
            AttackSystem.Instance.TryAttackSelectedTarget(attacker, firstHitTarget, Done);
            // Показать интерфейс
            GridBehaviour.Instance.OnTargetChoosen -= OnObjectChoosen;
        }

        private void Done()
        {
            GameController.Instance.UnitActionsEnd();
        }
    }


    public class StateGameEnd : State
    {
        public StateGameEnd(StateMachine stateMachine) : base(stateMachine) {
            UIActionsPanel.Instance.gameObject.SetActive(false);
        }

        // Нанести урон горящим зданиям, например
    }
}