using UnityEngine;

namespace gamex.player
{
    public class PlayerShipAnimationController: MonoBehaviour
    {
        [SerializeField] private Animator _playerShipAnimator;
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");

        public void UpdateAnimator(float horizontal, float vertical)
        {
            _playerShipAnimator.SetFloat(Horizontal, horizontal);
            _playerShipAnimator.SetFloat(Vertical, vertical);
        }
    }
}