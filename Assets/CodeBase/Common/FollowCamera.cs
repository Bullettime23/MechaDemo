using UnityEngine;

namespace Common
{
    public class FollowCamera : MonoBehaviour
    {

        [SerializeField] private Transform m_Target;

        [SerializeField] private float m_InterpolationLinear;

        [SerializeField] private float m_InterpolationAngular;

        [SerializeField] private float m_CameraZOffset;

        [SerializeField] private float m_CameraForwardOffset;

        private void FixedUpdate()
        {
            // Чтобы не было ошибки при попытке обратиться к несуществующему объекту
            if (m_Target == null) return;

            Vector2 camPos = transform.position;

            //Чтобы камера смотрела немного вперед, добавляется отступ
            Vector2 target = m_Target.position + m_Target.transform.up * m_CameraForwardOffset;

            // Чтобы сместить камеру, используем интерполяцию между начальной точкой, конечной и линейную скорость умножаем на время между кадрами
            Vector2 newCamPos = Vector2.Lerp(camPos, target, m_InterpolationLinear * Time.deltaTime);

            transform.position = new Vector3(newCamPos.x, newCamPos.y, m_CameraZOffset);

            // Вращение камеры
            if (m_InterpolationAngular > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, m_Target.rotation, m_InterpolationAngular * Time.deltaTime);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            m_Target = newTarget;
        }
    }
}