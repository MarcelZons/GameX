using System;
using System.Collections.Generic;
using UnityEngine;
using WG.GameX.Managers;

namespace WG.GameX.Player
{
    public class ShieldController : MonoBehaviour
    {
        [SerializeField] private Gradient _colorGradiant;
        private int _health;
        private float _shieldRegerationTime;
        private Shield[] _shields;

        private void Awake()
        {
            _shields = GetComponentsInChildren<Shield>();
        }

        private void Start()
        {
            _health = DependencyMediator.Instance.PlayerShipController.ShieldHealth;
            _shieldRegerationTime = DependencyMediator.Instance.PlayerShipController.ShieldRegerationTime;
            foreach (var shield in _shields)
            {
                shield.Setup(_health, _colorGradiant, _shieldRegerationTime);
            }
        }
    }
}