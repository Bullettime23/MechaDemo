using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace Mecha
{
    public enum TurretMode
    {
        Single = 0,
        Multiple = 1,
    }

    [CreateAssetMenu(fileName = "TurretProperties", menuName = "Scriptable Objects/TurretPropertiesMecha")]
    public class TurretPropertiesMecha : ScriptableObject
    {
        /// <summary>
        /// Турель реализует разную логику в зависимости от типа
        /// </summary>
        [SerializeField] private TurretMode m_Mode = TurretMode.Single;

        public TurretMode Mode => m_Mode;

        /// <summary>
        /// Ссылка на префаб прожектайла который будет стрелять турель.
        /// </summary>
        [SerializeField] private MechaProjectile m_ProjectilePrefab;
        public MechaProjectile ProjectilePrefab => m_ProjectilePrefab;

        /// <summary>
        /// Звук выстрела. Это на ДЗ добавить самим звук при выстреле.
        /// </summary>
        [SerializeField] private AudioClip m_LaunchSFX;
        public AudioClip LaunchSFX => m_LaunchSFX;

        [Header("Automatic gun")]
        /// <summary>
        /// Работает только в режиме Multiple
        /// </summary>
        [SerializeField] private int m_ShotsPerTurret = 3;
        public int ShotsPerTurret => m_ShotsPerTurret;

        [SerializeField] private float m_ShotsInterval = 0.5f;
        public float ShotsInterval => m_ShotsInterval;
    }
}

