using Common;

namespace Mecha
{
    public class Projectile : ProjectileBase
    {

        private Unit m_ParentShooter;


        #region Public API
        public void SetParentShooter(Unit parentShooter)
        {
            m_ParentShooter = parentShooter;
        }
        #endregion

        protected override void OnHit(DestructibleBase destr)
        {
            print($"Hit {destr.gameObject.name}");
        }
    }
}