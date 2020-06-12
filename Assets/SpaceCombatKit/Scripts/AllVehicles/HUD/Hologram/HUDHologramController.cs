using UnityEngine;
using System.Collections;

// This class is for creating a hologram of an trackable object and updating it with the trackable object's
// relative orientation

namespace VSX.Vehicles{

	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]

	public class HUDHologramController : MonoBehaviour {
	
		// Cached components
		MeshFilter hologramMeshFilter;
		MeshRenderer hologramMeshRenderer;
		Material hologramMat;
	
		[SerializeField]
		private float hologramSize = 0.3f;

		// This is for making the outline brighter (less saturated) than the middle
		[SerializeField]
		private float outlineSaturationCoefficient = 0.5f;

		[SerializeField]
		public SpriteRenderer platform;

		[SerializeField]
		private float platformSaturationCoefficient = 0.8f;

        private float outlineWidth;

        public float extentsSizeToOutlineThicknessFactor = 0.01f;

        public bool orient = true;


		void Awake()
		{

			// Cache components
			hologramMeshFilter = GetComponent<MeshFilter>();
			hologramMeshRenderer = GetComponent<MeshRenderer>();
			hologramMat = hologramMeshRenderer.material;
            outlineWidth = hologramMat.GetFloat("_OutlineWidth");			
		}
	
		// Set a new mesh for the hologram
		public void Set(Mesh mesh, Texture2D albedo, Texture2D normal)
		{

			hologramMeshFilter.sharedMesh = mesh;

            if (mesh == null)
            {
                if (platform != null) platform.gameObject.SetActive(false);
                return;
            }

            if (platform != null) platform.gameObject.SetActive(true);

            if (albedo != null) hologramMat.SetTexture("_MainTex", albedo);
            if (normal != null) hologramMat.SetTexture("_NormalMap", normal);
            

            // Adjust the scale according to the size parameter set in inspector
            Vector3 extents = hologramMeshFilter.sharedMesh.bounds.extents;


            float averageDimension = (extents.x + extents.y + extents.z) / 3.0f;
            float scale = hologramSize / (averageDimension * 2);
			transform.localScale = new Vector3(scale, scale, scale);
            
            hologramMat.SetFloat("_OutlineWidth", extentsSizeToOutlineThicknessFactor * (outlineWidth / averageDimension));

        }
	
		// Enable the hologram
		public void Enable()
		{
			hologramMeshRenderer.enabled = true;
	    }
	
		// Disable the hologram
	    public void Disable()
	    {
            hologramMeshRenderer.enabled = false;
	    }
	
		// Set the hologram color
		public void SetColor(Color col)
		{
            if (hologramMat == null) Debug.Log(transform.root.name);
            hologramMat.SetColor("_RimColor", col);

			float h, s, v;
			Color.RGBToHSV(col, out h, out s, out v);
			float _s = outlineSaturationCoefficient * s;
			Color outlineColor = Color.HSVToRGB(h, _s, v);
			
			hologramMat.SetColor("_OutlineColor", outlineColor);
            
            _s = platformSaturationCoefficient * s;
			Color platformColor = Color.HSVToRGB(h, _s, v);

			if (platform != null) platform.color = platformColor;

		}
	
		// Update the hologram orientation. The orientation shows whether the ship in the hologram is facing 
		//the player, regardless of it's relative position
		public void UpdateHologram(Transform targetTransform, Transform trackerTransform)
		{
            if (orient)
            {
                // Calculate orientation
                Vector3 direction = (targetTransform.position - trackerTransform.position).normalized;
                Quaternion temp = Quaternion.LookRotation(direction, trackerTransform.up);
                Quaternion rot = Quaternion.Inverse(Quaternion.Inverse(targetTransform.rotation) * temp);
                transform.localRotation = rot;
            }
		}	
	}
}