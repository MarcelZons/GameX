using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Unity event for running functions when a raycast hit is detected
    /// </summary>
    [System.Serializable]
    public class OnCollisionScannerHitDetectedEventHandler : UnityEvent<RaycastHit> { }

    /// <summary>
    /// This class uses a raycast from the transform's previous position to its current one to detect a hit on a collider regardless of speed.
    /// </summary>
    public class CollisionScanner : MonoBehaviour, IRootTransformUser
    {

        [Header("Settings")]

        [SerializeField]
        protected HitScanIntervalType hitScanIntervalType = HitScanIntervalType.FrameInterval;

        // Frame interval

        [SerializeField]
        protected int hitScanFrameInterval = 1;
        protected int frameCountSinceLastScan = 1;

        // Time interval

        [SerializeField]
        protected float hitScanTimeInterval;
        protected float lastHitScanTime;

        protected Vector3 lastPosition;

        [SerializeField]
        protected Transform rootTransform;    // To check for collisions with firer
        
        [Header("Events")]

        // Hit detected event
        public OnCollisionScannerHitDetectedEventHandler onHitDetected;
        
        protected bool disabled = false;


        // Reset when enabled
        private void OnEnable()
        {
            disabled = false;
            lastPosition = transform.position;
        }

        // Do a single hit scan
        protected void DoHitScan()
        {

            if (disabled) return;

            RaycastHit[] hits;

            // Scan from previous position to current position
            float scanDistance = Vector3.Distance(lastPosition, transform.position);

            // Raycast
            hits = Physics.RaycastAll(lastPosition, transform.forward, scanDistance);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));    // Sort by distance

            for (int i = 0; i < hits.Length; ++i)
            {
                bool ignore = false;

                // Ignore collisions with sender
                if (hits[i].rigidbody != null && hits[i].rigidbody.transform == rootTransform)
                {
                    ignore = true;
                }

                // Ignore trigger colliders 
                if (!ignore && !hits[i].collider.isTrigger)
                {
                    disabled = true;
                    onHitDetected.Invoke(hits[i]);
                    break;
                }
            }

            // Update the last position
            lastPosition = transform.position;

        }

        public void SetRootTransform(Transform rootTransform)
        {
            this.rootTransform = rootTransform;
        }

        /// <summary>
        /// Disable this collision scanner.
        /// </summary>
        public void SetHitScanDisabled()
        {
            disabled = true;
        }

        /// <summary>
        /// Enable this collision scanner
        /// </summary>
        public void SetHitScanEnabled()
        {
            disabled = false;
        }

        // Called every frame
        private void Update()
        {

            switch (hitScanIntervalType)
            {
                case HitScanIntervalType.FrameInterval:

                    // Check if enough frames have passed for a new scan
                    if (frameCountSinceLastScan >= hitScanFrameInterval)
                    {
                        DoHitScan();
                        frameCountSinceLastScan = 0;
                    }

                    break;

                case HitScanIntervalType.TimeInterval:

                    // Check if enough time has passed for a new scan
                    if ((Time.time - lastHitScanTime) > hitScanTimeInterval)
                    {
                        DoHitScan();
                        lastHitScanTime = Time.time;
                    }

                    break;

            }

            frameCountSinceLastScan += 1;

        }
    }
}