using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Provides an example of a visual effects manager for a jet exhaust.
    /// </summary>
    public class JetExhaustVisualEffectsController : MonoBehaviour
    {
        
        [Header("General")]

        [SerializeField]
        protected Engines engines;

        [Header("Visual Elements")]

        // Glow renderers

        [SerializeField]
        protected List<MeshRenderer> exhaustGlowRenderers = new List<MeshRenderer>();
        List<Material> exhaustGlowMaterials = new List<Material>();

        [SerializeField]
        protected string exhaustGlowShaderColorName = "_TintColor";

        // Halo renderers

        [SerializeField]
        protected List<MeshRenderer> exhaustHaloRenderers = new List<MeshRenderer>();
        List<Material> exhaustHaloMaterials = new List<Material>();

        [SerializeField]
        protected string exhaustHaloShaderColorName = "_TintColor";

        // Particle systems

        [SerializeField]
        protected List<ParticleSystem> exhaustParticleSystems = new List<ParticleSystem>();
        protected ParticleSystem.MainModule[] exhaustParticleSystemMainModules;
        protected List<Material> exhaustParticleMaterials = new List<Material>();
        protected List<float> exhaustParticleStartSpeeds = new List<float>();

        [SerializeField]
        protected string exhaustParticleShaderColorName = "_TintColor";

        // Trail renderers

        [SerializeField]
        protected List<TrailRenderer> exhaustTrailRenderers = new List<TrailRenderer>();
        protected List<Material> exhaustTrailMaterials = new List<Material>();

        [SerializeField]
        protected string exhaustTrailShaderColorName = "_TintColor";

        [Header("Cruising")]

        /// <summary> A curve that describes how the effects change as the throttle values change. </summary> 
        [SerializeField]
        protected AnimationCurve throttleValueToEffectsCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary> The primary color of the exhaust. </summary> 
        [SerializeField]
        protected Gradient exhaustColorGradient = new Gradient();

        [SerializeField]
        protected float maxCruisingGlowAlpha = 0.8f;

        [SerializeField]
        protected float maxCruisingHaloAlpha = 0.3f;

        [SerializeField]
        protected float maxCruisingParticleAlpha = 0.2f;

        [SerializeField]
        protected float maxCruisingParticleSpeedFactor = 1f;

        [SerializeField]
        protected float maxCruisingTrailAlpha = 0.75f;


        [Header("Boost")]

        [SerializeField]
        protected Color boostColor = Color.white;

		[SerializeField]
        protected float boostGlowAlpha = 1f;

        [SerializeField]
        protected float boostHaloAlpha = 0.4f;

		[SerializeField]
        protected float boostParticleAlpha = 0.3f;

		[SerializeField]
        protected float boostParticleSpeedFactor = 2f;

        [SerializeField]
        protected float boostTrailAlpha = 1f;


        // Called when component is first added to a gameobject or reset in the inspector
        protected virtual void Reset()
        {
            exhaustColorGradient.colorKeys = new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.5f, 0f, 1f), 0) };
            engines = GetComponent<Engines>();
        }


		protected virtual void Awake()
		{

            // Cache all of the materials

            for (int i = 0; i < exhaustGlowRenderers.Count; ++i)
			{
				exhaustGlowMaterials.Add(exhaustGlowRenderers[i].material);
			}

			for (int i = 0; i < exhaustHaloRenderers.Count; ++i)
			{
				exhaustHaloMaterials.Add(exhaustHaloRenderers[i].material);
			}

            exhaustParticleSystemMainModules = new ParticleSystem.MainModule[exhaustParticleSystems.Count];
			for (int i = 0; i < exhaustParticleSystems.Count; ++i)
			{
                exhaustParticleSystemMainModules[i] = exhaustParticleSystems[i].main;
                exhaustParticleMaterials.Add(exhaustParticleSystems[i].gameObject.GetComponent<ParticleSystemRenderer>().material);
                exhaustParticleStartSpeeds.Add(exhaustParticleSystemMainModules[i].startSpeed.constant);
			}
            
			for (int i = 0; i < exhaustTrailRenderers.Count; ++i)
			{
				exhaustTrailMaterials.Add(exhaustTrailRenderers[i].material);
			}
        }


        /// <summary>
		/// Reset and clear the exhaust effects.
		/// </summary>
		public virtual void ResetExhaust()
		{
			for (int i = 0; i < exhaustTrailRenderers.Count; ++i)
			{
				exhaustTrailRenderers[i].Clear();
			}
		}


        /// <summary>
        /// Enable or disable the trail renderers .
        /// </summary>
        /// <param name="setEnabled">Whether the trail renderers will be enabled or disabled.</param>
        public virtual void SetExhaustTrailsEnabled(bool setEnabled)
		{
			for (int i = 0; i < exhaustTrailRenderers.Count; ++i)
			{
				exhaustTrailRenderers[i].enabled = setEnabled;
			}
		}


        /// <summary>
        /// Update the exhaust visual effects.
        /// </summary>
        /// <param name="throttleValue">The throttle value for the engine effects.</param
        /// <param name="boostOn">Whether the engine's boost function is on or off.</param>
        public virtual void UpdateEffects ()
		{

            float cruisingEffectsAmount, boostEffectsAmount;

            // If engines assigned, use it to drive the effects
            if (!engines.EnginesActivated)
            {
                cruisingEffectsAmount = 0;
                boostEffectsAmount = 0;
            }
            else
            {
                cruisingEffectsAmount = throttleValueToEffectsCurve.Evaluate(engines.MovementInputs.z);
                boostEffectsAmount = engines.BoostInputs.z;
            }

            float particleAlpha = 0;
			float particleSpeedFactor = 0;
			float haloAlpha = 0;
			float glowAlpha = 0;
			float trailAlpha = 0;
 

            Color c = (1 - boostEffectsAmount) * cruisingEffectsAmount * exhaustColorGradient.Evaluate(cruisingEffectsAmount) + 
                        boostEffectsAmount * boostColor;
            
            particleAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingParticleAlpha + 
                                boostEffectsAmount * boostParticleAlpha;

            particleSpeedFactor = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingParticleSpeedFactor + 
                                    boostEffectsAmount * boostParticleSpeedFactor;

            haloAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingHaloAlpha +
                            boostEffectsAmount * boostHaloAlpha;

            glowAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingGlowAlpha +
                            boostEffectsAmount * boostGlowAlpha;

            trailAlpha = (1 - boostEffectsAmount) * cruisingEffectsAmount * maxCruisingTrailAlpha +
                            boostEffectsAmount * boostTrailAlpha;

            // Update halo materials
			for (int i = 0; i < exhaustHaloMaterials.Count; ++i)
			{
				c.a = haloAlpha;
				exhaustHaloMaterials[i].SetColor(exhaustHaloShaderColorName, c);
			}
			
            // Update glow materials
			for (int i = 0; i < exhaustGlowMaterials.Count; ++i)
			{
				float h, s, v;
				Color.RGBToHSV(c, out h, out s, out v);
				c = Color.HSVToRGB(h, s, v);
				c.a = glowAlpha;
				exhaustGlowMaterials[i].SetColor(exhaustGlowShaderColorName, c);
			}
			
            // Update particle effects
			for (int i = 0; i < exhaustParticleMaterials.Count; ++i)
			{
				c.a = particleAlpha;
				exhaustParticleMaterials[i].SetColor(exhaustParticleShaderColorName, c);
			}
	
            // Update particle speed
			for (int i = 0; i < exhaustParticleSystemMainModules.Length; ++i)
			{
                exhaustParticleSystemMainModules[i].startSpeed = particleSpeedFactor * exhaustParticleStartSpeeds[i];
			}
				
            // Update trail renderer materials
			for (int i = 0; i < exhaustTrailMaterials.Count; ++i)
			{
				c.a = trailAlpha;
				exhaustTrailMaterials[i].SetColor(exhaustTrailShaderColorName, c);
			}
        }

        // Called every frame
        protected virtual void Update()
        {
            UpdateEffects();
        }
	}
}
