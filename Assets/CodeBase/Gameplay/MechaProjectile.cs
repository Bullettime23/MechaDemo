using UnityEngine;

namespace Mecha
{
    public class MechaProjectile : MonoBehaviour
    {
        [SerializeField] private float m_MoveSpeed;
        [SerializeField] private float m_LifeTime;
        [SerializeField] private Transform m_Target;

        private float m_TimeLeft;
        public Vector3 FinishPosition;
        private Vector3 m_Direction;

        private void Start()
        {
            if (m_Target != null)
            {
                FinishPosition = m_Target.position;
            }
            m_Direction = FinishPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(FinishPosition);
            m_TimeLeft = m_LifeTime;
        }
        private void Update()
        {
            m_TimeLeft -= Time.deltaTime;
            Vector3 currentDistance = FinishPosition - transform.position;

            if (m_TimeLeft <= 0 || Vector3.Dot(currentDistance, m_Direction) <= 0)
                OnLifeEnd();
            transform.position += m_Direction * m_MoveSpeed * Time.deltaTime;
        }

        private void OnLifeEnd()
        {
            Destroy(gameObject);
            print("Projectile life end");
        }
    }
}