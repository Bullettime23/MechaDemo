using UnityEngine;

namespace Common
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [SerializeField] private float m_Velocity;
        public float Velocity => m_Velocity;
        [SerializeField] private float m_Lifetime;
        [SerializeField] private int m_Damage;
        [SerializeField] protected ImpactEffect m_ImpactEffectPrefab;

        protected virtual void OnHit(DestructibleBase destr) { }
        protected virtual void OnHit(Collider col) { }
        protected virtual void OnProjectileLifetimeEnd(Collider collider, Vector3 point) { }

        private Timer m_LifeTimer;

        private void Start()
        {
            m_LifeTimer = new Timer(m_Lifetime);
        }

        private void Update()
        {
            float stepLenght = Time.deltaTime * m_Velocity;
            Vector3 step = transform.forward * stepLenght;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.up, out hit, stepLenght))
            {
                OnHit(hit.collider);

                DestructibleBase dest = hit.collider.transform.root.GetComponent<DestructibleBase>();

                if (dest != null && dest != m_Parent)
                {
                    dest.ApplyDamage(m_Damage);

                    OnHit(dest);

                }

                if (dest != m_Parent) OnProjectileLifetimeEnd(hit.collider, hit.point);
            }

            m_LifeTimer.RemoveTime(Time.deltaTime);

            if (m_LifeTimer.IsFinish)
                OnProjectileLifetimeEnd(hit.collider, hit.point);

            transform.position += step;
        }

        private protected DestructibleBase m_Parent;
        public void SetParent(DestructibleBase parent)
        {
            m_Parent = parent;
        }

        public DestructibleBase GetParent() => m_Parent;
    }
}
