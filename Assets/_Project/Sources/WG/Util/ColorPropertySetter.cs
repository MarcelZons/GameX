using UnityEngine;

namespace WG.GameX.Util
{
    public class ColorPropertySetter : MonoBehaviour
    {
        //The color of the object
        //The material property block we pass to the GPU
        
        [SerializeField] 
        private float _opacity;
        
        private MaterialPropertyBlock propertyBlock;
        private static readonly int Opacity = Shader.PropertyToID("_Opacity");

        private Renderer _renderer;
        private static readonly int RimColor = Shader.PropertyToID("_RimColor");

        private void Awake()
        {
            //create propertyblock only if none exists
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();
            
            //Get a renderer component either of the own gameobject or of a child
            _renderer = GetComponent<Renderer>();
            //set the color property
        }

        public void SetVisibility(float opacityValue, Color rimColor)
        {
            _opacity = opacityValue;
            propertyBlock.SetColor(RimColor,  rimColor);
            //propertyBlock.SetFloat(Opacity, opacityValue);
            //apply propertyBlock to renderer
            _renderer.SetPropertyBlock(propertyBlock);
        }
    }
}