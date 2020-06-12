using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    public class MissileGuidanceSystem : MonoBehaviour
    {
        [Header("Missile Guidance")]

        [SerializeField]
        protected TargetLeader targetLeader;

        [SerializeField]
        protected VehicleEngines3D engines;

        [SerializeField]
        protected float maxLeadTargetAngle = 20;

        [Header("Maneuvring")]

        protected Vector3 controlValuesByAxis;

        [SerializeField]
        protected Vector3 maxRotationAngles = new Vector3(360, 360, 360);

        [Header("PID Controller")]

        public PIDController3D steeringPIDController;


        // Update is called once per frame
        void Update()
        {
            // Turn toward the lead target position
            if (targetLeader.Target != null)
            {
                // Set the target leader's intercept speed as the current missile speed
                targetLeader.InterceptSpeed = engines.GetCurrentMaxSpeedByAxis(false).z;

                Vector3 toTargetVector = targetLeader.Target.transform.position - transform.position;
                Vector3 adjustedLeadTargetPos = transform.position + (targetLeader.LeadTargetPosition - transform.position).normalized * toTargetVector.magnitude;

                float leadTargetAngle = Vector3.Angle(toTargetVector, adjustedLeadTargetPos - transform.position);
                float amount = Mathf.Clamp(maxLeadTargetAngle / leadTargetAngle, 0, 1);
                Vector3 nextTargetPos = amount * adjustedLeadTargetPos + (1 - amount) * targetLeader.Target.transform.position;

                Maneuvring.TurnToward(transform, nextTargetPos, maxRotationAngles, steeringPIDController);
                engines.SetSteeringInputs(steeringPIDController.GetControlValues());
                engines.SetMovementInputs(new Vector3(0, 0, 1));
            }
            else
            {
                engines.SetSteeringInputs(Vector3.zero);
                engines.SetMovementInputs(new Vector3(0, 0, 1));
            }
        }
    }
}