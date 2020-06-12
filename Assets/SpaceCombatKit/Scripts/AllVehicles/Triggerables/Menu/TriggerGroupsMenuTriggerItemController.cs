using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{

    // Unity event for running functions when a trigger item in the menu is selected.
    [System.Serializable]
    public class OnTriggerGroupsMenuTriggerItemSelectedEventHandler : UnityEvent<int, int> { }

    /// <summary>
    /// Manages a UI element for a trigger item (an element that can be clicked on in the Trigger Groups Menu to set a new trigger value).
    /// </summary>
    public class TriggerGroupsMenuTriggerItemController : MonoBehaviour
	{
	
		[SerializeField]
		protected Text triggerValueText;
		
		[SerializeField]
        protected Image buttonImage;

		[SerializeField]
        protected Sprite unselectedSprite;

		[SerializeField]
        protected Sprite selectedSprite;
	
		[SerializeField]
        protected Color selectedTextColor;
        protected Color unselectedTextColor;

        protected int triggerableIndex;
        protected int groupIndex;

        protected int triggerValue = -1;
		public int TriggerValue { get { return triggerValue; } }

        // Trigger item selected event
		public OnTriggerGroupsMenuTriggerItemSelectedEventHandler onTriggerItemSelected;


        /// <summary>
        /// Initialize the trigger item information.
        /// </summary>
        /// <param name="newTriggerableIndex">The index of the triggerable module that this trigger item represents.</param>
        /// <param name="newGroupIndex">The index of the triggerable group that this item belongs to.</param>
        /// <param name="defaultTriggerValue">The trigger index to default to.</param>
        public void Init(int newTriggerableIndex, int newGroupIndex, int defaultTriggerValue)
		{
			
			triggerableIndex = newTriggerableIndex;

			groupIndex = newGroupIndex;
	
			buttonImage.sprite = unselectedSprite;
	
			SetTriggerValue(defaultTriggerValue);

			unselectedTextColor = triggerValueText.color;

		}


        /// <summary>
        /// Called when this trigger item is selected in the trigger groups menu.
        /// </summary>
        /// <param name="callEvent">Whether to call the event.</param>
        public void Select(bool callEvent = true)
		{
			buttonImage.sprite = selectedSprite;
			triggerValueText.text = "_";
			triggerValueText.color = selectedTextColor;

            if (callEvent) onTriggerItemSelected.Invoke(triggerableIndex, groupIndex);
		}
	
		
		/// <summary>
        /// Called when this trigger item is no longer focused on by the UI.
        /// </summary>
		public void Deselect()
		{

			buttonImage.sprite = unselectedSprite;
			
			triggerValueText.text = triggerValue.ToString();
		
			triggerValueText.color = unselectedTextColor;

		}
	
		
		/// <summary>
        /// Set a new trigger index for the triggerable module in the trigger group
        /// </summary>
        /// <param name="newTriggerValue">The new trigger index.</param>
		public void SetTriggerValue(int newTriggerValue)
		{
			triggerValue = newTriggerValue;
			triggerValueText.text = triggerValue.ToString();
		}
	}
}