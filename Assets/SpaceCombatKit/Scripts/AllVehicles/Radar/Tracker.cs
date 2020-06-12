using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Unity event to run functions when a Tracker starts tracking a trackable.
    /// </summary>
    [System.Serializable]
    public class OnTrackerStartedTrackingEventHandler : UnityEvent<Trackable> { }

    /// <summary>
    /// Unity event to run functions Tracker stops tracking a trackable
    /// </summary>
    [System.Serializable]
    public class OnTrackerStoppedTrackingEventHandler : UnityEvent<Trackable> { }

    /// <summary>
    /// Unity event to run functions when a Tracker's trackables list is updated
    /// </summary>
    [System.Serializable]
    public class OnTrackerTrackablesUpdatedEventHandler : UnityEvent { }

    /// <summary>
    /// Tracks targets in the scene according to specified tracking parameters.
    /// </summary>
    public class Tracker : ModuleManager
    {

        [Header("Tracking Parameters")]
        [Tooltip("The reference transform for tracking targets.")]
        [SerializeField]
        protected Transform referenceTransform;
        public Transform ReferenceTransform { get { return referenceTransform; } }

        [Tooltip("The tracking range.")]
        [SerializeField]
        protected float range = 5000;
        public float Range { get { return range; } }

        [Tooltip("The teams that this tracker can track.")]
        [SerializeField]
        protected List<Team> trackableTeams = new List<Team>();

        // The trackable types that this tracker can track
        [Tooltip("The types of trackable that this tracker can track.")]
        [SerializeField]
        protected List<TrackableType> trackableTypes = new List<TrackableType>();

        [Tooltip("Whether to update targets every frame. Can be left unchecked for manually updating for performance reasons.")]
        [SerializeField]
        protected bool updateTargetsEveryFrame = true;

        [Tooltip("The root transform of the tracker. Used to prevent tracking of self.")]
        [SerializeField]
        protected Transform rootTransform;
        public Transform RootTransform
        {
            get { return rootTransform; }
            set { rootTransform = value; }
        }

        // A tracking state for determining when a trackable starts being tracked or stops being tracked by this tracker.
        protected enum TrackingState
        {
            Tracked,
            WasTracked,
            NotTracked
        }

        // The trackables currently being tracked by this tracker
        protected List<Trackable> targets = new List<Trackable>();
        public List<Trackable> Targets { get { return targets; } }

        // A list of tracking states for all the trackables in the scene (index corresponds to trackable ID)
        protected List<TrackingState> trackingStatesByID = new List<TrackingState>();
        
        [Header("Events")]

        // Started tracking trackable event
        public OnTrackerStartedTrackingEventHandler onStartedTracking;

        // Stopped tracking trackable event
        public OnTrackerStoppedTrackingEventHandler onStoppedTracking;

        // Trackables updated event
        public OnTrackerTrackablesUpdatedEventHandler onTrackablesListUpdated;



        protected override void Awake()
        {

            base.Awake();

            if (TrackableSceneManager.Instance == null)
            {
                Debug.LogWarning("No TrackableSceneManager component found in scene, please add one to enable this radar to track targets.");
            }
            else
            {
                for (int i = 0; i < TrackableSceneManager.Instance.Trackables.Count; ++i)
                {
                    OnTrackableRegistered(TrackableSceneManager.Instance.Trackables[i]);
                }

                // Listen for new trackables being registered in the scene
                TrackableSceneManager.Instance.onTrackableRegistered.AddListener(OnTrackableRegistered);

                // Listen for trackables being unregistered in the scene
                TrackableSceneManager.Instance.onTrackableUnregistered.AddListener(OnTrackableUnregistered);
            }
        }


        // Called when the component is first added to a gameobject or reste in the inspector
        protected virtual void Reset()
        {
            // Track all teams
            trackableTeams = new List<Team>((Team[])Enum.GetValues(typeof(Team)));

            // Track all types
            trackableTypes = new List<TrackableType>((TrackableType[])Enum.GetValues(typeof(TrackableType)));

            this.referenceTransform = transform;

            this.rootTransform = transform.root;

        }


        // Called when a trackable is registered in the scene
        protected virtual void OnTrackableRegistered(Trackable trackable)
        {
            // Keep track of its tracking state
            trackingStatesByID.Add(TrackingState.NotTracked);
        }


        // Called when a trackable is destroyed in the scene
        protected virtual void OnTrackableUnregistered(Trackable trackable)
        {
            if (trackingStatesByID[trackable.TrackableID] != TrackingState.NotTracked)
            {
                int index = Targets.IndexOf(trackable);
                if (index != -1)
                {
                    Targets.RemoveAt(index);
                    OnStoppedTracking(trackable);
                }
            }
        }


        // Called to check if a target is trackable by this radar
        public virtual bool IsTrackable(Trackable target)
        {
            
            // Check distance
            if (!target.IgnoreTrackingDistance && Vector3.Distance(target.transform.position, transform.position) > range) return false;

            // Check if this tracker can select the team
            bool found = false;
            for (int i = 0; i < trackableTeams.Count; ++i)
            {
                if (trackableTeams[i] == target.Team)
                {
                    found = true;
                }
            }
            if (!found) return false;
            
            // Check if this target can select the type
            found = false;
            for (int i = 0; i < trackableTypes.Count; ++i)
            {
                if (trackableTypes[i] == target.TrackableType)
                {
                    found = true;
                }
            }
            if (!found) return false;
            
            if (this.rootTransform != null && target.RootTransform == this.rootTransform)
            {
                return false;
            }
            
            return true;

        }


        // Called to update the list of tracked targets
        protected virtual void UpdateTrackedTargetsList()
        {
            // Update targets list
            if (TrackableSceneManager.Instance != null) TrackableSceneManager.Instance.GetTrackables(this);

            // Look for targets that weren't being tracked before
            for (int i = 0; i < targets.Count; ++i)
            {

                if (trackingStatesByID[targets[i].TrackableID] == TrackingState.NotTracked)
                {
                    // Call the function to do something with the newly tracked target
                    OnStartedTracking(targets[i]);
                }

                // Update the tracking state
                trackingStatesByID[targets[i].TrackableID] = TrackingState.Tracked;

            }
            
            // Look for targets that aren't being tracked any more
            for (int i = 0; i < trackingStatesByID.Count; ++i)
            {
                if (trackingStatesByID[i] == TrackingState.WasTracked)
                {
                    // Call the function to do something with the target that isn't being tracked anymore
                    OnStoppedTracking(TrackableSceneManager.Instance.GetTrackableByID(i));
                    trackingStatesByID[i] = TrackingState.NotTracked;
                }
                // Push the state of newly tracked trackables into past tense
                else if (trackingStatesByID[i] == TrackingState.Tracked)
                {
                    trackingStatesByID[i] = TrackingState.WasTracked;
                }
            }
            
            onTrackablesListUpdated.Invoke();
        }

        // Called when this tracker first starts tracking a target
        protected virtual void OnStartedTracking(Trackable trackable)
        {
            onStartedTracking.Invoke(trackable);
        }

        // Called when this tracker stops tracking a target
        protected virtual void OnStoppedTracking(Trackable trackable)
        {
            onStoppedTracking.Invoke(trackable);
        }


        protected override void OnModuleMounted(Module module)
        {
            base.OnModuleMounted(module);

            Trackable moduleTrackable = module.GetComponent<Trackable>();
            Trackable thisTrackable = GetComponent<Trackable>();
            if (thisTrackable != null && moduleTrackable != null)
            {
                thisTrackable.AddChildTrackable(moduleTrackable);
            }
        }


        // Called every frame
        protected virtual void Update()
        {
            if (updateTargetsEveryFrame)
            {
                UpdateTrackedTargetsList();
            }
        }
    }
}
