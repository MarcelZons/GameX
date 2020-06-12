using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VSX.UniversalVehicleCombat
{

	/// <summary>
    /// This class manages a single item in the module selection part of the loadout menu
    /// </summary>
	public class ModuleMenuItemController : MonoBehaviour 
	{
	
		[SerializeField]
		private Text labelText;

        [SerializeField]
        protected Image iconImage;
        
		[SerializeField]
		private Image selectionImage;
		
		[HideInInspector]
		public int itemIndex;

        [SerializeField]
        private Color selectedColor;

        [SerializeField]
		private Color unselectedColor;

		


        /// <summary>
        /// Set the displayed label of the module item.
        /// </summary>
        /// <param name="newLabel">The new label for the module item.</param>
        public void SetLabel(string newLabel)
		{
			labelText.text = newLabel;
		}


        /// <summary>
        /// Set the sprite to be displayed for this module item.
        /// </summary>
        /// <param name="newIcon"></param>
        public void SetIcon(Sprite newIcon)
		{
			iconImage.sprite = newIcon;
		}
	

        /// <summary>
        /// Button event called when the player clicks on this module item in the menu.
        /// </summary>
        /// <param name="updateController">Whether to pass this event to the loadout menu manager.</param>
		public void Select(bool updateController)
		{
			if (updateController)
			{ 
				LoadoutManager.Instance.SelectModule(itemIndex);
			}

            selectionImage.color = selectedColor;
		}
	

        /// <summary>
        /// Event called when this module item is unselected in the module selection UI.
        /// </summary>
		public void Unselect()
        {

            selectionImage.color = unselectedColor;
	
		}
	}
}
