using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Unity event to run functions when a target selector's selected target changes.
    /// </summary>
    [System.Serializable]
    public class OnSelectedTargetChangedEventHandler : UnityEvent<Trackable> { }

    /// <summary>
    /// Select a target from a list of trackables.
    /// </summary>
    public class TargetSelector : MonoBehaviour
    {
        /// The list of trackables this selector is working with.
        protected List<Trackable> trackables = new List<Trackable>();

        [Header("Selection Criteria")]

        // The teams that can be selected
        [SerializeField]
        protected List<Team> selectableTeams = new List<Team>();

        // The types that can be selected
        [SerializeField]
        protected List<TrackableType> selectableTypes = new List<TrackableType>();

        // The maximum depth that can be selected
        [SerializeField]
        protected float maxDepth = 0;

        // Always look for the highest depth child
        [SerializeField]
        protected bool selectHighestDepthChild = false;

        [Header("General")]

        // Whether or not to automatically scan for a target when none is selected
        [SerializeField]
        protected bool scanEveryFrame = true;

        [SerializeField]
        protected float frontTargetAngle = 10;

        [SerializeField]
        protected Transform frontTargetReference;

        [SerializeField]
        protected bool callSelectEventOnTarget = true;

        protected Trackable selectedTarget;
        public Trackable SelectedTarget { get { return selectedTarget; } }

        [Header("Events")]

        // Selected target changed event
        public OnSelectedTargetChangedEventHandler onSelectedTargetChanged;


        // Called when the component is first added to a gameobject, or the component is reset in the inspector
        protected virtual void Reset()
        {

            // Select all teams
            selectableTeams = new List<Team>((Team[])System.Enum.GetValues(typeof(Team)));

            // Select all types
            selectableTypes = new List<TrackableType>((TrackableType[])System.Enum.GetValues(typeof(TrackableType)));

            frontTargetReference = transform;
            
        }

        // Get the index of the currently selected target in the list
        protected virtual int GetSelectedTargetIndex()
        {

            if (selectedTarget == null) return -1;

            for (int i = 0; i < trackables.Count; ++i)
            {
                if (trackables[i] == selectedTarget)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Select the first selectable target.
        /// </summary>
        public virtual void SelectFirstSelectableTarget()
        {
            for (int i = 0; i < trackables.Count; ++i)
            {
                if (IsSelectable(trackables[i]))
                {
                    Select(trackables[i]);
                    return;
                }
            }

            if (selectedTarget != null) Select(null);
        }


        // Check if a target is selectable
        protected virtual bool IsSelectable(Trackable target)
        {
            // Check if the team is selectable
            bool teamFound = false;
            for (int i = 0; i < selectableTeams.Count; ++i)
            {
                if (selectableTeams[i] == target.Team)
                {
                    teamFound = true;
                    break;
                }
            }
            if (!teamFound) return false;

            // Check if the type is selectable 
            bool typeFound = false;
            for (int i = 0; i < selectableTypes.Count; ++i)
            {
                if (selectableTypes[i] == target.TrackableType)
                {
                    typeFound = true;
                    break;
                }
            }
            if (!typeFound) return false;

            // Check if the depth is selectable
            if (target.Depth > maxDepth) return false;

            return true;

        }

        /// <summary>
        /// Called when the Tracker stops tracking a target.
        /// </summary>
        /// <param name="untrackedTrackable"></param>
        public virtual void OnStoppedTracking(Trackable trackable)
        {
            if (trackable == selectedTarget)
            {
                Select(null);
            }
        }
        

        // Select a target
        public virtual void Select(Trackable newSelectedTarget)
        {

            if (newSelectedTarget == selectedTarget) return;

            // Unselect the currently selected target
            if (selectedTarget != null)
            {
                selectedTarget.Unselect();
            }

            if (newSelectedTarget != null)
            {
                // If toggled, select the highest depth child in the hierarchy.
                if (selectHighestDepthChild)
                {
                    for (int i = 0; i < 1000; ++i)
                    {
                        if (newSelectedTarget.ChildTrackables.Count > 0)
                        {
                            Select(newSelectedTarget.ChildTrackables[0]);
                            return;
                        }
                    }
                }
            }

            // Update the target 
            selectedTarget = newSelectedTarget;

            // Call select event on the new target
            if (selectedTarget != null && callSelectEventOnTarget)
            {
                selectedTarget.Select();
            }

            // Call the event
            onSelectedTargetChanged.Invoke(selectedTarget);

        }


        /// <summary>
        /// Cycle back or forward through the targets list.
        /// </summary>
        /// <param name="forward">Whether to cycle forward.</param>
        public virtual void Cycle(bool forward)
        {

            // Get the index of the currently selected target
            int index = GetSelectedTargetIndex();

            // If the selected target is null or doesn't exist in the list, just get the first selectable target
            if (index == -1)
            {
                SelectFirstSelectableTarget();
                return;
            }

            // Step through the targets in the specified direction looking for the next selectable one
            int direction = forward ? 1 : -1;
            for (int i = 0; i < trackables.Count; ++i)
            {

                index += direction;

                // Wrap at the end
                if (index >= trackables.Count)
                {
                    index = 0;
                }

                // Wrap at the beginning
                else if (index < 0)
                {
                    index = trackables.Count - 1;
                }

                // Select the target if it's selectable
                if (IsSelectable(trackables[index]))
                {
                    Select(trackables[index]);
                    return;
                }
            }

            if (selectedTarget != null) Select(null);

        }


        /// <summary>
        /// Select the nearest target to the tracker.
        /// </summary>
        public virtual void SelectNearest()
        {
            // Find the index of the target that is nearest
            float minDist = float.MaxValue;
            int index = -1;
            for (int i = 0; i < trackables.Count; ++i)
            {
                if (IsSelectable(trackables[i]))
                {
                    float dist = Vector3.Distance(trackables[i].transform.position, transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                }
            }

            // Select the target
            if (index != -1)
            {
                Select(trackables[index]);
            }
            else
            {
                if (selectedTarget != null) Select(null);
            }
        }


        /// <summary>
        /// Select the target closest to the front of the tracker, within a specified angle.
        /// </summary>
        public virtual void SelectFront()
        {

            float minAngle = 180;

            // Get the target that is nearest the forward vector of the tracker
            int index = -1;
            for (int i = 0; i < trackables.Count; ++i)
            {
                if (IsSelectable(trackables[i]))
                {
                    float angle = Vector3.Angle(trackables[i].transform.position - transform.position, transform.forward);

                    if (angle < minAngle && angle < frontTargetAngle)
                    {
                        minAngle = angle;
                        index = i;
                    }
                }
            }

            // Select the target
            if (index != -1)
            {
                Select(trackables[index]);
            }
        }

        // Called every frame
        protected virtual void Update()
        {
            // If toggled, always look for a new target when none is selected
            if (scanEveryFrame && selectedTarget == null)
            {
                SelectFirstSelectableTarget();
            }
        }
    }
}
