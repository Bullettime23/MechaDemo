using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    /// <summary>
    /// Уничтожаемый объект на сцене, который может иметь хитпоинты
    /// </summary>
    public abstract class DestructibleBase : Entity
    {
        #region Properties
        /// <summary>
        /// Объект игнорирует повреждения
        /// </summary>
        [SerializeField]
        private bool m_Indestructable;
        public bool IsIndestructable => m_Indestructable;

        /// <summary>
        /// Start amout of health
        /// </summary>
        [SerializeField]
        private int m_HitPoints;
        public int InitialHitPoints => m_HitPoints;

        /// <summary>
        /// Текущее количество жизней
        /// </summary>
        private int m_CurrentHitPoints;
        public int CurrentHitPoints => m_CurrentHitPoints;

        #endregion

        #region Unity_Events

        protected virtual void Awake()
        {
            m_CurrentHitPoints = m_HitPoints;
        }
        #endregion

        #region Public API

        /// <summary>
        /// Вызывается при получении объектом урона
        /// </summary>
        /// <param name="damage">Количество урона</param>
        public virtual void ApplyDamage(int damage)
        {
            if (m_Indestructable) return;

            m_CurrentHitPoints -= damage;

            if (m_CurrentHitPoints <= 0) OnDeath();
        }
        #endregion

        /// <summary>
        /// Вызывается при гибели объекта
        /// </summary>
        protected virtual void OnDeath()
        {
            m_EventOnDeath?.Invoke(this);
            Destroy(gameObject);
        }

        [SerializeField] private UnityEvent<DestructibleBase> m_EventOnDeath;
        public UnityEvent<DestructibleBase> EventOnDeath => m_EventOnDeath;

        #region Collection
        private static HashSet<DestructibleBase> m_AllDestructibles;
        public static IReadOnlyCollection<DestructibleBase> AllDestructibles => m_AllDestructibles;

        protected virtual void OnEnable()
        {
            if (m_AllDestructibles == null)
                m_AllDestructibles = new HashSet<DestructibleBase>();

            m_AllDestructibles.Add(this);
        }

        protected virtual void OnDestroy()
        {
            m_AllDestructibles.Remove(this);
        }
        #endregion
    }
}
