using Common;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Mecha
{
    public class Unit : DestructibleBase
    {
        [SerializeField] private float m_MoveSpeed;
        [SerializeField] private int m_MaxMoveDistanse;
        [SerializeField] private float m_MaxShotDistanse;
        [SerializeField] private float m_MinDistanceFromPoint = 0.1f;

        private bool m_IsMovedThisTurn;

        public bool ShouldMove = false;
        public Action OnMoveEnd;

        private List<GameObject> m_Path;
        private int currentPathIndex = 0;
        private Vector3 m_MoveTarget;




        private void Update()
        {
            if (ShouldMove)
            {

                // Получить следующий узел маршрута
                if (currentPathIndex < m_Path.Count)
                {
                // Найти его вектор в мировых координатах
                    Vector3 nextNodePostion = m_Path[currentPathIndex].transform.position;
                    m_MoveTarget = new Vector3(nextNodePostion.x, transform.position.y, nextNodePostion.z);
                }
                // двигаться к нему, пока не будет достигнут
                transform.position += (m_MoveTarget - transform.position) * m_MoveSpeed * Time.deltaTime;
                // повторить для следующего узла
                // если последний узел, ShouldMove = false
                if (Vector3.Distance(transform.position, m_MoveTarget) < m_MinDistanceFromPoint)
                {
                    if (currentPathIndex < m_Path.Count)
                        currentPathIndex++;
                    else
                    {
                        ShouldMove = false;
                        OnMoveEnd?.Invoke();
                    }
                }
            }
        }

        [SerializeField] private int m_TeamNumber;

        private int m_ActionTokens;

        #region Public API
        public bool IsMovedThisTurn => m_IsMovedThisTurn;
        public int ActionTokens => m_ActionTokens;
        public int TeamNumber => m_TeamNumber;

        public void OnTurnStart()
        {
            m_ActionTokens = 2;
            m_IsMovedThisTurn = false;
        }

        public void OnSkip()
        {
            m_ActionTokens = 0;
        }
        public void Attack()
        {
            m_ActionTokens = 0;
        }

        public void MoveByPath(List<GameObject> path)
        {
            m_ActionTokens--;
            m_IsMovedThisTurn = true;
            m_Path = path;
            ShouldMove = true;
        }

        #endregion

        protected override void OnDeath()
        {
            base.OnDeath();
            m_ActionTokens = 0;
        }
    }
}