using UnityEngine;
using System.Collections;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Represents a position and rotation target for the vehicle camera, with adjustable settings.
    /// </summary>
    public class CameraViewTarget : MonoBehaviour
    {

        [Header("General")]

        [Tooltip("Determines the order that camera view targets will be accessed, affecting cycling backward/forward of camera views.")]
        [SerializeField]
        protected int sortingIndex = 0;
        public int SortingIndex { get { return sortingIndex; } }

        [Tooltip("The Vehicle Camera View value for this camera view target.")]
        [SerializeField]
        protected VehicleCameraView cameraView;
        public VehicleCameraView CameraView { get { return cameraView; } }

        [Tooltip("Whether to parent the camera to this camera view target when the view is selected.")]
        [SerializeField]
        protected bool parentCamera;
        public bool ParentCamera { get { return parentCamera; } }

        [Header("Position Settings")]

        [Tooltip("Whether to lock the camera position to this camera view target when it is selected.")]
        [SerializeField]
        private bool lockPosition;
        public bool LockPosition { get { return lockPosition; } }

        [Tooltip("How strongly the camera follows the position of this transform.")]
        [SerializeField]
        private float positionFollowStrength = 0.4f;
        public float PositionFollowStrength { get { return positionFollowStrength; } }

        [Tooltip("How much of a sideways offset occurs for the camera in proportion to the roll angular velocity of the vehicle.")]
        [SerializeField]
        private float spinOffsetCoefficient = 1f;
        public float SpinOffsetCoefficient { get { return spinOffsetCoefficient; } }

        [Header("Rotation Settings")]

        [Tooltip("Whether to lock the camera's orientation to the forward axis of this transform.")]
        [SerializeField]
        private bool lockCameraForwardVector = true;
        public bool LockCameraForwardVector { get { return lockCameraForwardVector; } }

        [Tooltip("Whether to lock the rotation of the camera to this transform.")]
        [SerializeField]
        private bool lockRotation;
        public bool LockRotation { get { return lockRotation; } }

        [Tooltip("How strongly the camera follows the rotation of this camera view target.")]
        [SerializeField]
        private float rotationFollowStrength = 0.08f;
        public float RotationFollowStrength { get { return rotationFollowStrength; } }

	}
}
