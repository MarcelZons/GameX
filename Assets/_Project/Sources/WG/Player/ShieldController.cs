using System;
using System.Collections.Generic;
using UnityEngine;

namespace WG.GameX.Player
{
    public class ShieldController : MonoBehaviour
    {
        [SerializeField] private Gradient _colorGradiant;
        [SerializeField] private int _health;
        [SerializeField] private int _subtractionAmount;
        [SerializeField] private Shield[] _shields;

        private void Awake()
        {
            _shields = GetComponentsInChildren<Shield>();
        }

        private void Start()
        {
            foreach (var shield in _shields)
            {
                shield.Setup(_health, _colorGradiant);
            }
        }
    }
}