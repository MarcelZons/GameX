using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Pooling;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity event for running functions when a detonator is detonated.
    /// </summary>
    [System.Serializable]
    public class OnDetonatorDetonatedEventHandler : UnityEvent<Vector3> { }

    /// <summary>
    /// Unity event for running functions when a detonator is reset.
    /// </summary>
    [System.Serializable]
    public class OnDetonatorResetEventHandler : UnityEvent { }

    /// <summary>
    /// This class detonates an object (creates an explosion and deactivates the gameobject etc).
    /// </summary>
    public class Detonator : MonoBehaviour
    {
        
        [Header("Detonator")]

        [SerializeField]
        protected GameObject explosionPrefab;

        [SerializeField]
        protected bool usePoolManager;

        [SerializeField]
        protected float delayBeforeDeactivation = 0;

        [SerializeField]
        protected List<GameObject> visibleObjects = new List<GameObject>();

        [Header("Timed Detonation")]

        [SerializeField]
        protected bool detonateAfterLifetime = false;

        [SerializeField]
        protected float lifeTime = 1;

        protected float lifeTimeStartTime;


        [Header("Events")]

        // Detonator detonated event
        public OnDetonatorDetonatedEventHandler onDetonated;

        // Detonator reset event
        public OnDetonatorResetEventHandler onDetonatorReset;


        protected bool detonated = false;



        protected virtual void Start()
        {
            if (usePoolManager && PoolManager.Instance == null)
            {
                usePoolManager = false;
                Debug.LogWarning("Cannot pool explosions or hit effects as there isn't a PoolManager in the scene. Please add one to use pooling, or set the usePoolManager field on this component to False.");
            }
        }

        /// <summary>
        /// Do a delayed detonation of the detonator.
        /// </summary>
        /// <param name="delay">The delay before detonation.</param>
        public virtual void BeginDelayedDetonation(float delay)
        {
            StartCoroutine(DelayedDetonationCoroutine(delay));
        }

        // Coroutine to delay before detonation
        protected IEnumerator DelayedDetonationCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Detonate();
        }

        /// <summary>
        /// Detonate at the current position.
        /// </summary>
	    public virtual void Detonate()
        {
            Detonate(transform.position, Vector3.up);
        }

        /// <summary>
        /// Detonate at a raycast hit point.
        /// </summary>
        /// <param name="hit">The raycast hit information.</param>
        public virtual void Detonate(RaycastHit hit)
        {
            Detonate(hit.point, hit.normal);
        }

        /// <summary>
        /// Detonate at a world position.
        /// </summary>
        /// <param name="detonationPosition">The detonation position.</param>
        public virtual void Detonate(Vector3 detonationPosition, Vector3 detonationNormal)
        {
            if (!detonated)
            {
                detonated = true;

                // Move to the detonation position
                transform.position = detonationPosition;

                // Create an explosion
                if (explosionPrefab != null)
                {
                    if (usePoolManager)
                    {
                        PoolManager.Instance.Get(explosionPrefab, detonationPosition, Quaternion.LookRotation(detonationNormal));
                    }
                    else
                    {
                        GameObject.Instantiate(explosionPrefab, detonationPosition, Quaternion.LookRotation(detonationNormal));
                    }
                }

                // Disable all of the visible objects 
                for (int i = 0; i < visibleObjects.Count; ++i)
                {
                    visibleObjects[i].SetActive(false);
                }

                // Start a coroutine to wait before deactivation (e.g. to preserve trails for missiles)
                StartCoroutine(WaitBeforeDeactivate());

                // Invoke the detonation event 
                onDetonated.Invoke(detonationPosition);
            }
        }


        /// <summary>
        /// Reset the detonator.
        /// </summary>
        public virtual void ResetDetonator()
        {

            detonated = false;

            // Enable all of the visible objects 
            for (int i = 0; i < visibleObjects.Count; ++i)
            {
                visibleObjects[i].SetActive(true);
            }

            gameObject.SetActive(true);

            // Call event
            onDetonatorReset.Invoke();
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }

        protected virtual void OnEnable()
        {
            ResetDetonator();
            lifeTimeStartTime = Time.time;
        }

        // Coroutine for waiting before deactivation to preserve trail renderers etc.
        protected IEnumerator WaitBeforeDeactivate()
        {
            yield return new WaitForSeconds(delayBeforeDeactivation);
            gameObject.SetActive(false);
        }


        protected virtual void Update()
        {
            if (detonateAfterLifetime)
            {
                if (Time.time - lifeTimeStartTime > lifeTime)
                {
                    Detonate(transform.position, transform.up);
                }
            }
        }
    }
}