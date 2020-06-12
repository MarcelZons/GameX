using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Control the color of a set of effects materials.
    /// </summary>
    public class EffectsColorManager : MonoBehaviour
    {

        [SerializeField]
        protected Color effectsColor = Color.white;

        [SerializeField]
        protected string colorID = "_TintColor";

        [SerializeField]
        protected bool preserveAlpha = true;

        [Header("Effects Elements")]

        [SerializeField]
        protected List<Renderer> effectsRenderers = new List<Renderer>();
        protected List<Material> effectsMaterials = new List<Material>();


        private void Awake()
        {
            // Cache the materials
            for (int i = 0; i < effectsRenderers.Count; ++i)
            {
                effectsMaterials.Add(effectsRenderers[i].material);
            }
        }

        // Called every frame
        private void Update()
        {
            // Update material colors
            for (int i = 0; i < effectsMaterials.Count; ++i)
            {
                Color c = effectsColor;
                if (preserveAlpha) c.a = effectsMaterials[i].GetColor(colorID).a;
                effectsMaterials[i].SetColor(colorID, c);
            }

        }
    }
}