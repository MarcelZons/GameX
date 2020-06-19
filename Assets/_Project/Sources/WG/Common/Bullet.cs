using UnityEngine;
using WG.GameX.Util;

namespace WG.GameX.Common
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _selfDestructTime;
        [SerializeField] private BulletExplode _bulletExplode;
        [SerializeField] private int _damageAmount;
        [SerializeField] private TrailRenderer _trailRenderer;
        private Rigidbody _rigidbody;
        private LineRenderer _lineRenderer;

        public int DamageAmount => _damageAmount;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _lineRenderer = GetComponentInChildren<LineRenderer>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            _lineRenderer.enabled = false;
        }

        private void OnEnable()
        {
            Invoke(nameof(SelfDestruct), _selfDestructTime);
            if(_trailRenderer != null)
                _trailRenderer.emitting = false;
        }

        public void Fire(Vector3 bulletVelocity)
        {
            if (_trailRenderer != null)
            {
                _trailRenderer.Clear();
                _trailRenderer.emitting = true;
            }
                
            
            _lineRenderer.enabled = true;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = bulletVelocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("BulletCollidable") == false)
                return;

            if (_trailRenderer != null)
            {
                _trailRenderer.Clear();
                _trailRenderer.emitting = true;
            }
            
            if (_trailRenderer != null)
            {
                _trailRenderer.Clear();
                _trailRenderer.emitting = false;
            }
                
            
            _lineRenderer.enabled = false;
            _bulletExplode.Play();
            _rigidbody.isKinematic = true;
            Invoke(nameof(SelfDestruct), 1);
        }

        private void SelfDestruct()
        {
            gameObject.SetActive(false);
        }
    }
}