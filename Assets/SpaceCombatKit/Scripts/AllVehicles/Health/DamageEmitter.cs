using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Emits damage to damage receivers, either at a raycast hit point, or to all damage receivers in the vicinity.
    /// </summary>
    public class DamageEmitter : MonoBehaviour
    {

        [Header("Damage")]

        [SerializeField]
        protected List<HealthTypeDamageValue> healthTypeDamageValues = new List<HealthTypeDamageValue>();
        public List<HealthTypeDamageValue> HealthTypeDamageValues { get { return healthTypeDamageValues; } }

        [SerializeField]
        protected DamageSourceType damageSourceType;

        // Whether to apply the damage amount by the second (e.g. for laser beams).
        [SerializeField]
        protected bool damagePerSecond = false;

        protected float damageMultiplier;

        [Header("Area Damage")]

        [SerializeField]
        protected DamageReceiverScanner damageReceiverScanner;

        [SerializeField]
        protected AnimationCurve damageFalloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

        protected Transform rootTransform;
        

        
        public void SetDamageMultiplier(float damageMultiplier)
        {
            this.damageMultiplier = damageMultiplier;
        }

        /// <summary>
        /// Emit damage based on a raycast hit.
        /// </summary>
        /// <param name="hit">The raycast hit information.</param>
        public void RaycastHitDamage(RaycastHit hit)
        {
            
            DamageReceiver damageReceiver = hit.collider.GetComponent<DamageReceiver>();

            if (damageReceiver != null)
            {
                if (damageReceiver.RootTransform == rootTransform) return;

                for (int i = 0; i < healthTypeDamageValues.Count; ++i)
                {
                    if (healthTypeDamageValues[i].HealthType == damageReceiver.HealthType)
                    {
                        float damage = healthTypeDamageValues[i].DamageValue;

                        if (damagePerSecond)
                        {
                            damage *= Time.deltaTime;
                        }

                        damageReceiver.Damage(damage, hit.point, damageSourceType, rootTransform);
                    }
                }
            }
            
        }

        /// <summary>
        /// Emit damage from the current position to any damage receivers in range.
        /// </summary>
        public void EmitDamage()
        {
            // Damage all the damageables that are inside the damageable scanner's trigger collider
            EmitDamageFromPoint(transform.position);
        }


        /// <summary>
        /// Emit damage from a point in world space to any damage receivers in range.
        /// </summary>
        /// <param name="damageEmissionPoint">The world space point from which to emit damage.</param>
        public void EmitDamageFromPoint(Vector3 damageEmissionPoint)
        {
            if (damageReceiverScanner != null)
            {
                // Go through all the detected damageables and damage them according to the damage falloff parameters
                for (int i = 0; i < damageReceiverScanner.DamageReceiversInRange.Count; ++i)
                {

                    if (damageReceiverScanner.DamageReceiversInRange[i].RootTransform == rootTransform.gameObject) return;

                    // Get the distance from the damage emission center to the damageable's closest point
                    Vector3 closestPoint = damageReceiverScanner.DamageReceiversInRange[i].GetClosestPoint(damageEmissionPoint);
                    float distance = Vector3.Distance(damageEmissionPoint, closestPoint);

                    // Emit each type of damage this damage emitter can emit, based on falloff curve
                    for (int j = 0; j < healthTypeDamageValues.Count; ++j)
                    {
                        if (healthTypeDamageValues[j].HealthType == damageReceiverScanner.DamageReceiversInRange[i].HealthType)
                        {
                            float damage = damageFalloffCurve.Evaluate(distance / damageReceiverScanner.ScannerTriggerCollider.radius) * healthTypeDamageValues[j].DamageValue;

                            if (damagePerSecond)
                            {
                                damage *= Time.deltaTime;
                            }

                            damageReceiverScanner.DamageReceiversInRange[i].Damage(damage, closestPoint, damageSourceType, rootTransform);

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the root transform that is passed to any Damage Receivers.
        /// </summary>
        /// <param name="rootTransform">The root transform.</param>
        public void SetRootTransform(Transform rootTransform)
        {
            this.rootTransform = rootTransform;
        }
    }
}