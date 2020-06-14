using UnityEngine;

namespace WG.GameX.Common
{
    public class BulletExplode : MonoBehaviour
    {
        private ParticleSystem[] _explostionParticles;
        private void OnEnable()
        {
            _explostionParticles = GetComponentsInChildren<ParticleSystem>();
        }

        public void Play()
        {
            foreach (var particle in _explostionParticles)
            {
                particle.Play(true);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Play();
            }
        }
    }
}