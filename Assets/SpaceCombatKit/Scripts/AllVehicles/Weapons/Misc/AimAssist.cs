using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    public class AimAssist : MonoBehaviour
    {

        public Transform target;
        protected Transform currentTarget;

        public Transform angleReferenceTransform;
        public Transform aimTransform;

        public bool useTargetLeader = true;
        public TargetLeader targetLeader;

        
        
        public float aimAssistAngle;


        void UpdateAimAssist()
        {
            if (useTargetLeader)
            {
                if (targetLeader != null && targetLeader.Target != null)
                {
                    currentTarget = targetLeader.Target.transform;
                }
                else
                {
                    currentTarget = null;
                }
            }
            else
            {
                currentTarget = target;
            }

            if (currentTarget != null)
            {
                float angle = Vector3.Angle(angleReferenceTransform.forward, targetLeader.LeadTargetPosition - angleReferenceTransform.position);

                if (angle < aimAssistAngle)
                {
                    aimTransform.LookAt(targetLeader.LeadTargetPosition, angleReferenceTransform.up);
                    Debug.DrawLine(aimTransform.position, aimTransform.position + aimTransform.forward * 1000, Color.green);
                }
                else
                {
                    aimTransform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                aimTransform.localRotation = Quaternion.identity;
            }
        }

        private void Update()
        {
            UpdateAimAssist();
        }
    }
}