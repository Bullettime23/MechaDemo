using System;
using UnityEngine;

namespace Mecha
{
    // Скрипт вешается на танк
    public class TankTower : MonoBehaviour
    {
        [SerializeField] private Transform m_Tower;


        private Quaternion m_InitialRotation;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            m_InitialRotation = m_Tower.transform.rotation;
        }


        // TODO: Сделать анимацию вращения башни
        public void SetTarget(Vector3 targetPosition, Action delegat)
        {
            // Цель на одном уровне с башней, но отличается по координатам X и Z
            Vector3 direction = new Vector3(targetPosition.x, m_Tower.position.y, targetPosition.z) - m_Tower.transform.position;

            m_Tower.transform.rotation = Quaternion.LookRotation(direction) * m_InitialRotation;
            delegat?.Invoke();
        }
        public void SetToInitialRotation()
        {
            // Выровнять башню относительно куорпуса

            m_Tower.rotation = m_InitialRotation;
        }
    }
}