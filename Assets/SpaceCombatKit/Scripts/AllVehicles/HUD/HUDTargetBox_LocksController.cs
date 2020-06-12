using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Manages the locks for a target box displayed on the HUD.
    /// </summary>
    public class HUDTargetBox_LocksController : MonoBehaviour
    {

        [SerializeField]
        protected Text numLocksText;
        protected int numLocks;

        [SerializeField]
        protected List<HUDTargetBox_LockController> lockBoxes = new List<HUDTargetBox_LockController>();

        [SerializeField]
        protected float lockingMargin = 10;

        [SerializeField]
        protected float lockedMargin = 4;

        [SerializeField]
        protected float animationTime = 0.5f;

        protected int lastUsedIndex = -1;

        protected Coroutine resetCoroutine;

        
        /// <summary>
        /// Add a lock to the target box.
        /// </summary>
        /// <param name="targetLocker">The target locker that is locked onto the target.</param>
        public virtual void AddLock(TargetLocker targetLocker)
        {

            lastUsedIndex += 1;
            
            if (lastUsedIndex < lockBoxes.Count)
            {
                lockBoxes[lastUsedIndex].gameObject.SetActive(true);

                // Update the lock state
                switch (targetLocker.LockState)
                {
                    case LockState.NoLock:

                        lockBoxes[lastUsedIndex].gameObject.SetActive(false);
                        break;

                    case LockState.Locking:

                        lockBoxes[lastUsedIndex].gameObject.SetActive(true);
                        lockBoxes[lastUsedIndex].RectTransform.offsetMin = new Vector2(-lockingMargin, -lockingMargin);
                        lockBoxes[lastUsedIndex].RectTransform.offsetMax = new Vector2(lockingMargin, lockingMargin);
                        break;

                    case LockState.Locked:

                        lockBoxes[lastUsedIndex].gameObject.SetActive(true);
                        float amount = Mathf.Clamp((Time.time - targetLocker.LockStateChangeTime) / animationTime, 0, 1);
                        float offset = lockingMargin - amount * (lockingMargin - lockedMargin);
                        lockBoxes[lastUsedIndex].RectTransform.offsetMin = new Vector2(-offset, -offset);
                        lockBoxes[lastUsedIndex].RectTransform.offsetMax = new Vector2(offset, offset);

                        break;
                }
                numLocks += 1;
                numLocksText.text = numLocks.ToString();
            }
            else
            {
                return;
            }
        }

        protected virtual void OnEnable()
        {
            resetCoroutine = StartCoroutine(ResetLockBoxesCoroutine());
        }

        protected virtual void OnDisable()
        {
            StopCoroutine(resetCoroutine);
        }

        // Coroutine for resetting the lead target boxes at the end of the frame
        protected virtual IEnumerator ResetLockBoxesCoroutine()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                for (int i = 0; i < lockBoxes.Count; ++i)
                {
                    lockBoxes[i].Deactivate();
                }
                lastUsedIndex = -1;
                numLocks = 0;
                numLocksText.text = numLocks.ToString();
            }
        }
    }
}