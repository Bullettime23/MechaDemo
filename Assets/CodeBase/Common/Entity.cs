using UnityEngine;

namespace Common
{
    /// <summary>
    /// Базовый класс для всех интерактивных объектов на сцене
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        /// <summary>
        /// Название объекта для пользователя
        /// </summary>
        [SerializeField]
        private string m_Nickname;
        public string Nickname => m_Nickname;
    }
}