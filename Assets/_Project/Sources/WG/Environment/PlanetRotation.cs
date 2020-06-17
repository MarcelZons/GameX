using System;
using UnityEngine;

namespace WG.GameX.Environment
{
    public class PlanetRotation : MonoBehaviour
    {
        [Range(0,10)]
        [SerializeField] private float _rotationSpeed;
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        // Update is called once per frame
        private void Update()
        {
            _transform.Rotate(Vector3.up * (Time.deltaTime * _rotationSpeed));
        }
    }
    
}
