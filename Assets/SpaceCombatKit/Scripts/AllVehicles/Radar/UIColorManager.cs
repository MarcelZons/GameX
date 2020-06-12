using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat.Radar
{
    /// <summary>
    /// Manages the color of UI elements and renderers.
    /// </summary>
    public class UIColorManager : MonoBehaviour
    {

        [SerializeField]
        protected bool preserveAlpha = true;  // Whether to preserve the original alpha of each UI element

        [SerializeField]
        protected List<Image> images = new List<Image>();

        [SerializeField]
        protected List<Text> texts = new List<Text>();

        [SerializeField]
        protected List<Renderer> renderers = new List<Renderer>();

     
        /// <summary>
        /// Set the color of the UI elements.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        public virtual void SetColor(Color newColor)
        {

            // Update color of images 
            for (int i = 0; i < images.Count; ++i)
            {
                Color tempColor = images[i].color;

                tempColor.r = newColor.r;
                tempColor.g = newColor.g;
                tempColor.b = newColor.b;

                if (!preserveAlpha) tempColor.a = newColor.a;

                images[i].color = tempColor;
            }

            // Update the color of texts
            for (int i = 0; i < texts.Count; ++i)
            {
                Color tempColor = texts[i].color;

                tempColor.r = newColor.r;
                tempColor.g = newColor.g;
                tempColor.b = newColor.b;

                if (!preserveAlpha) tempColor.a = newColor.a;

                texts[i].color = tempColor;
            }

            // Update the color of renderers
            for (int i = 0; i < renderers.Count; ++i)
            {
                Color tempColor = renderers[i].material.color;

                tempColor.r = newColor.r;
                tempColor.g = newColor.g;
                tempColor.b = newColor.b;

                if (!preserveAlpha) tempColor.a = newColor.a;

                renderers[i].material.color = tempColor;
            }
        }
    }
}