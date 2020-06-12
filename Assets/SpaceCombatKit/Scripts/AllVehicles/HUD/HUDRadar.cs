using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace VSX.UniversalVehicleCombat.Radar
{
    
    /// <summary>
    /// Manages a 2D or 3D radar on a vehicle HUD.
    /// </summary>
    public class HUDRadar : HUDComponent
    {
     
        [Header("Target Information Sources")]

        [SerializeField]
        protected List<Tracker> trackers = new List<Tracker>();

        [SerializeField]
        protected List<TargetSelector> targetSelectors = new List<TargetSelector>();

        [SerializeField]
        protected List<TargetLocker> targetLockers = new List<TargetLocker>();

        [Header("Widgets")]

        [SerializeField]
        protected Transform widgetParent;

        [SerializeField]
        protected List<HUDRadarWidgetContainer> radarWidgetContainers = new List<HUDRadarWidgetContainer>();

        [Header("Settings")]

        [SerializeField]
        protected float widgetScale = 1;

        [SerializeField]
        protected bool clampToBorder = false;

        [SerializeField]
        protected float equatorRadius = 0.5f;

        [SerializeField]
        protected float scaleExponent = 1f;

        protected float currentZoom = 1;
        public float CurrentZoom { get { return currentZoom; } }

        protected float radarDisplayRange;

        protected Vector3 targetRelPos;

        [SerializeField]
        protected int maxNewTargetsEachFrame = 1;
        protected int numTargetsLastFrame;
        protected int displayedTargetCount;
        
        

        [SerializeField]
        private Color defaultWidgetColor = Color.white;

        [SerializeField]
        protected List<TeamColor> teamColors = new List<TeamColor>();

        [SerializeField]
        protected bool enable3D = false;

        [SerializeField]
        protected bool VerticalAxisZ = true;    // Whether the vertical axis is the Z axis rather than the Y axis

      
        // Called when something is changed in the inspector
        protected void OnValidate()
        {
            // Make sure that the assigned exponent is greater or equal to 1
            scaleExponent = Mathf.Max(scaleExponent, 1);
        }


        protected void Awake()
        {

            if (hudCamera == null) hudCamera = Camera.main;

            // Make sure that the distance exponent is at least 1
            scaleExponent = Mathf.Max(scaleExponent, 1);

            currentZoom = 0f;
        }


        /// <summary>
        /// Set the zoom.
        /// </summary>
        /// <param name="zoom">The zoom.</param>
        public virtual void SetZoom(float zoom)
        {
            // The zoom must be greater than 0
            currentZoom = Mathf.Max(zoom, 0);
        }


        /// <summary>
        /// Add a target selector to the HUD target boxes
        /// </summary>
        /// <param name="targetSelector">The target selector to add.</param>
        public virtual void AddTargetSelector(TargetSelector targetSelector)
        {
            if (!targetSelectors.Contains(targetSelector))
            {
                targetSelectors.Add(targetSelector);
            }
        }

        /// <summary>
        /// Remove a target selector from the HUD target boxes.
        /// </summary>
        /// <param name="targetSelector">The target selector to remove.</param>
        public virtual void RemoveTargetSelector(TargetSelector targetSelector)
        {
            int index = targetSelectors.IndexOf(targetSelector);
            if (index != -1)
            {
                targetSelectors.RemoveAt(index);
            }
        }

        /// <summary>
        /// Add a target locker to the HUD target boxes.
        /// </summary>
        /// <param name="targetLocker">The target locker to add.</param>
        public virtual void AddTargetLocker(TargetLocker targetLocker)
        {
            if (!targetLockers.Contains(targetLocker))
            {
                targetLockers.Add(targetLocker);
            }
        }

        /// <summary>
        /// Remove a target locker from the HUD target boxes.
        /// </summary>
        /// <param name="targetLocker">The target locker to remove.</param>
        public virtual void RemoveTargetLocker(TargetLocker targetLocker)
        {
            int index = targetLockers.IndexOf(targetLocker);
            if (index != -1)
            {
                targetLockers.RemoveAt(index);
            }
        }

        // Visualize a target tracked by a Tracker component
        protected virtual void Visualize(Tracker tracker, Trackable trackable)
        {
            
            targetRelPos = tracker.ReferenceTransform.InverseTransformPoint(trackable.transform.position);

            // If target is outside the radar display range and is not clamped to the border, ignore it
            if (targetRelPos.magnitude > radarDisplayRange)
            {
                if (!clampToBorder)
                {
                    return;
                }
            }

            // Get a radar widget that matches the trackable type and display it
            for (int i = 0; i < radarWidgetContainers.Count; ++i)
            {
                for (int j = 0; j < radarWidgetContainers[i].trackableTypes.Count; ++j)
                {

                    if (radarWidgetContainers[i].trackableTypes[j] == trackable.TrackableType)
                    {

                        HUDRadarWidget widget = radarWidgetContainers[i].GetNextAvailable(widgetParent);
                        
                        widget.SetColor(defaultWidgetColor); 
                        for (int k = 0; k < teamColors.Count; ++k)
                        {
                            if (teamColors[k].team == trackable.Team)
                            {
                                widget.SetColor(teamColors[k].color);
                            }
                        }

                        bool isSelected = false;
                        for (int k = 0; k < targetSelectors.Count; ++k)
                        {
                            if (targetSelectors[k].SelectedTarget == trackable)
                            {
                                isSelected = true;
                                break;
                            }
                        }
                        widget.SetSelected(isSelected);

                        Vector3 localPos;
                        if (targetRelPos.magnitude > radarDisplayRange)
                        {
                            localPos = (targetRelPos.normalized * radarDisplayRange) * (equatorRadius / radarDisplayRange);
                            
                            widget.SetIsClampedToBorder(true);
                        }
                        else
                        {

                            float amount = (targetRelPos.magnitude / radarDisplayRange);
                            amount = 1 - Mathf.Pow(1 - amount, scaleExponent);
                            localPos = (amount * equatorRadius) * targetRelPos.normalized;

                            widget.SetIsClampedToBorder(false);
                        }

                        if (VerticalAxisZ)
                        {
                            localPos = Quaternion.Euler(-90, 0, 0) * localPos;
                        }

                        widget.transform.localPosition = localPos;

                        widget.transform.localRotation = Quaternion.identity;

                        widget.UpdateRadarWidget(trackable);

                        return;
                    }
                }
            }
        }

        // Late update
        void LateUpdate()
        {
            // If not activated, clear all the radar widgets and exit
            if (!activated)
            {
                for (int i = 0; i < radarWidgetContainers.Count; ++i)
                {
                    radarWidgetContainers[i].Begin();
                    radarWidgetContainers[i].Finish();
                }
                return;
            }

            // Update the display range
            float maxRange = 0;
            for (int i = 0; i < trackers.Count; ++i)
            {
                maxRange = Mathf.Max(maxRange, trackers[i].Range);
            }
            radarDisplayRange = (1 - currentZoom) * maxRange;

            // Begin using the radar widget containers
            for (int i = 0; i < radarWidgetContainers.Count; ++i)
            {
                radarWidgetContainers[i].Begin();
            }

            // Visualize the targets
            for (int i = 0; i < trackers.Count; ++i)
            {
                for (int j = 0; j < trackers[i].Targets.Count; ++j)
                {
                   Visualize(trackers[i], trackers[i].Targets[j]);

                    // Don't add more than the specified amount of widgets per frame
                    if (displayedTargetCount - numTargetsLastFrame >= maxNewTargetsEachFrame)
                    {
                        break;
                    }
                }
            }

            numTargetsLastFrame = displayedTargetCount;

            // Finish using the radar widget containers
            for (int i = 0; i < radarWidgetContainers.Count; ++i)
            {
                radarWidgetContainers[i].Finish();
            }

        }
    }
}
