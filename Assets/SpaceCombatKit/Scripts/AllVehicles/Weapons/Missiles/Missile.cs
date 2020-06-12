using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Base class for a guided missile.
    /// </summary>
    public class Missile : MonoBehaviour
    {

        [SerializeField]
        protected CollisionScanner collisionScanner;

        [SerializeField]
        protected DamageEmitter damageEmitter;
        
        [SerializeField]
        protected TargetProximityTrigger targetProximityTrigger;

        [SerializeField]
        protected TargetLocker targetLocker;

        [SerializeField]
        protected TargetLeader targetLeader;

        [SerializeField]
        protected Rigidbody rBody;

        protected List<IRootTransformUser> rootTransformUsers = new List<IRootTransformUser>();


        protected virtual void Awake()
        {
            rootTransformUsers = new List<IRootTransformUser>(GetComponentsInChildren<IRootTransformUser>());
        }

        /// <summary>
        /// Set the sender's root gameobject.
        /// </summary>
        /// <param name="senderRootGameObject">The sender's root gameobject.</param>
        public virtual void SetSenderRootTransform(Transform senderRootTransform)
        {
            for (int i = 0; i < rootTransformUsers.Count; ++i)
            {
                rootTransformUsers[i].SetRootTransform(senderRootTransform);
            }
        }

        /// <summary>
        /// Set the target.
        /// </summary>
        /// <param name="target">The new target.</param>
        public virtual void SetTarget(Trackable target)
        {
            if (targetLocker != null) targetLocker.SetTarget(target);
            if (targetLeader != null) targetLeader.SetTarget(target);
            if (targetProximityTrigger != null) targetProximityTrigger.SetTarget(target);
        }

        /// <summary>
        /// Set the lock state of the missile.
        /// </summary>
        /// <param name="lockState">The new lock state.</param>
        public virtual void SetLockState(LockState lockState)
        {
            if (targetLocker != null) targetLocker.SetLockState(lockState);
        }

        /// <summary>
        /// Set the velocity of the missile.
        /// </summary>
        /// <param name="velocity">The velocity to be set.</param>
        public virtual void SetVelocity(Vector3 velocity)
        {
            if (rBody != null) rBody.velocity = velocity;
        }
    }
}