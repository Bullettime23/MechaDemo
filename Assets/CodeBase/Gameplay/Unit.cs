using Common;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Mecha
{
    [RequireComponent(typeof(Collider))]
    public class Unit : DestructibleBase
    {
        [SerializeField] private float m_MoveSpeed;
        [SerializeField] private float m_RotationSpeed = 30;
        [SerializeField] private int m_MaxMoveDistanse;
        [SerializeField] private float m_MaxShotDistanse;
        [SerializeField] private float m_MinDistanceFromPoint = 0.1f;
        [SerializeField] private Turret[] m_Turrets;
        public int MaxDamage = 2;
        [Range(0f, 1f)]
        public float CritChanse = 0.1f;
        [SerializeField] private TankTower m_Tower;

        public bool ShouldMove = false;
        public Action OnMoveEnd;

        public Action OnStatusChange;

        private List<GameObject> m_Path;
        private int currentPathIndex = 0;
        private Vector3 m_MoveTarget;

        private GridStat m_CurrentGrid;
        public GridStat CurrentGrid => m_CurrentGrid;

        #region Unity Actions
        private void Start()
        {
            m_CurrentGrid = GridBehaviour.Instance.TryGetGrid(transform.position);
        }

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
                // Повернуться к следующей клетке
                if (TurnToNextPoint())
                {
                    MoveToNextNode();
                }

            }
        }

        #endregion

        #region Movement
        private bool TurnToNextPoint()
        {
            Vector3 direction = m_MoveTarget - transform.position;

            if (direction == Vector3.zero)
            {
                return true;
            }

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            if (transform.rotation == lookRotation)
            {
                return true;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, m_RotationSpeed * Time.deltaTime);
            return false;

        }

        private void MoveToNextNode()
        {
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
                    m_CurrentGrid = m_Path[m_Path.Count - 1].GetComponent<GridStat>();
                    m_CurrentGrid.PlaceObjectToGrid(gameObject);
                    //m_Covers = m_CurrentGrid.TakeCover();
                    ShouldMove = false;
                    OnMoveEnd?.Invoke();
                }
            }
        }
        #endregion

        [SerializeField] private int m_TeamNumber = 1;
        [SerializeField] private int m_MoveTokensInitial = 1;
        [SerializeField] private int m_AttackTokensInitial = 1;

        private int m_Move;
        private int m_MoveTokens;
        private int m_AttackTokens;

        #region Public API
        public bool HasActionTokens => m_MoveTokens + m_AttackTokens > 0;
        public int MoveTokens => m_MoveTokens;
        public int AttackTokens => m_AttackTokens;
        public int TeamNumber => m_TeamNumber;

        public void OnTurnStart()
        {
            m_AttackTokens = m_AttackTokensInitial;
            m_MoveTokens = m_MoveTokensInitial;
            OnStatusChange?.Invoke();
        }

        public void SetActionTokensToZero()
        {
            m_AttackTokens = 0;
            m_MoveTokens = 0;
        }

        public void MoveByPath(List<GameObject> path)
        {
            // Башню в начальное положение
            if (m_Tower != null)
                m_Tower.SetToInitialRotation();

            GridStat currentGrid = GridBehaviour.Instance.TryGetGrid(transform.position);
            currentGrid.RemoveObjectFromGrid();

            m_MoveTokens--;
            m_Path = path;
            currentPathIndex = 0;
            ShouldMove = true;
            OnStatusChange?.Invoke();
        }

        public void AimToTargetAndFire(Vector3 target, Action done)
        {
            m_Tower.SetTarget(target, WhenAimedToTarget);

            void WhenAimedToTarget()
            {
                System.Collections.IEnumerator FireCountDown()
                {
                    yield return new WaitForSeconds(1.5f);
                    foreach (Turret tur in m_Turrets)
                    {
                        tur.FireToPoint(target);
                    }
                    m_AttackTokens--;

                    OnStatusChange?.Invoke();

                    done?.Invoke();
                }

                StartCoroutine(FireCountDown());
            }
        }
        #endregion

        protected override void OnDeath()
        {
            base.OnDeath();
            SetActionTokensToZero();
        }

        public override void ApplyDamage(int damage)
        {
            base.ApplyDamage(damage);
            OnStatusChange?.Invoke();
        }
    }
}