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
        public DestructibleBase GetFirstObjectOfTrajectory(Unit shooter, GameObject target)
        {
            // Сделать Raycast от юнита до цели
            RaycastHit hit;

            Vector3 direction = target.transform.position - shooter.transform.position;
            // Выбрать первую цель на пути


            if (Physics.Raycast(shooter.transform.position, direction, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                Debug.DrawLine(shooter.transform.position, hit.point);
                return hit.transform.GetComponent<DestructibleBase>();
            }
            return null;
        }

        public void TryAttackSelectedTarget(Unit attacker, DestructibleBase target, Action attackOver)
        {

            Unit targetUnit = target.GetComponent<Unit>();

            // Может попасть или промахнуться
            float hitChance = 0.9f;
            float damageMultiplyer = 1f;
            float critChance = 0.5f;

            bool isHit = UnityEngine.Random.Range(hitChance, 1f) > 0.6f;
            bool isCritical = false;

            // Атака по статическому объекту всегда проходит, и может нанесети крит с 50% вероятностью
            if (targetUnit != null)
            {
                Dictionary<CoverDirection, Cover> covers = targetUnit.CurrentGrid.Covers;

                Vector3 reversedDirection = attacker.transform.position - targetUnit.transform.position;

                int attackDirectionInt = Mathf.CeilToInt((180 + Vector3.SignedAngle(Vector3.forward, reversedDirection, Vector3.forward)) / 90);

                Cover affiliatedCover;
                // Если цель в укрытии, она получает шанс увернуться, меньше урона, меньше вероятность крита

                if (covers.TryGetValue((CoverDirection)attackDirectionInt, out affiliatedCover))
                {
                    bool isFullCover = affiliatedCover.Type == CoverType.FullCover;
                    hitChance -= isFullCover ? 0.8f : 0.4f;
                    critChance -= isFullCover ? 0.5f : 0.3f;
                }
                Debug.Log($"Angle {180 + Vector3.SignedAngle(targetUnit.transform.position, reversedDirection, Vector3.forward)} CoverDirection {affiliatedCover?.Direction}");

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
                yield return new WaitForSeconds(1.5f);
                attackOver?.Invoke();
            }

            void AttackerFired()
            {
                if (isHit)
                {
                    target.ApplyDamage(calculatedDamage);
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