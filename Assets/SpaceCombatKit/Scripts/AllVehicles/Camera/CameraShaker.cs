using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Shake the camera based on the rumble level of the RumbleManager.
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("The transform to be shaken.")]
        [SerializeField]
        public Transform cameraTransform;

        [Tooltip("The rumble manager responsible for shaking this camera.")]
        [SerializeField]
        private RumbleManager rumbleManager;

        [Header("Shake Parameters")]

        [Tooltip("The maximum shake vector length that describes the angle for the shake relative to a unit forward vector.")]
        [SerializeField]
        private float maxShakeVectorLength = 0.05f;

        
        // Update is called once per frame
        protected virtual void Update()
        {
            cameraTransform.localRotation = Quaternion.identity;

            if (rumbleManager != null)
            {
                ShakeCamera(rumbleManager.CurrentLevel);
            }
        }


        // Shake the camera once.
        protected virtual void ShakeCamera(float shakeStrength)
        {
            
            // Get a random vector on the xy plane
            Vector3 localShakeVector = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f).normalized;

            // Scale according to desired shake magnitude
            localShakeVector *= shakeStrength * maxShakeVectorLength;

            // Calculate the look target 
            Vector3 shakeLookTarget = cameraTransform.TransformPoint(Vector3.forward + localShakeVector);
            
            // Look at the target
            cameraTransform.LookAt(shakeLookTarget, transform.up);

        }      
    }
}