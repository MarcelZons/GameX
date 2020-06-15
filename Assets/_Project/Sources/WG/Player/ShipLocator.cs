using System;
using UnityEngine;

namespace WG.GameX.Player
{
    public class ShipLocator : MonoBehaviour
    {
        [Range(0, 2)] public float x, y, z;

        public RectTransform canvas;

        private void Update()
        {
            // Vector2 temp = camera.WorldToViewportPoint(position);
            // temp.x *= canvas.sizeDelta.x;
            // temp.y *= canvas.sizeDelta.y;
            //
            // temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
            // temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

            var pos = new Vector3(x, y, z);
            var newPos = pos * canvas.sizeDelta * canvas.pivot;
            newPos.x = 100;
            GetComponent<RectTransform>().position = newPos;
        }

        private void OnValidate()
        {
            var pos = new Vector3(x, y, z);
            var newPos = pos * canvas.sizeDelta * canvas.pivot;
            newPos.x = newPos.x > 1 ? newPos.x - 50 : newPos.x += 50;
            GetComponent<RectTransform>().position = newPos;
        }
    }
}