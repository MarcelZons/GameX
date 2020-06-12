using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// This class implements engines (movement, steering and boost) for a space vehicle
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleEngines3D : Engines
	{

        // The current translation forces available on each axis (according to power availability etc)
        protected Vector3 availableMovementForces = Vector3.zero;
        public Vector3 AvailableMovementForces { get { return availableMovementForces; } }

        // The current rotation forces available on each axis (according to power availability etc)
        protected Vector3 availableSteeringForces = Vector3.zero;
        public Vector3 AvailableSteeringForces { get { return availableSteeringForces; } }
       
        // The current boost forces available on each axis (according to power availability etc)
        protected Vector3 availableBoostForces = Vector3.zero;
        public Vector3 AvailableBoostForces { get { return availableBoostForces; } }

        [Header("Default Forces")]

        [Tooltip("The default translation (thrust) forces for each axis (used when Power component is not being used).")]
        [SerializeField]
        protected Vector3 defaultMovementForces = new Vector3(200, 200, 300);

        [Tooltip("The default rotation (steering torque) forces for each axis (used when Power component is not being used).")]
        [SerializeField]
        protected Vector3 defaultSteeringForces = new Vector3(8f, 8f, 18f);

        [Tooltip("The default boost forces for each axis (used when Power component is not being used).")]
        [SerializeField]
        protected Vector3 defaultBoostForces = new Vector3(200, 200, 300);

        // 
        [Tooltip("Whether to add the full throttle forces to the boost forces or implement boost forces by themselves.")]
        [SerializeField]
        protected bool applyMovementForcesDuringBoost = true;

        [Header("Limits")]

        [Tooltip("The translation (thrust) forces limits for each axis. For example, clamping vehicle speed regardless of power settings.")]
        [SerializeField]
        protected Vector3 maxMovementForces = new Vector3(400, 400, 600);
        public Vector3 MaxMovementForces { get { return maxMovementForces; } }

        [Tooltip("The rotation (steering) force limits for each axis. For example, clamping vehicle rotation speed regardless of power settings.")]
        [SerializeField]
        protected Vector3 maxSteeringForces = new Vector3(8f, 8f, 18f);
        public Vector3 MaxSteeringForces { get { return maxSteeringForces; } }

        [SerializeField]
        protected Rigidbody m_rigidbody;
        

        /// Called when this component is first added to a gameobject or reset in the inspector
        protected virtual void Reset()
        {

            m_rigidbody = GetComponent<Rigidbody>();

            // Initialize the rigidbody with good values
            m_rigidbody.useGravity = false;
            m_rigidbody.mass = 1;
            m_rigidbody.drag = 3;
            m_rigidbody.angularDrag = 4;
        }

       
        protected virtual void Awake()
        {
            // Cache the rigidbody
            m_rigidbody = GetComponent<Rigidbody>();
        }

        // Called when the scene starts
        protected override void Start()
        {
            base.Start();
            
            // Start off with the default forces 
            availableSteeringForces = defaultSteeringForces;
            availableMovementForces = defaultMovementForces;
            availableBoostForces = defaultBoostForces;
        }


        /// <summary>
        /// Get the maximum speed on each axis, for example for loadout data.
        /// </summary>
        /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
        /// <returns>The maximum speed on each axis.</returns>
        public override Vector3 GetDefaultMaxSpeedByAxis(bool withBoost)
		{
            Vector3 maxForces = defaultMovementForces + (withBoost ? defaultBoostForces : Vector3.zero);
            maxForces = Vector3.Min(maxForces, maxMovementForces);
            
			return (new Vector3(GetSpeedFromForce(maxForces.x), GetSpeedFromForce(maxForces.y), GetSpeedFromForce(maxForces.z)));

		}

        /// <summary>
        /// Get the current maximum speed on each axis, for example for normalizing speed indicators.
        /// </summary>
        /// <param name="withBoost">Whether to include boost in the maximum speed.</param>
        /// <returns>The maximum speed on each axis.</returns>
        public override Vector3 GetCurrentMaxSpeedByAxis(bool withBoost)
        {
            Vector3 maxForces = availableMovementForces + (withBoost ? availableBoostForces : Vector3.zero);
            maxForces = Vector3.Min(maxForces, maxMovementForces);

            return (new Vector3(GetSpeedFromForce(maxForces.x), GetSpeedFromForce(maxForces.y), GetSpeedFromForce(maxForces.z)));

        }


        /// <summary>
        /// Calculate the maximum speed of this Rigidbody for a given force.
        /// </summary>
        /// <param name="force">The linear force to be used in the calculation.</param>
        /// <returns>The maximum speed.</returns>
        protected virtual float GetSpeedFromForce(float force)
		{
			float delta_V_Thrust = (force / m_rigidbody.mass) * Time.fixedDeltaTime;
			float dragFactor = Time.fixedDeltaTime * m_rigidbody.drag;
			float maxSpeed = delta_V_Thrust / dragFactor;

			return maxSpeed;
		}

        // Apply the physics
        protected virtual void FixedUpdate()
		{
            if (enginesActivated)
            {

                // Implement steering torques
                m_rigidbody.AddRelativeTorque(Vector3.Scale(steeringInputs, availableSteeringForces), ForceMode.Acceleration);

                Vector3 nextTranslationThrottleValues = movementInputs;

                // If full throttle is to be applied during boost, add it
                if (applyMovementForcesDuringBoost)
                {
                    if (boostInputs.x > 0.5f)
                        nextTranslationThrottleValues.x = Mathf.Clamp(Mathf.Sign(nextTranslationThrottleValues.x), minMovementInputs.x, maxMovementInputs.x);
                    if (boostInputs.y > 0.5f)
                        nextTranslationThrottleValues.y = Mathf.Clamp(Mathf.Sign(nextTranslationThrottleValues.y), minMovementInputs.y, maxMovementInputs.y);
                    if (boostInputs.z > 0.5f)
                        nextTranslationThrottleValues.z = Mathf.Clamp(1, minMovementInputs.z, maxMovementInputs.z);

                }
                
                // Get next forces to be applied
                Vector3 nextForces = Vector3.Scale(nextTranslationThrottleValues, availableMovementForces) +
                                        Vector3.Scale(boostInputs, availableBoostForces);

                // Clamp forces within limits
                nextForces = Vector3.Min(nextForces, maxMovementForces);

                // Implement forces
                m_rigidbody.AddRelativeForce(nextForces);
            }

		}
	}
}
