using UnityEngine;

namespace Common
{
    public class CircleArea : MonoBehaviour
    {
        [SerializeField] public float m_Radius;
        public float Radius => m_Radius;

        public Vector2 GetRandomInsideZone()
        {
            return (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * m_Radius;
        }

#if UNITY_EDITOR

        private static Color GizmoColor = new Color(0, 1, 0, 0.3f);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = GizmoColor;
            Gizmos.DrawSphere(transform.position, m_Radius);
        }
#endif
    }
}