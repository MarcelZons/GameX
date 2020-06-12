using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Highlight and unhighlight a mesh (e.g. when it is part of a trackable targeted by the player)
    /// </summary>
    public class HighlightMesh : MonoBehaviour
    {

        [SerializeField]
        protected Color highlightedAlbedoColor = Color.white;
        protected Color originalColor;

        [SerializeField]
        protected Color highlightColor;

        protected MeshRenderer meshRenderer;


        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            originalColor = meshRenderer.material.color;
        }

        /// <summary>
        /// Highlight the mesh.
        /// </summary>
        public void Highlight()
        {
            meshRenderer.material.color = highlightedAlbedoColor;
            meshRenderer.material.SetColor("_EmissionColor", highlightColor);
        }

        /// <summary>
        /// Unhighlight the mesh.
        /// </summary>
        public void Unhighlight()
        {
            meshRenderer.material.color = originalColor;
            meshRenderer.material.SetColor("_EmissionColor", Color.black);
        }
    }
}
