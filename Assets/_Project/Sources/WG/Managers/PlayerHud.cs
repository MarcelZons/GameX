using UnityEngine;

namespace WG.GameX.Managers
{
    public class PlayerHud : MonoBehaviour
    {
        private Material _material;
        private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");

        private void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        public void SetValue(float value)
        {
            _material.SetFloat(FillAmount, value);
        }
    }
}