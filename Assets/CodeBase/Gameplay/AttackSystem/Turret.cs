using UnityEngine;

namespace Mecha
{
    public class Turret : MonoBehaviour
    {
        /// <summary>
        /// Текущие патроны в турели.
        /// </summary>
        [SerializeField] private TurretPropertiesMecha m_TurretProperties;

        private Vector3 m_Target;
        private float m_Cooldown = 0;
        private float m_ShotsLeft;

        private void Update()
        {
            if (m_TurretProperties.Mode == TurretMode.Multiple && m_ShotsLeft > 0)
            {
                m_Cooldown -= Time.deltaTime;

                if (m_Cooldown <= 0)
                {
                    //Каждый выстрел ставить турель на кулдаун
                    LaunchProjectileToTheTarget();
                    m_ShotsLeft--;
                    m_Cooldown = m_TurretProperties.ShotsInterval;
                }
            }
        }

        #region Public API

        /// <summary>
        /// Метод стрельбы турелью. 
        /// </summary>
        public void FireToPoint(Vector3 target)
        {
            if (m_TurretProperties == null)
                return;

            m_Target = target;

            if (m_TurretProperties.Mode == TurretMode.Single)
            {
                LaunchProjectileToTheTarget();

                if (m_TurretProperties.LaunchSFX)
                {
                    // SFX на домашку
                    AudioSource.PlayClipAtPoint(m_TurretProperties.LaunchSFX, transform.position);

                }
            }

            if (m_TurretProperties.Mode == TurretMode.Multiple)
            {
                m_ShotsLeft = m_TurretProperties.ShotsPerTurret;

                if (m_TurretProperties.LaunchSFX)
                {
                    // SFX на домашку
                    AudioSource.PlayClipAtPoint(m_TurretProperties.LaunchSFX, transform.position);

                }
            }
        }


        /// <summary>
        /// Установка свойств турели. Будет использовано в дальнейшем для паверапки.
        /// </summary>
        /// <param name="props"></param>
        //public void AssignLoadout(TurretPropertiesMecha props)
        //{
        //    m_TurretProperties = props;
        //}
        #endregion

        private void LaunchProjectileToTheTarget()
        {
            // инстанцируем прожектайл который уже сам полетит.
            var projectile = Instantiate(m_TurretProperties.ProjectilePrefab.gameObject).GetComponent<MechaProjectile>();
            projectile.transform.position = transform.position;
            projectile.transform.up = transform.up;

            // метод выставления данных прожектайлу о том кто стрелял для избавления от попаданий в самого себя
            projectile.FinishPosition = m_Target;
        }
    }
}