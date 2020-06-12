using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Unity event to run functions when beam state changes.
    /// </summary>
    [System.Serializable]
    public class OnBeamStateChangedEventHandler : UnityEvent<BeamState> { };

    /// <summary>
    /// Unity event to run functions when beam level changes.
    /// </summary>
    [System.Serializable]
    public class OnBeamLevelChangedEventHandler : UnityEvent<float> { };

    /// <summary>
    /// Unity event to run functions when a beam hits a collider.
    /// </summary>
    [System.Serializable]
    public class OnBeamControllerHitDetectedEventHandler : UnityEvent<RaycastHit> { };

    /// <summary>
    /// Unity event to run functions when a beam stops hitting a collider
    /// </summary>
    [System.Serializable]
    public class OnBeamControllerHitNotDetectedEventHandler : UnityEvent { };

    

    /// <summary>
    /// Controls a beam for e.g. a weapon.
    /// </summary>
    public class BeamController : MonoBehaviour
    {

        [Header("Beam Parameters")]

        [SerializeField]
        protected LineRenderer beamLineRenderer;

        [SerializeField]
        protected BeamHitEffectController beamHitEffectController;

        [SerializeField]
        protected Transform beamSpawn;

        [SerializeField]
        protected float range = 1000;

        protected BeamState currentBeamState = BeamState.Off;

        protected float beamStateStartTime = 0;

        protected float beamLevel = 0;

        protected bool firing = false;

        [Header("Fade In/Out")]

        [SerializeField]
        protected float beamFadeInTime = 0.33f;

        [SerializeField]
        protected AnimationCurve beamFadeInCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        protected float beamFadeOutTime = 0.33f;

        [SerializeField]
        protected AnimationCurve beamFadeOutCurve = AnimationCurve.Linear(0, 1, 1, 0);

        [Header("Pulse")]

        [SerializeField]
        protected bool isPulsed = false;

        [SerializeField]
        protected float beamSustainTime = 0; // for pulsed beam

        [SerializeField]
        protected float beamOffTime = 0;

        [Header("Beam Controller Events")]

        // Beam level changed unity event
        public OnBeamStateChangedEventHandler onBeamStateChanged;

        // Beam level changed unity event
        public OnBeamLevelChangedEventHandler onBeamLevelChanged;

        // Beam hit detected unity event
        public OnBeamControllerHitDetectedEventHandler onHitDetected;

        // Beam hit not detected unity event
        public OnBeamControllerHitNotDetectedEventHandler onHitNotDetected;

        protected Transform rootTransform;


        // Called when scene starts
        protected virtual void Start()
        {

            if (rootTransform == null) rootTransform = transform.root;

            SetBeamLevel(0);
            if (beamHitEffectController != null)
            {
                beamHitEffectController.SetActivation(false);
            }
        }


        public void SetRootTransform(Transform rootTransform)
        {
            this.rootTransform = rootTransform;
        }


        // Set the beam state
        protected virtual void SetBeamState(BeamState newBeamState)
        {

            switch (newBeamState)
            {
                case BeamState.FadingIn:

                    currentBeamState = BeamState.FadingIn;
                    beamStateStartTime = Time.time - beamLevel * beamFadeInTime;    // Assume linear fade in/out
                    break;

                case BeamState.FadingOut:

                    currentBeamState = BeamState.FadingOut;
                    beamStateStartTime = Time.time - (1 - beamLevel) * beamFadeOutTime;     // Assume linear fade in/out
                    break;

                case BeamState.Sustaining:

                    currentBeamState = BeamState.Sustaining;
                    beamStateStartTime = Time.time;
                    break;

                case BeamState.Off:

                    currentBeamState = BeamState.Off;
                    beamStateStartTime = Time.time;
                    break;

            }

            onBeamStateChanged.Invoke(newBeamState);
        }


        // Do a hit scan
        protected virtual bool DoHitScan()
        {

            // Raycast
            RaycastHit[] hits;
            hits = Physics.RaycastAll(beamSpawn.position, beamSpawn.forward, range);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));    // Sort by distance

            for (int i = 0; i < hits.Length; ++i)
            {

                bool isSelf = false;
                DamageReceiver damageReceiver = hits[i].collider.GetComponent<DamageReceiver>();
                if (rootTransform != null && damageReceiver != null && damageReceiver.RootTransform == rootTransform)
                {
                    isSelf = true;
                }
                
                if (!isSelf && !hits[i].collider.isTrigger)
                {

                    // Set beam to stop at hit point
                    beamLineRenderer.SetPosition(0, Vector3.zero);
                    beamLineRenderer.SetPosition(1, beamSpawn.transform.InverseTransformPoint(hits[i].point));

                    // Update hit effect
                    if (beamHitEffectController != null)
                    {
                        beamHitEffectController.SetActivation(true);
                        beamHitEffectController.OnHit(hits[i]);
                    }

                    onHitDetected.Invoke(hits[i]);
                    return true;
                }
            }
            
            // Set beam to stop at full range
            beamLineRenderer.SetPosition(0, Vector3.zero);
            beamLineRenderer.SetPosition(1, Vector3.forward * range);

            // Disable hit effect
            if (beamHitEffectController != null) beamHitEffectController.SetActivation(false);

            onHitNotDetected.Invoke();

            return false;
        }


        /// <summary>
        /// Set the beam level.
        /// </summary>
        /// <param name="level">Beam level.</param>
        public virtual void SetBeamLevel(float level)
        {
            beamLevel = Mathf.Clamp(level, 0, 1);

            // Set the color
            if (beamLineRenderer.material.HasProperty("_TintColor"))
            {
                Color c = beamLineRenderer.material.GetColor("_TintColor");
                c.a = level;
                beamLineRenderer.material.SetColor("_TintColor", c);
            }            

            // Update hit effect
            if (beamHitEffectController != null)
            {
                beamHitEffectController.SetLevel(level);
            }

            // Call event
            onBeamLevelChanged.Invoke(level);
        }


        /// <summary>
        /// Start triggering the beam.
        /// </summary>
        public virtual void StartTriggering()
        {
            if (!firing) SetBeamState(BeamState.FadingIn);
            firing = true;
        }


        /// <summary>
        /// Stop triggering the beam.
        /// </summary>
        public virtual void StopTriggering()
        {
            if (firing) SetBeamState(BeamState.FadingOut);
            firing = false;
        }

        public virtual void TriggerOnce()
        {
            if (!firing) SetBeamState(BeamState.FadingIn);
        }


        protected virtual void LateUpdate()
        {
            
            // Handle beam transitions
            switch (currentBeamState)
            {
                case BeamState.FadingIn:

                    float fadeInAmount = (Time.time - beamStateStartTime) / beamFadeInTime;
                    if (fadeInAmount > 1)
                    {
                        SetBeamLevel(1);
                        SetBeamState(BeamState.Sustaining);
                    }
                    SetBeamLevel(Mathf.Clamp(fadeInAmount, 0, 1));
                    break;

                case BeamState.FadingOut:

                    float fadeOutAmount = (Time.time - beamStateStartTime) / beamFadeOutTime;
                    if (fadeOutAmount > 1)
                    {
                        SetBeamLevel(0);
                        SetBeamState(BeamState.Off);
                        if (beamHitEffectController != null) beamHitEffectController.SetActivation(false);
                    }
                    else
                    {
                        SetBeamLevel(Mathf.Clamp(1 - fadeOutAmount, 0, 1));
                    }
                    break;
                case BeamState.Sustaining:
                    if (isPulsed)
                    {
                        float sustainAmount = 1 - (Time.time - beamStateStartTime) / beamSustainTime;
                        if (sustainAmount < 0)
                        {
                            SetBeamState(BeamState.FadingOut);
                        }
                    }
                    else
                    {
                        SetBeamLevel(1);
                    }
                    break;
                case BeamState.Off:
                    if (isPulsed && firing)
                    {
                        float offAmount = (Time.time - beamStateStartTime) / beamOffTime;
                        if (offAmount > 1)
                        {
                            SetBeamState(BeamState.FadingIn);
                        }
                    }
                    break;


            }

            if (currentBeamState != BeamState.Off)
            {
                DoHitScan();
            }
        }
    }
}