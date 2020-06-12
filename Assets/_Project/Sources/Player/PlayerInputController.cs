using UnityEngine;

namespace gamex.player
{
    public class PlayerInputController: MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public float MouseScroll { get; private set; }
        private void Update()
        {
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
            MouseScroll = Input.mouseScrollDelta.y;
        }
    }
}