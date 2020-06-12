using UnityEngine;

namespace gamex.player
{
    public class PlayerShipAnimationController
    {
        public PlayerShipAnimationController(Animator playerShipAnimator)
        {
            _playerShipAnimator = playerShipAnimator;
        }
        private Animator _playerShipAnimator;
        
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");

        public void Update(float horizontal, float vertical)
        {
            _playerShipAnimator.SetFloat(Horizontal, horizontal);
            _playerShipAnimator.SetFloat(Vertical, vertical);
        }
    }
}