using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Infrastructure;
using UnityEngine;

// TODO: Может, сделать этот класс Static?
namespace Mecha
{
    public class AttackSystem : Singleton<AttackSystem>
    {
        [SerializeField] private float m_CriticalDamageMultiplyer = 1.5f;
        [SerializeField]
        private float
            m_DefalutHitChanse = 0.9f,
            m_DefaultCritChanse = 0.2f,
            m_HitChanceReduceByFullCover,
            m_CritReduceByFullCover,
            m_HitChanceReduceByHalfCover,
            m_CritReduceByHalfCover;

        public DestructibleBase GetFirstObjectOfTrajectory(Unit shooter, GameObject target)
        {
            // Сделать Raycast от юнита до цели

            Unit targetUnit = target.GetComponent<Unit>();

            Vector3 direction = target.transform.position - shooter.transform.position;
            // Выбрать первую цель на пути

            RaycastHit[] hits = Physics.RaycastAll(shooter.transform.position, direction, direction.magnitude);

            foreach (RaycastHit hit in hits)
            {
                print(hit.collider.gameObject.name);
            }

            //Узнать, блокируется ли цель 
            for (int i = 0; i < hits.Length; i++)
            {
                DestructibleBase destructible = hits[i].collider.gameObject.GetComponent<DestructibleBase>();

                if (destructible == null)
                {
                    //Игнорировать случайные объекты на сцене
                    continue;
                }
                // Не может застрелить себя
                if (destructible == shooter.GetComponent<DestructibleBase>())
                {
                    continue;
                }

                //Собственное укрытие не считается
                if (shooter.CoverTook != null && destructible == shooter.CoverTook.Desturctible)
                {
                    continue;
                }

                //Урон по противнику в укрытии считается в следующем методе
                if (targetUnit != null && targetUnit.CoverTook != null && destructible == targetUnit.CoverTook.Desturctible)
                {
                    //Дальше снаряд не полетит )
                    break;
                }

                // Есть какое-то препятствие
                return destructible;
            }

            return target.GetComponent<DestructibleBase>();
        }

        public void TryAttackSelectedTarget(Unit attacker, DestructibleBase target, Action attackOver)
        {

            Unit targetUnit = target.GetComponent<Unit>();

            // Может попасть или промахнуться
            float hitChance = m_DefalutHitChanse;
            float damageMultiplyer = 1f;
            float critChance = m_DefaultCritChanse;

            bool isHit = UnityEngine.Random.Range(hitChance, 1f) > 0.6f;
            bool isCritical = false;

            // Атака по статическому объекту всегда проходит, и может нанесети крит с 50% вероятностью
            if (targetUnit != null)
            {
                Dictionary<CoverDirection, Cover> covers = targetUnit.CurrentGrid.Covers;

                Vector3 reversedDirection = attacker.transform.position - targetUnit.transform.position;

                // -180 => 180
                float signedAngle = Vector3.SignedAngle(Vector3.forward, reversedDirection, Vector3.forward);

                float attackAngle = signedAngle >= 0 ? signedAngle : signedAngle + 180f;

                int attackDirectionInt = Mathf.CeilToInt(attackAngle / 90) + 1;

                Cover affiliatedCover;
                // Если цель в укрытии, она получает шанс увернуться, меньше урона, меньше вероятность крита

                if (covers.TryGetValue((CoverDirection)attackDirectionInt, out affiliatedCover))
                {
                    bool isFullCover = affiliatedCover.Type == CoverType.FullCover;
                    hitChance -= isFullCover ? m_HitChanceReduceByFullCover : m_HitChanceReduceByHalfCover;
                    critChance -= isFullCover ? m_CritReduceByFullCover : m_CritReduceByHalfCover;
                }

                isCritical = UnityEngine.Random.Range(attacker.CritChanse + critChance, 1f) > 0.6f;
                if (isCritical)
                {
                    damageMultiplyer = m_CriticalDamageMultiplyer;
                }

                isHit = UnityEngine.Random.Range(hitChance, 1f) > 0.6f;
                // Снизить урон при попадании по цели в укрытии
                if (isHit && affiliatedCover != null && !isCritical)
                {
                    damageMultiplyer -= 0.2f;
                }
            }


            int calculatedDamage = Mathf.CeilToInt(attacker.MaxDamage * damageMultiplyer);

            // Нанести урон юниту или укрытию, за которым он прячется

            IEnumerator WaitAfterShotAnimation()
            {
                yield return new WaitForSeconds(2.5f);
                attackOver?.Invoke();
            }

            void AttackerFired()
            {
                DestructibleBase damageTaker = target;
                if (isHit)
                {
                    target.ApplyDamage(calculatedDamage);
                }
                //Если урон приходится не на юнит за укрытием, то на укрытие
                if (targetUnit != null && !isHit && targetUnit.CoverTook != null)
                {
                    damageTaker = targetUnit.CoverTook.Desturctible;
                    damageTaker.ApplyDamage(calculatedDamage);
                }

                // Показать интерфейс и проиграть анимацию попадания
                Debug.Log($"{attacker.name} deal {calculatedDamage} damage to the {target.name}");

                if (isHit)
                {
                    if (isCritical)
                    {
                        UIDamagePanel.Instance.ShowCritical(target.transform.position, calculatedDamage);

                    }
                    else
                    {
                        UIDamagePanel.Instance.ShowDamage(target.transform.position, calculatedDamage);
                    }
                }
                else
                {
                    UIDamagePanel.Instance.ShowMiss(target.transform.position);
                }

                StartCoroutine(WaitAfterShotAnimation());

            }

            // Camera to attack
            CameraMovement.Instance.FocusOn(attacker.transform.position + (target.transform.position - attacker.transform.position) / 2);

            // Проиграть анимацию
            attacker.AimToTargetAndFire(target.transform.position, AttackerFired);


        }
    }
}