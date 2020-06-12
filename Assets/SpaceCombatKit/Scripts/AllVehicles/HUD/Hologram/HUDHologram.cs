using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VSX.Vehicles;
using UnityEngine.UI;

// This script is for managing the hologram of the target

namespace VSX.UniversalVehicleCombat.Radar
{

    public class HUDHologram : HUDComponent
    {
        protected Trackable target;

        [SerializeField]
        protected HUDHologramController targetHologram;

        [Header("Colors")]

        [SerializeField]
        protected Color defaultColor = Color.white;

        [SerializeField]
        protected List<TeamColor> teamColors = new List<TeamColor>();
	
        [Header("Target Label")]

        [SerializeField]
        protected Text label;

        [SerializeField]
        protected float labelSaturationMultiplier = 0.5f;

        [SerializeField]
        protected bool disableLabelIfValueMissing;

        [Header("Keys")]

        [SerializeField]
        protected string labelKey = "Label";

        [SerializeField]
        protected string meshKey = "HologramMesh";

        [SerializeField]
        protected string albedoMapKey = "HologramAlbedoMap";

        [SerializeField]
        protected string normalMapKey = "HologramNormalMap";



        public virtual void OnTargetChanged(Trackable newTarget)
        {

            target = newTarget;

            if (activated) InitializeHologram(target);

        }

        public override void Activate()
        {
            base.Activate();
            InitializeHologram(false);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            InitializeHologram(true);
        }


        protected virtual void InitializeHologram (bool clearHologram)
        {

            if (targetHologram == null) return;

            // Clear hologram if null
            if (clearHologram || target == null)
            {
                targetHologram.SetColor(defaultColor);

                if (label != null)
                {
                    float h, s, v;
                    Color.RGBToHSV(defaultColor, out h, out s, out v);
                    s *= labelSaturationMultiplier;
                    label.color = Color.HSVToRGB(h, s, v);
                }

                targetHologram.Set(null, null, null);
                return;
            }

            // Get color
            Color col = defaultColor;
            for (int i = 0; i < teamColors.Count; ++i)
            {
                if (teamColors[i].team == target.Team)
                {
                    col = teamColors[i].color;
                }
            }

            // Update hologram color
            targetHologram.SetColor(col);

            // Update label color
            if (label != null)
            {
                float h, s, v;
                Color.RGBToHSV(col, out h, out s, out v);
                s *= labelSaturationMultiplier;
                label.color = Color.HSVToRGB(h, s, v);
            }

            // Update target label
            if (label != null)
            {
                if (target.variablesDictionary.ContainsKey(labelKey))
                {
                    label.gameObject.SetActive(true);
                    label.text = target.variablesDictionary[labelKey].StringValue();
                }
                else
                {
                    label.gameObject.SetActive(false);
                }

            }

            Mesh hologramMesh = null;
            if (target.variablesDictionary.ContainsKey(meshKey))
            {
                hologramMesh = (Mesh)target.variablesDictionary[meshKey].ObjectValue();
            }

            Texture2D hologramAlbedo = null;
            if (target.variablesDictionary.ContainsKey(albedoMapKey))
            {
                hologramAlbedo = (Texture2D)target.variablesDictionary[albedoMapKey].ObjectValue();
            }

            Texture2D hologramNormal = null;
            if (target.variablesDictionary.ContainsKey(normalMapKey))
            {
                hologramNormal = (Texture2D)target.variablesDictionary[normalMapKey].ObjectValue();
            }

            // Update hologram
            targetHologram.Set(hologramMesh, hologramAlbedo, hologramNormal);

        }



        void LateUpdate()
        { 			
			// Update it
            if (target != null)
			    targetHologram.UpdateHologram(target.transform, transform);
            
		}
	}
}
