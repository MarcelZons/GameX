using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;

namespace VSX.UniversalVehicleCombat
{
    public class RadarAudioController : MonoBehaviour
    {

        [Header("Selected Target Changed")]

        [SerializeField]
        protected AudioSource selectedTargetChangedAudio;

        [Header("Hostile Alarm")]

        [SerializeField]
        protected List<Team> hostileTeams = new List<Team>();

        [SerializeField]
        protected AudioSource hostileTeamDetectedAudio;

        [SerializeField]
        protected float hostileAlarmDelay = 0.25f;

        protected int numHostilesTracked = 0;

        [Header("Target Locking")]

        [SerializeField]
        protected AudioSource lockingAudioSource;

        [SerializeField]
        protected AudioSource lockedAudioSource;

        protected List<TargetLocker> targetLockers = new List<TargetLocker>();


        /// <summary>
        /// Called when a new target is tracked.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        public virtual void OnStartedTrackingTarget(Trackable target)
        {
            
            if (hostileTeams.Contains(target.Team))
            {
                // If a hostile is not currently detected, raise the alarm
                if (numHostilesTracked == 0)
                {
                    if (hostileTeamDetectedAudio != null) hostileTeamDetectedAudio.PlayDelayed(hostileAlarmDelay);
                }

                numHostilesTracked += 1;
            }
        }


        /// <summary>
        /// Called when a target stops being tracked.
        /// </summary>
        /// <param name="newTarget">The untracked target</param>
        public void OnStoppedTrackingTarget(Trackable target)
        {
            // If the untracked target is hostile, reduce the count of hostiles being tracked
            if (hostileTeams.Contains(target.Team))
            {
                numHostilesTracked -= 1;
            }
        }


        /// <summary>
        /// Play the locked audio source
        /// </summary>
        public virtual void PlayLockedAudio()
        {
            if (lockedAudioSource != null) lockedAudioSource.Play();
        }


        /// <summary>
        /// Called when a target locked is loaded on the vehicle.
        /// </summary>
        /// <param name="targetLocker">The loaded target locker.</param>
        public virtual void OnTargetLockerLoaded(TargetLocker targetLocker)
        {
            targetLocker.onLocked.AddListener(delegate { PlayLockedAudio(); });
            targetLockers.Add(targetLocker);
        }


        /// <summary>
        /// Called when a target locker is unloaded from the vehicle.
        /// </summary>
        /// <param name="targetLocker">The unloaded target locker.</param>
        public virtual void OnTargetLockerUnloaded(TargetLocker targetLocker)
        {
            targetLocker.onLocked.RemoveListener(delegate { PlayLockedAudio(); });
            targetLockers.Remove(targetLocker);
        }


        public virtual void OnSelectedTargetChanged(Trackable newTarget)
        {
            if (selectedTargetChangedAudio != null && newTarget != null)
            {
                selectedTargetChangedAudio.Play();
            }
        }


        protected virtual void Update()
        {
            // Do target locking audio
            if (lockingAudioSource != null)
            {
                // Check if any of the target lockers are currently locking
                bool foundLocking = false;
                for (int i = 0; i < targetLockers.Count; ++i)
                {
                    if (targetLockers[i].LockState == LockState.Locking)
                    {
                        foundLocking = true;
                        break;
                    }
                }

                // If locking state has been found, play the locking audio
                if (foundLocking)
                {
                    if (!lockingAudioSource.isPlaying)
                    {
                        lockingAudioSource.Play();
                    }
                }
                // else stop the locking audio
                else
                {
                    if (lockingAudioSource.isPlaying)
                    {
                        lockingAudioSource.Stop();
                    }
                }
            } 
        }
    }
}
