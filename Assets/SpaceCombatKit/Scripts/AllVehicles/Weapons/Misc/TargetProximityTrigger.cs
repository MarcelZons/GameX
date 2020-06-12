using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.UniversalVehicleCombat.Radar;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity event for running functions when the target proximity trigger is triggered.
    /// </summary>
    [System.Serializable]
    public class OnTargetProximityTriggerTriggeredEventHandler : UnityEvent { }

    /// <summary>
    /// Triggers when a target enters a trigger collider, with the option to only trigger when the distance is projected to increase
    /// in the next frame.
    /// </summary>
    public class TargetProximityTrigger : MonoBehaviour
    {

        [Header("Settings")]

        [SerializeField]
        protected Trackable target;

        [SerializeField]
        protected DamageReceiverScanner scanner;

        [SerializeField]
        protected bool triggerOnDistanceIncrease = true;

        protected bool targetInsideTrigger = false;

        protected float closestDistance;    // Keep a running tab on closest distance to trigger when it increases.

        [SerializeField]
        protected string targetTrackableVelocityKey = "Velocity";

        [SerializeField]
        protected Rigidbody rigidBody;

        [Header("Events")]

        // Proximity trigger triggered event
        public OnTargetProximityTriggerTriggeredEventHandler onTriggered;

        protected bool triggered = false;


        protected virtual void Awake()
        {

            rigidBody = GetComponent<Rigidbody>();

            if (scanner != null)
            {
                scanner.onDamageReceiverDetected.AddListener(OnDamageReceiverDetected);
                scanner.onDamageReceiverUndetected.AddListener(OnDamageReceiverUndetected);
            }
        }


        protected virtual void OnEnable()
        {
            triggered = false;
        }

        /// <summary>
        /// Set the target.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        public virtual void SetTarget(Trackable newTarget)
        {
            this.target = newTarget;
        }

        // Called when a damage receiver enters the damage receiver scanner's range
        protected virtual void OnDamageReceiverDetected(DamageReceiver damageReceiver)
        {
            if (target != null)
            {
                if (damageReceiver.RootTransform == target.RootTransform)
                {
                    targetInsideTrigger = true;
                }
            }
        }

        // Called when a damage receiver exits the damage receiver scanner's range
        protected virtual void OnDamageReceiverUndetected(DamageReceiver damageReceiver)
        {
            if (target != null)
            {
                if (damageReceiver.RootTransform == target.RootTransform)
                {
                    targetInsideTrigger = false;
                }
            }
        }


        // Check if the trigger should activated
        protected virtual bool CheckTrigger()
        {

            if (target == null) return false;

            if (targetInsideTrigger)
            {
                if (triggerOnDistanceIncrease)
                {

                    if (GetClosestDistanceToTarget(Time.deltaTime) > GetClosestDistanceToTarget(0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


        // Get the closest distance to the target based on a time projection (necessary because things can move very fast and the target can
        // change position a lot in one frame).
        protected virtual float GetClosestDistanceToTarget(float timeProjection)
        {

            Vector3 targetVelocity = target.variablesDictionary.ContainsKey(targetTrackableVelocityKey) ?
                                        target.variablesDictionary[targetTrackableVelocityKey].Vector3Value() : Vector3.zero;

            Vector3 targetOffset = targetVelocity * timeProjection;
            targetOffset -= rigidBody.velocity * timeProjection;

            float result = scanner.ScannerTriggerCollider.radius;

            for (int i = 0; i < scanner.DamageReceiversInRange.Count; ++i)
            {
                if (scanner.DamageReceiversInRange[i].RootTransform == target.gameObject)
                {

                    Vector3 colliderWorldPos = scanner.ScannerTriggerCollider.bounds.center;
                    colliderWorldPos -= targetOffset;

                    Vector3 nextClosestPos = scanner.DamageReceiversInRange[i].GetClosestPoint(colliderWorldPos);

                    float distanceToTarget = Vector3.Distance(nextClosestPos, colliderWorldPos);
                    if (distanceToTarget < result)
                    {
                        result = distanceToTarget;
                    }
                }
            }
            
            return result;
        }


        // Called every frame
        protected virtual void Update()
        {
            if (targetInsideTrigger)
            {
                if (CheckTrigger() && !triggered)
                {
                    onTriggered.Invoke();
                    triggered = true;
                }
            }
        }
    }
}