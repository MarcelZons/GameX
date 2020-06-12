﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// An enum that represents all the different ways that target boxes can be displayed when off screen.
    /// </summary>
    public enum TargetBoxOffScreenMode
    {
        ClampedToBorder,
        CenterRadial
    }

    /// <summary>
    /// Manages the display of target boxes on the HUD.
    /// </summary>
    public class HUDTargetBoxes : HUDComponent
    {

        [Header("Target Information Sources")]

        // All the target trackers to read from
        [SerializeField]
        protected List<Tracker> trackers = new List<Tracker>();

        [SerializeField]
        protected List<TargetSelector> targetSelectors = new List<TargetSelector>();

        [SerializeField]
        protected List<TargetLocker> targetLockers = new List<TargetLocker>();

        [SerializeField]
        protected List<TargetLeader> targetLeaders = new List<TargetLeader>();

        [Header("Target Boxes")]

        [SerializeField]
        protected List<HUDTargetBoxContainer> targetBoxContainers = new List<HUDTargetBoxContainer>();

        protected int numTargetBoxesByType = 0;

        [Header("Display Parameters")]

        [SerializeField]
        protected Canvas canvas;
        protected RectTransform canvasRectTransform;
        protected CanvasScaler canvasScaler;

        [SerializeField]
        protected bool useMeshBoundsCenter = true;

        [SerializeField]
        protected Vector2 UIViewportCoefficients = new Vector2(1, 1);

        [SerializeField]
        protected TargetBoxOffScreenMode targetBoxOffScreenMode;

        [SerializeField]
        protected float offscreenArrowsViewportSpaceRadius = 0.2f;

        [Header("World Space Parameters")]

        [SerializeField]
        protected float worldSpaceDistanceFromCamera = 1;

        [SerializeField]
        protected bool useTargetWorldPosition = false;

        protected bool isWorldSpace;

        protected float worldSpaceScaleCoefficient = 1;

        [Header("Presentation")]

        [SerializeField]
        protected Color defaultColor = Color.white;

        [SerializeField]
        protected List<TeamColor> teamColors = new List<TeamColor>();

        protected Vector3[] extentsCornersArray;    // Temporarily used to store information about the 8 corners of the target bounding box when calculating target box size

        // Information on the current view rect in viewport, screen and canvas space
        protected Rect viewportRect = new Rect();
        protected Rect screenRect = new Rect();
        protected Rect canvasRect = new Rect();


        protected void Awake()
        {
          
            if (hudCamera == null) hudCamera = Camera.main;

            extentsCornersArray = new Vector3[8];

            canvasRectTransform = canvas.GetComponent<RectTransform>();
            canvasScaler = canvas.GetComponent<CanvasScaler>();
            
            UpdateWorldSpaceScaleCoefficient();

        }

        // Update the cached information about the screen that is used to display the target boxes
        protected void UpdateScreenParameters()
        {
            // Update the viewport rect
            viewportRect.size = UIViewportCoefficients;
            viewportRect.center = Vector2.zero;

            // Update the screen rect
            screenRect.size = Vector2.Scale(viewportRect.size, new Vector2(Screen.width, Screen.height));
            screenRect.center = Vector2.zero;

            // Update the canvas rect
            canvasRect.size = Vector2.Scale(viewportRect.size, new Vector2(canvasRectTransform.sizeDelta.x, canvasRectTransform.sizeDelta.y));
            canvasRect.center = Vector2.zero;           
         
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
        /// Add a target leader to the HUD target boxes.
        /// </summary>
        /// <param name="targetLeader">The target leader to add.</param>
        public virtual void AddTargetLeader(TargetLeader targetLeader)
        {
            if (!targetLeaders.Contains(targetLeader))
            {
                targetLeaders.Add(targetLeader);
            }
        }

        /// <summary>
        /// Remove a target leader from the HUD target boxes.
        /// </summary>
        /// <param name="targetLeader">The target leader to remove.</param>
        public virtual void RemoveTargetLeader(TargetLeader targetLeader)
        {
            int index = targetLeaders.IndexOf(targetLeader);
            if (index != -1)
            {
                targetLeaders.RemoveAt(index);
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


        /// <summary>
        /// Called to update this HUD component.
        /// </summary>
        public override void OnUpdateHUD()
        {

            if (hudCamera == null) return;

            isWorldSpace = canvas.renderMode == RenderMode.WorldSpace;

            UpdateWorldSpaceScaleCoefficient();

            UpdateScreenParameters();

            // Begin using target box containers
            for (int i = 0; i < targetBoxContainers.Count; ++i)
            {
                targetBoxContainers[i].Begin();
            }
            
            if (activated)
            {
                // For each of the targets, look for a compatible target box and display it.
                for (int i = 0; i < trackers.Count; ++i)
                {
                    for (int j = 0; j < trackers[i].Targets.Count; ++j)
                    {
                        for (int k = 0; k < targetBoxContainers.Count; ++k)
                        {
                            for (int l = 0; l < targetBoxContainers[k].trackableTypes.Count; ++l)
                            {
                                if (targetBoxContainers[k].trackableTypes[l] == trackers[i].Targets[j].TrackableType)
                                {
                                    ShowTargetBox(trackers[i], trackers[i].Targets[j]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            
            
            // Finish using the target box containers
            for (int i = 0; i < targetBoxContainers.Count; ++i)
            {
                targetBoxContainers[i].Finish();
            }
        }


        // Calculate the scale coefficient that is used to convert a target widget that is sized for a 
        // screen overlay canvas to world space.
        protected void UpdateWorldSpaceScaleCoefficient()
        {
            worldSpaceScaleCoefficient = canvasScaler.matchWidthOrHeight * (1 / canvasScaler.referenceResolution.y) +
                                        (1 - canvasScaler.matchWidthOrHeight) * (1 / canvasScaler.referenceResolution.x);
        }

        protected Vector2 GetWorldSpaceSize(Vector2 viewportSize, float distanceFromCamera)
        {

            // Get the world space screen width and height at the specified distance from the camera
            float opposite = Mathf.Tan(hudCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distanceFromCamera;
            float worldSpaceScreenHeight = opposite * 2;
            float worldSpaceScreenWidth = hudCamera.aspect * worldSpaceScreenHeight;
            
            return (((1 / distanceFromCamera) / worldSpaceScaleCoefficient) * new Vector2(worldSpaceScreenWidth * viewportSize.x, worldSpaceScreenHeight * viewportSize.y));

        }

        /// <summary>
        /// Align an element on the HUD to a position in world space.
        /// </summary>
        /// <param name="hudElement">The HUD element.</param>
        /// <param name="centeredViewportPosition">The viewport position with (0,0) at the center.</param>
        /// <param name="addedRotation">The added rotation to apply to the HUD element.</param>
        /// <param name="zDistance">The z distance to put the HUD element.</param>
        /// <param name="alignRelative">Whether to align the element locally to another RectTransform.</param>
        /// <param name="relativeReference">The Rect Transform to align relative to.</param>
        protected void AlignHUDElement(RectTransform hudElement, Vector3 centeredViewportPosition, Quaternion addedRotation, float zDistance = 0, bool alignRelative = false, Vector3 relativeReference = new Vector3())
        {
            // Align in screen space overlay            
            if (!isWorldSpace)
            {
                if (alignRelative) centeredViewportPosition = centeredViewportPosition - relativeReference;
                hudElement.anchoredPosition = Vector3.Scale(centeredViewportPosition, canvasRectTransform.sizeDelta);
                hudElement.rotation = addedRotation;   
            }
            // Align in world space 
            else
            {
                hudElement.position = hudCamera.ViewportToWorldPoint(new Vector3(centeredViewportPosition.x + 0.5f, centeredViewportPosition.y + 0.5f, zDistance));
                hudElement.rotation = Quaternion.LookRotation(hudElement.position - hudCamera.transform.position, hudCamera.transform.up);
                hudElement.rotation *= addedRotation;
            }
        }


        /// <summary>
        /// Show a target box for a given target tracked by a given trackable.
        /// </summary>
        /// <param name="tracker"></param>
        /// <param name="trackable"></param>
        void ShowTargetBox(Tracker tracker, Trackable trackable)
        {

            // Get a target box
            HUDTargetBox targetBox = null;
            for (int i = 0; i < targetBoxContainers.Count; ++i)
            {
                for (int j = 0; j < targetBoxContainers[i].trackableTypes.Count; ++j)
                {
                    if (targetBoxContainers[i].trackableTypes[j] == trackable.TrackableType)
                    {
                        targetBox = targetBoxContainers[i].GetNextAvailable(canvas.transform);
                    }
                }
            }
            
            // Exit if no target box found
            if (targetBox == null) return;

            // Update whether the target is selected by a target selector
            bool isSelectedTarget = false;
            for (int i = 0; i < targetSelectors.Count; ++i)
            {
                if (targetSelectors[i].SelectedTarget == trackable)
                {
                    isSelectedTarget = true;
                    break;
                }
            }
            targetBox.SetIsSelectedTarget(isSelectedTarget);

            // Update world space target box scale (when switching from screen to world space, the target box is simply
            // scaled down to the equivalent world space size relative to the distance from the camera).
            float distanceFromCamera = 0;
            if (isWorldSpace)
            {
                distanceFromCamera = worldSpaceDistanceFromCamera;
                if (useTargetWorldPosition)
                {
                    distanceFromCamera = Vector3.Distance(hudCamera.transform.position, trackable.transform.position);
                }

                float scale = worldSpaceScaleCoefficient * distanceFromCamera;
                targetBox.transform.localScale = new Vector3(scale, scale, scale);
            }

            // Get world space target position
            Vector3 position = trackable.transform.position;
            if (useMeshBoundsCenter)
            {
                position = trackable.transform.TransformPoint(trackable.TrackingBounds.center);
            }
           
            // Get centered viewport position
            Vector3 centeredViewportPos = HUDTargetBoxesFunctions.GetViewportPosition(position, hudCamera, true);
            
            // Update whether the target is in view
            bool isInView = viewportRect.Contains(centeredViewportPos) && centeredViewportPos.z > 0;
            targetBox.SetIsInView(isInView);

            // Update the distance
            targetBox.SetDistance(Vector3.Distance(trackable.transform.position, tracker.transform.position));

            // If not in view, calculate screen border or arrow radial position
            float targetBoxAngle = 0;
            if (!isInView)
            {
                switch (targetBoxOffScreenMode)
                {
                    case TargetBoxOffScreenMode.ClampedToBorder:
                        centeredViewportPos = HUDTargetBoxesFunctions.ClampToBorder(centeredViewportPos, viewportRect, out targetBoxAngle);
                        break;

                    case TargetBoxOffScreenMode.CenterRadial:

                        Vector2 clampedCenteredViewportPosition = new Vector2(centeredViewportPos.x, centeredViewportPos.y).normalized * offscreenArrowsViewportSpaceRadius;
                        
                        centeredViewportPos.x = clampedCenteredViewportPosition.x * (1 / hudCamera.aspect);
                        centeredViewportPos.y = clampedCenteredViewportPosition.y;

                        targetBoxAngle = Mathf.Atan2(centeredViewportPos.y, centeredViewportPos.x) * Mathf.Rad2Deg;

                        break;

                }
            }
           
            // Update position, rotation and scale of the target box depending on the render mode of the canvas 
            AlignHUDElement(targetBox.RectTransform, centeredViewportPos, Quaternion.Euler(0f, 0f, targetBoxAngle), distanceFromCamera);

            // Update the size of the target box
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                Vector2 viewportSize = HUDTargetBoxesFunctions.GetViewportSize(trackable, extentsCornersArray, hudCamera, viewportRect);
                targetBox.SetSize(GetWorldSpaceSize(viewportSize, distanceFromCamera));
            }
            else
            {
                targetBox.SetSize(Vector2.Scale(HUDTargetBoxesFunctions.GetViewportSize(trackable, extentsCornersArray, hudCamera, canvasRect), canvasRectTransform.sizeDelta));
            }

            // Update the color of the target box
            targetBox.SetColor(defaultColor);
            for (int i = 0; i < teamColors.Count; ++i)
            {
                if (teamColors[i].team == trackable.Team)
                {
                    targetBox.SetColor(teamColors[i].color);
                }
            }
           
            // Update the target locks 
            for (int i = 0; i < targetLockers.Count; ++i)
            {
                if (targetLockers[i].Target == trackable)
                {
                    targetBox.AddLock(targetLockers[i]);
                }
            }

            // Update the lead target info for the target box
            for (int i = 0; i < targetLeaders.Count; ++i)
            {
                if (targetLeaders[i].Target != null && targetLeaders[i].Target == trackable)
                {

                    HUDTargetBox_LeadTargetBoxController leadTargetBoxController = targetBox.GetLeadTargetBox();

                    if (leadTargetBoxController != null)
                    {
                        Vector3 viewportPos = HUDTargetBoxesFunctions.GetViewportPosition(targetLeaders[i].LeadTargetPosition, hudCamera, true);
                        AlignHUDElement(leadTargetBoxController.Box.rectTransform, viewportPos, Quaternion.identity, distanceFromCamera, true, centeredViewportPos);
                        leadTargetBoxController.UpdateLeadTargetBox();
                    }

                }
            }
            
            targetBox.UpdateTargetBox(trackable);
        }
    }
}


