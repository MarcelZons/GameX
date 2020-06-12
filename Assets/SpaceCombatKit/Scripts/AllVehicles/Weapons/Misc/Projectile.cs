using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Base class for a projectile.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {

        protected Rigidbody rBody;

        protected List<IRootTransformUser> rootTransformUsers = new List<IRootTransformUser>();



        private void Awake()
        {
            rBody = GetComponent<Rigidbody>();
            rootTransformUsers = new List<IRootTransformUser>(GetComponentsInChildren<IRootTransformUser>());
        }

        // Reset rigidbody when enabled
        private void OnEnable()
        {
            rBody.velocity = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Set the sender's root transform.
        /// </summary>
        /// <param name="senderRootTransform">The sender's root transform.</param>
        public void SetSenderRootTransform(Transform senderRootTransform)
        {
            for (int i = 0; i < rootTransformUsers.Count; ++i)
            {
                rootTransformUsers[i].SetRootTransform(senderRootTransform);
            }
        }

        /// <summary>
        /// Set the velocity of the projectile.
        /// </summary>
        /// <param name="velocity">The new projectile velocity.</param>
        public virtual void SetVelocity(Vector3 velocity)
        {
            rBody.velocity = velocity;
        }
    }
}