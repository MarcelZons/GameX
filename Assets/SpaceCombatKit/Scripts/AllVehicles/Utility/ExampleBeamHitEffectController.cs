using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    public class ExampleBeamHitEffectController : BeamHitEffectController
    {

        [SerializeField]
        protected float maxGlowSize;

        [SerializeField]
        protected float maxGlowAlpha;

        [SerializeField]
        protected float maxSparkSize;

        [SerializeField]
        protected float maxSparkAlpha;

        protected Material glowMaterial;
        protected Material sparkMaterial;

        [SerializeField]
        protected ParticleSystem glowParticleSystem;
        protected ParticleSystem.MainModule glowParticleSystemMainModule;

        [SerializeField]
        protected ParticleSystem sparkParticleSystem;
        protected ParticleSystem.MainModule sparkParticleSystemMainModule;

        protected Transform cachedTransform;
        public Transform CachedTransform { get { return cachedTransform; } }

        protected float beamStrength;



        void Awake()
        {
            // Collect all the materials
            if (glowParticleSystem != null)
            {
                glowParticleSystemMainModule = glowParticleSystem.main;
                glowMaterial = glowParticleSystem.GetComponent<ParticleSystemRenderer>().material;
            }

            if (sparkParticleSystem != null)
            {
                sparkParticleSystemMainModule = sparkParticleSystem.main;
                sparkMaterial = sparkParticleSystem.GetComponent<ParticleSystemRenderer>().material;
            }

            cachedTransform = transform;

            // Deactivate the effect at the start
            SetActivation(false);

        }


        /// <summary>
        /// Called by the BeamSpawn component to update this effect.
        /// </summary>
        /// <param name="level">The 0-1 amount that the beam is on.</param>
        public override void SetLevel(float level)
        {

            if (glowParticleSystem != null)
            {
                glowParticleSystemMainModule.startSize = level * maxGlowSize;

                Color c = glowMaterial.GetColor("_TintColor");
                c.a = level;
                glowMaterial.SetColor("_TintColor", c);
            }

            if (sparkParticleSystem != null)
            {
                sparkParticleSystemMainModule.startSize = level * maxSparkSize;

                Color c = sparkMaterial.GetColor("_TintColor");
                c.a = level;
                sparkMaterial.SetColor("_TintColor", c);
            }
        }
    }
}
